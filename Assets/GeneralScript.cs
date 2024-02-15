using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GeneralScript : MonoBehaviour
{
    public Transform Player;
    public Camera gameplayCam;
    public Vector2 camAim;
    bool sqrPressedLastFrame;

    //Funtion to change player movement inrealtion to camera view ratehr than global postioning apon coords
    Vector3 CameraRelativeDirectionalInfluence(Vector3 v3) {

        var unflattenedDI = gameplayCam.transform.rotation * v3;
        unflattenedDI.y = 0;
        return unflattenedDI.normalized * v3.magnitude;
    }
    public enum PlayerState { 
        Idle,
        Aerial,
        BasicAttack,
        Launcher,
        PostLauncher,
        Descender,
        PostDecender,
        AirAttack,
    }

    public PlayerState playerState;
    public float timeInState;

    public GameObject targetGameObject;

    public void SetState(PlayerState state)
    {
        this.playerState = state;
        timeInState = 0;

    }
    public bool IsGrounded() {
        if (Player.position.y == 0)
        {
            return true;
        }
        else
        { 
            return false;
        }

        
    }

    public bool RegularAttackPressed() {
        var gamepad = Gamepad.all;
        if (gamepad[0].buttonWest.isPressed)
        {
            Debug.Log("regular attack pressed");
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool LauncherAttackPressed()
    {
        var gamepad = Gamepad.all;
        if (gamepad[0].buttonNorth.isPressed)
        {
            Debug.Log("launcher attack pressed");
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool JumpPressed()
    {
        
        var gamepad = Gamepad.all;
        if (gamepad[0].buttonSouth.isPressed)
        {
            
            Debug.Log("Jump attack pressed");
            return true;
        }
        else
        {
            return false;
        }

    }
    public bool DealDamageInRange(GameObject objectHit) {

        return false;
    }
    public void KnockUp(GameObject knockedUpObj)
    {
        //This is a test case to see that the implimentation of a damageable object reacts to a change in postion through General Script
        //this.gameObject.transform.position += new Vector3(0, 1, 0) * Time.deltaTime * 100;
        // We instead want to add force to a rigidBody
        knockedUpObj.gameObject.GetComponent<Rigidbody>().AddForce(0, 400, 0);
        // another method to keep in mind
        //knockedUpObj.GetComponent<Rigidbody>().velocity += new Vector3(0, 100, 0);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var playerSpeedMod = 4;
        //list on all controller inputs
        var gamepad = Gamepad.all;
        var directionalInfluence = gamepad[0].leftStick.ReadValue();
        var worldSpaceDirectionalInfluence = new Vector3(directionalInfluence.x, 0, directionalInfluence.y);
        var cameraSpaceDirectionalInfluence = CameraRelativeDirectionalInfluence(worldSpaceDirectionalInfluence);
        camAim += gamepad[0].rightStick.ReadValue() * Time.deltaTime * 100;
        camAim.y = Mathf.Clamp(camAim.y, -80,80);
       // moved the player movement into the statemachine within idle
        if (!(cameraSpaceDirectionalInfluence.magnitude == 0))
        {
            Player.rotation = Quaternion.LookRotation(cameraSpaceDirectionalInfluence); 
        }
        // This rotates around the X Axis before then rotating around the Y Axis (mult on quaterions is adds rotations sequnetally)
        gameplayCam.transform.rotation = Quaternion.AngleAxis(camAim.x, Vector3.up) * Quaternion.AngleAxis(camAim.y, Vector3.right);
        // Finding 
        gameplayCam.transform.position = Player.position - gameplayCam.transform.rotation * Vector3.forward * 4;

        timeInState += Time.deltaTime;
        switch (playerState)
        {
            case PlayerState.Idle:
                {
                    if (!IsGrounded())
                    {
                        SetState(PlayerState.Aerial);
                        break;
                    }
                    if (RegularAttackPressed())
                    {
                        SetState(PlayerState.BasicAttack);
                        break;
                    }   
                        
                    if (LauncherAttackPressed())
                    {   
                        SetState(PlayerState.Launcher);
                        break;
                    }

                    Player.position += cameraSpaceDirectionalInfluence * Time.deltaTime * playerSpeedMod;

                    break;
                }
            case PlayerState.Aerial:
                {
                 if (IsGrounded()) { 
                    SetState(PlayerState.Idle);
                        break;
             
                 }
                 if (RegularAttackPressed()) { 
                    SetState (PlayerState.BasicAttack);
                        break;   
                 }
                 if (LauncherAttackPressed()) {
                    SetState(PlayerState.Descender);
                        break;                   
                    
                 }
                    break;
                }
            case PlayerState.Launcher:
                {
                //check if damagable object is hit
                //if (timeInState >= 0.5f) {
                //        if (DealDamageInRange(out var hitobject))
                //        {
                //            // If hit dectects move into post launcher
                //            targetGameObject = gameObject;
                //            SetState(PlayerState.PostLauncher);
                //            break;
                //        }
                //        else 
                //        {
                //            SetState(PlayerState.Idle);
                //            break;
                //        }
                //}   


                    break;
                }
            case PlayerState.PostLauncher: 
                {
                    if (JumpPressed()) 
                    {
                        //impliment jumping to enemy for follow up
                        SetState(PlayerState.Aerial);
                        //SetVelocityTowardsEnemy(targetGameObject);
                        break;
                    }
                    break;
                }
            case PlayerState.BasicAttack:
                {
                    // wait for attack to activate
                    if (timeInState >= 0.5f) 
                    {
                        //DealDamageInRange(out _);
                        SetState(PlayerState.Idle);
                    
                    }
                    break;
                }
            case PlayerState.AirAttack: 
                {
                    if (timeInState >= 0.5f)
                    {
                        //DealDamageInRange(out _);
                        SetState(PlayerState.Aerial);

                    }
                    break;
                }
            //case PlayerState.Descender: 
            //    {
                  
            //        break;
            //    }
        }
        // this will become a
        if (gamepad[0].buttonWest.isPressed && !sqrPressedLastFrame)
        {
            //create hitbox
            var hitColliders = Physics.OverlapSphere(Player.position, 2);
            for (int i = 0; i < hitColliders.Length; i++) { 
            if (hitColliders[i].gameObject.GetComponent<Damagable>()!= null){ 
                    // Test case to check if collsion is working
                    //GameObject.Destroy(hitColliders[i].gameObject);
                    // A launcher that will push a game object up into the air
                    KnockUp(hitColliders[i].gameObject);
                }
            }
            //Debug.Log("creating hitbox");
        }
        //setting a bool to check for next frame square input
        sqrPressedLastFrame = gamepad[0].buttonWest.isPressed;
    }
}
