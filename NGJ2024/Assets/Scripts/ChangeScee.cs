using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeZone : MonoBehaviour
{
    public string sceneToLoad; // The name of the scene to load

    // Function called when another Collider enters the trigger zone
    private void OnTriggerEnter(Collider other)
    {
        // Load the specified scene
       	SceneManager.LoadScene(sceneToLoad);
    }
}
