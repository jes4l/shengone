using UnityEngine;

public class Toilet : MonoBehaviour
{
    public GameObject destructionParticlePrefab; // Drag and drop your particle effect prefab here
    public AudioClip destructionSound; // Drag and drop your destruction sound here
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Function to handle collisions with the wrench
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object is the wrench
        if (collision.gameObject.CompareTag("destroyer"))
        {
            // Play sound effect
            if (destructionSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(destructionSound);
            }

            // Spawn particle effect
            SpawnDestructionParticle();

            // Destroy the toilet object
            Destroy(gameObject);
        }
    }

    // Function to spawn the destruction particle effect
    private void SpawnDestructionParticle()
    {
        if (destructionParticlePrefab != null)
        {
            Instantiate(destructionParticlePrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Destruction particle prefab not set for Toilet script.");
        }
    }
}
