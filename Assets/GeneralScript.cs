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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // sto
        var gamepad = Gamepad.all;
        // vecotr2
        var directionalInfluence = gamepad[0].leftStick.ReadValue();
        var worldSpaceDirectionalInfluence = new Vector3(directionalInfluence.x, 0, directionalInfluence.y);
        var cameraSpaceDirectionalInfluence = CameraRelativeDirectionalInfluence(worldSpaceDirectionalInfluence);
        camAim += gamepad[0].rightStick.ReadValue() * Time.deltaTime * 100;
        camAim.y = Mathf.Clamp(camAim.y, -80,80);
        Player.position +=  cameraSpaceDirectionalInfluence * Time.deltaTime;
        if (!(cameraSpaceDirectionalInfluence.magnitude == 0))
        {
            Player.rotation = Quaternion.LookRotation(cameraSpaceDirectionalInfluence);
        }
        gameplayCam.transform.rotation = Quaternion.AngleAxis(camAim.x, Vector3.up) * Quaternion.AngleAxis(camAim.y, Vector3.right);
        gameplayCam.transform.position = Player.position - gameplayCam.transform.rotation * Vector3.forward * 4;
        
        if(gamepad[0].buttonWest.isPressed && !sqrPressedLastFrame)
        {
            //create hitbox
            var hitColliders = Physics.OverlapSphere(Player.position, 1);
            for (int i = 0; i < hitColliders.Length; i++) { 
            if (hitColliders[i].gameObject.GetComponent<Damagable>()!= null){ 
                GameObject.Destroy(hitColliders[i].gameObject);
                }
            }
            Debug.Log("creating hitbox");
        }
        //checking if the 
        sqrPressedLastFrame = gamepad[0].buttonWest.isPressed;
    }
}
