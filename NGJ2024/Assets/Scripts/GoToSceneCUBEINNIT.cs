using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToSceneCUBEINNIT : MonoBehaviour
{
   
    Collider player;
    PassPortIllegalscrpt pss;
    
    string ID = StaticScript.charachteristics;
   
    // Update is called once per frame
    void Update()
    {
        Vector3 center = transform.position;
        float radius = 5.0f;

        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].CompareTag("Player"))
            {
                Debug.Log("Player is within the sphere");
                player = hitColliders[i];
            }

            if (hitColliders[i].CompareTag("passport"))
            {
                Debug.Log("Passport is within the sphere");
                 pss = hitColliders[i].gameObject.GetComponent<PassPortIllegalscrpt>();
              
            }

            i++;
        }

        if (player != null && pss != null)
        {
            if (pss.ID != ID)
            {
                SceneManager.LoadScene("EndScene", LoadSceneMode.Single);
            }
            else
            {
                SceneManager.LoadScene("WinScene", LoadSceneMode.Single);
            }
        }
    }
}
