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
    }
    public void ReloadSlideIn()
    {
        PlaySound(reloadSlide);
    }
    public void ReloadThump()
    {
        PlaySound(reloadThump);
    }
    private void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
   
}
