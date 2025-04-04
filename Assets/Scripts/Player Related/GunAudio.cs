using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAudio : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip shootGun;
    public AudioClip cockGun;
    public AudioClip reloadGun;
    public AudioClip reloadSlide;
    public AudioClip reloadThump;
    public AudioClip pullOutReload;
   

    public AudioSource audioSource;
  

    public void GunShoot()
    {
        PlaySound(shootGun);
    }
    public void GunCock()
    {
        PlaySound(cockGun);
    }

    public void GunReload()
    {
        PlaySound(reloadGun);

        //Debug.Log("Reloading");
    }
    public void ReloadSlideIn()
    {
        PlaySound(reloadSlide);
    }
    public void ReloadThump()
    {
        PlaySound(reloadThump);
    }
    public void PullOutReload()
    {
        PlaySound(pullOutReload);
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
