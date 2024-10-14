using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundAttenuation : MonoBehaviour
{
    public Transform listener; // This is the object that represents the listener (e.g., camera or player)
    public AudioSource audioSource;
    public float maxDistance = 10f; // Maximum distance at which the sound is audible at full volume

    void Update()
    {
        if (listener == null || audioSource == null)
            return;

        // Calculate the distance between the audio source and the listener
        float distance = Vector3.Distance(transform.position, listener.position);

        // Calculate the volume based on distance
        float volume = Mathf.Clamp01(1f - (distance / maxDistance));

        // Set the volume of the audio source
        audioSource.volume = volume;
    }
}
