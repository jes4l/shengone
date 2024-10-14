using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CCTVHandler : MonoBehaviour
{

    private float timer = 0;

    // Layer mask to specify which layer to search for
    public LayerMask layerMask;
    
    public GameObject cameraObject;

    public GameObject playerObject;

    public Camera cameraComponent;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetObjectsOnLayer();
    }

    void GetObjectsOnLayer()
    {
        timer += Time.deltaTime;
        cameraObject.transform.Rotate(0, oscillate(timer, 1, 0.5f), 0);

        bool dead = false;

        Vector3 cameraPos = cameraObject.transform.position;
        Vector3 playerPos = playerObject.transform.position;

        Vector3 vpPos = cameraComponent.WorldToViewportPoint(playerObject.transform.position);
        // Find all GameObjects with the specified layer
        GameObject[] objectsInLayer = GameObject.FindObjectsOfType<GameObject>();

        // List to store objects found on the specified layer
        List<GameObject> objectsOnLayer = new List<GameObject>();

        RaycastHit hit;

        Vector3 playerPosition = playerObject.transform.position;
        Vector3 cameraPosition = cameraObject.transform.position;



        // Iterate through all found GameObjects
        foreach (GameObject obj in objectsInLayer)
        {
            // Check if the layer of the current GameObject matches the specified layer
            if (((1 << obj.layer) & layerMask) != 0 && vpPos.x >= 0f && vpPos.x <= 1f && vpPos.y >= 0f && vpPos.y <= 1f && vpPos.z > 0f)
            {

                    if (Physics.Raycast(cameraPosition, playerPosition - cameraPosition, out hit))
                    {
                        // Check if the hit object is not the player itself and not the current object being checked
                        if (hit.collider.gameObject != playerObject && hit.collider.gameObject != obj)
                        {
                            // There is an obstacle between camera and player, so skip this object
                            continue;
                        }
                    }

                    // Add the GameObject to the list of objects on the specified layer
                    objectsOnLayer.Add(obj);

                    if (obj.tag == "Illegal")
                    {
                    SceneManager.LoadScene("EndScene", LoadSceneMode.Single);
                    dead = true;
                    }


            }
        }

     
    }


    
 
    float oscillate(float time, float speed, float scale)
    {
        return Mathf.Cos(time * speed / (0.5f * Mathf.PI)) * scale;
    }
}
