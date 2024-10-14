using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnBoids : MonoBehaviour
{
    [SerializeField]
    float radius;

    [SerializeField]
    private List<GameObject> boidsPrefab; // Assign this in the Inspector

    [SerializeField]
    float speed;

    [SerializeField]
    private List<GameObject> locations;

    [SerializeField]
    float range;

    [SerializeField]
    public int numberOfBoids;

    [SerializeField]
    float MaxTim = 3f;

    float tim = 0f;
    void Update()
    {
        Boids[] boid = FindObjectsOfType<Boids>();
        if (boid.Length < numberOfBoids && tim>MaxTim)
        {
            float angle = 1 * Mathf.PI * 2 / numberOfBoids;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            Vector3 position = transform.position + new Vector3(x, 0, z);
            GameObject boidsInstance = Instantiate(boidsPrefab[Random.Range(0,boidsPrefab.Count)], position, transform.rotation);
            Boids boids = boidsInstance.GetComponent<Boids>();
            boids.range = range;
            boids.speed = speed;
            // Set properties
            for (int j = 0; j < locations.Count; j++)
            {
                
                boids.locations.Add(locations[j]);
            }
            tim = 0;
        }
        tim += Time.deltaTime;
        }
    }
    
