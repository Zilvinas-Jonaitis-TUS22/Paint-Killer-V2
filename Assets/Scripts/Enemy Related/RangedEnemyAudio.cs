using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource gruntAudioSource;
    public AudioSource hurtAudioSource;

    [Header("Grunt Audio Clips")]
    public AudioClip[] randomGrunts;

    [Header("Hurt Audio Clips")]
    public AudioClip[] hurtGrunts;

    [Header("Grunt Settings")]
    public float minGruntInterval = 3f;
    public float maxGruntInterval = 7f;

    private float gruntTimer;
    private EnemyHealth enemyHealth;
    private float lastHealth;

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();

        if (enemyHealth != null)
            lastHealth = enemyHealth.currentHealth;

        ResetGruntTimer();
    }

    void Update()
    {
        HandleGruntSounds();
        CheckForDamage();
    }

    void HandleGruntSounds()
    {
        gruntTimer -= Time.deltaTime;
        if (gruntTimer <= 0f && randomGrunts.Length > 0)
        {
            int index = Random.Range(0, randomGrunts.Length);
            gruntAudioSource.PlayOneShot(randomGrunts[index]);
            ResetGruntTimer();
        }
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
        if (hurtAudioSource != null && hurtGrunts.Length > 0)
        {
            
            int index = Random.Range(0, hurtGrunts.Length);
            hurtAudioSource.PlayOneShot(hurtGrunts[index]);
          
        }
    }

    void ResetGruntTimer()
    {
        gruntTimer = Random.Range(minGruntInterval, maxGruntInterval);
    }


}
