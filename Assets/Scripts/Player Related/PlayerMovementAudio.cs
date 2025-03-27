using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementAudio : MonoBehaviour
{
    public AudioSource movementAudioSource;
    public AudioClip rustlingJacket;
    private FirstPersonController FirstPersonController;

    private void Start()
    {
        FirstPersonController = GetComponent<FirstPersonController>();
        movementAudioSource.loop = true; // Enable looping
    }

    void Update()
    {
        if (FirstPersonController.isSprinting && FirstPersonController.Grounded)
        {
            if (!movementAudioSource.isPlaying) // Play only if it's not already playing
            {
                movementAudioSource.clip = rustlingJacket;
                movementAudioSource.Play();
            }
        }
        else
        {
            if (movementAudioSource.isPlaying)
            {
                movementAudioSource.Stop(); // Stop when not sprinting
            }
        }
    }
}
