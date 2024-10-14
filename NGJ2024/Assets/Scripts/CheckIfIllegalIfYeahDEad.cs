using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckIfIllegalIfYeahDEad : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Illegal"))
        {
            SceneManager.LoadScene("EndScene", LoadSceneMode.Single);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
