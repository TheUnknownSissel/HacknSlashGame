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
        Player.position +=  cameraSpaceDirectionalInfluence * Time.deltaTime * playerSpeedMod;
        if (!(cameraSpaceDirectionalInfluence.magnitude == 0))
        {
            Player.rotation = Quaternion.LookRotation(cameraSpaceDirectionalInfluence);
        }
        // This rotates around the X Axis before then rotating around the Y Axis (mult on quaterions is adds rotations sequnetally)
        gameplayCam.transform.rotation = Quaternion.AngleAxis(camAim.x, Vector3.up) * Quaternion.AngleAxis(camAim.y, Vector3.right);
        // Finding 
        gameplayCam.transform.position = Player.position - gameplayCam.transform.rotation * Vector3.forward * 4;
        
        if(gamepad[0].buttonWest.isPressed && !sqrPressedLastFrame)
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
            Debug.Log("creating hitbox");
        }
        //setting a bool to check for next frame square input
        sqrPressedLastFrame = gamepad[0].buttonWest.isPressed;
    }
}
