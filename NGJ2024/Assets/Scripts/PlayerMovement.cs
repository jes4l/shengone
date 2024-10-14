using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
   
   
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
   
    [SerializeField]
    float playerGravity;

    [SerializeField]
    CharacterController characterController;

    [SerializeField]
    public float speed = 5.0f;
    [SerializeField]
    public float mouseSensitivity = 2.0f;
    [SerializeField]
    Camera cam;


    private float yaw = 0.0f;
    private float pitch = 0.0f;

  
    [SerializeField]
    float crouchHeight;
    void Update()
    {

        float tempSpeed = speed;
        cam.transform.position = transform.position;
      

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            tempSpeed = speed/2;
            cam.transform.position = transform.position - new Vector3(0f,crouchHeight,0f);
        }
       
        // Get the horizontal and vertical input (forward/back and strafe)
        float moveForward = Input.GetAxis("Vertical") * tempSpeed;
        float moveSideWays = Input.GetAxis("Horizontal") * tempSpeed;
        
            // Create a direction vector based on user input
            Vector3 direction = new Vector3(moveSideWays, transform.position.y-1-tempSpeed, moveForward);

      
        
        // Transform the direction from local space to world space relative to the camera
        direction = cam.transform.TransformDirection(direction);
        // Move the character controller in the calculated direction
       
        if (characterController.collisionFlags == CollisionFlags.None)
        {
            // If there's no obstacle, move the character
            characterController.Move(direction);
        }
        else
        {
            direction.y = 0f;
            characterController.Move(direction);
           
        }
        // Get the mouse movement input
        yaw += mouseSensitivity * Input.GetAxis("Mouse X");
        pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -60f, 60f);
        if (moveForward != 0f || moveSideWays != 0f)
        {
            pitch += Mathf.Sin(Time.time*5)*0.08f;
        }
        // Rotate the camera based on mouse input
        cam.transform.rotation = Quaternion.Euler(pitch, yaw, 0.0f);
      
    }
}
