using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBrain : MonoBehaviour
{
    private Transform childObject; // Reference to the child camera object

    void Start()
    {
        // Find the child camera object named "Topcamera"
        childObject = transform.Find("Topcamera");
    }

    void Update()
    {
        // Calculate the new rotation angle
        float newY = Mathf.Clamp(Mathf.Sin(Time.time), -0.3f, 0.3f);

        // Create a new rotation Quaternion
        if (childObject != null)
        {
            // Rotate the child object around the Y-axis
            childObject.Rotate(0f, newY, 0f);
        }
    }
}
