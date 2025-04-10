using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyAudioScript : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource hurtAudioSource; 

    [Header("Hurt Audio Clips")]
    public AudioClip[] hurtClips;

    private EnemyHealth enemyHealth;
    private float lastHealth;

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();

        if (enemyHealth != null)
            lastHealth = enemyHealth.currentHealth;
    }

    void Update()
    {
        CheckForDamage();
    }

    void CheckForDamage()
    {
        if (enemyHealth == null) return;

        if (enemyHealth.currentHealth < lastHealth)
        {
            PlayHurtSound();
            lastHealth = enemyHealth.currentHealth;
        }
    }

    void PlayHurtSound()
    {
        if (hurtAudioSource != null && hurtClips.Length > 0)
        {

            int index = Random.Range(0, hurtClips.Length);
            hurtAudioSource.PlayOneShot(hurtClips[index]);
           
        }
    }
}
