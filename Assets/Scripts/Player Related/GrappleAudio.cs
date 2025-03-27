using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleAudio : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip grappleShoot;
    public AudioClip grappleAttach;

    public void GrappleShoot()
    {
        PlaySound(grappleShoot);
    }
    public void GrappleAttach()
    {
        PlaySound(grappleAttach);
    }
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip); // Play the clip without overriding
        }
    }
    public void StopAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
