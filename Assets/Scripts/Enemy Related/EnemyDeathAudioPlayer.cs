using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathAudioPlayer : MonoBehaviour
{
    public string enemyType; // Set this from SpawnNewBasic
    public AudioSource audioSource;
    public AudioClip basicDeathClip;
    public AudioClip rangedDeathClip;
    void Start()
    {
        PlayDeathAudio();
    }

    void PlayDeathAudio()
    {
        
        AudioClip selectedClip = null;

        if (enemyType == "Basic") selectedClip = basicDeathClip;
        else if (enemyType == "Ranged") selectedClip = rangedDeathClip;

        if (audioSource != null && selectedClip != null)
        {
            audioSource.clip = selectedClip;
            audioSource.Play();
        }

        Debug.Log("Enemy type is: " + enemyType);
    }
}
