using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleBSoundController : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource sonarAttackSound;
    public AudioSource sprayAttackSound;
    public AudioSource floodAttackSound;
    public AudioSource summonAttackSound;
    public AudioSource deathSound;

    private Animator animator;

    private bool prevSonar, prevSpray, prevFlood, prevSummon, prevDead;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        PlaySoundOnBoolChange(ref prevSonar, "SonarAttack", sonarAttackSound);
        PlaySoundOnBoolChange(ref prevSpray, "SprayAttack", sprayAttackSound);
        PlaySoundOnBoolChange(ref prevFlood, "FloodAttack", floodAttackSound);
        PlaySoundOnBoolChange(ref prevSummon, "SpawnEnemies", summonAttackSound);
        PlaySoundOnBoolChange(ref prevDead, "Dead", deathSound);
    }

    private void PlaySoundOnBoolChange(ref bool previous, string boolName, AudioSource sound)
    {
        bool current = animator.GetBool(boolName);

        if (current && !previous && sound != null)
        {
            sound.Play();
        }

        previous = current;
    }
}
