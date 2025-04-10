using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNewBasic : MonoBehaviour
{
    public GameObject enemyPrefab;

    public string enemyType;

    private SkinnedMeshRenderer enemyRenderer;

    public Material basicMaterial;
    public Material rangedMaterial;

    public SelfDestruct selfDestruct;

    public AudioSource dyingAudioSource;
    public AudioClip basicDeathClip;
    public AudioClip rangedDeathClip;


    public void InstantiateBlendShapeEnemy()
    {
     
        selfDestruct.EnemyTypeAudio(enemyType);
        Quaternion newRotation = Quaternion.Euler(90, -180, 0);
        GameObject newEnemy = Instantiate(enemyPrefab, transform.position, newRotation);
        EnemyDeathAudioPlayer deathAudioPlayer = newEnemy.GetComponent<EnemyDeathAudioPlayer>();
        if (deathAudioPlayer != null)
        {
            deathAudioPlayer.enemyType = enemyType; // Pass the type string to the prefab
        }
        enemyRenderer = newEnemy.GetComponent<SkinnedMeshRenderer>();
        Light enemyLight = newEnemy.GetComponentInChildren<Light>();


        if (enemyType == "Basic")
        {
            Vector3 flippedScale = newEnemy.transform.localScale;
            flippedScale.x *= -1;
            newEnemy.transform.localScale = flippedScale;
            // If the enemy is of type "basic", change the material to basicMaterial
            enemyRenderer.material = basicMaterial;
            if (enemyLight != null)
                enemyLight.range = 3;

        }
        else if (enemyType == "Ranged")
        {
            newEnemy.transform.localScale = new Vector3(-1.6f, 2.9f, 2f);
            enemyRenderer.material = rangedMaterial;
            if (enemyLight != null)
                enemyLight.range = 6;
        }
        Animator newEnemyAnimator = newEnemy.GetComponent<Animator>();
        if (newEnemyAnimator != null)
        {
            newEnemyAnimator.Play("CrumpleAnimation");
        }
      
    }
    public void EnemyTypeAudio(string enemyType)
    {

        if (enemyType == "Basic" && basicDeathClip != null)
        {
            dyingAudioSource.clip = basicDeathClip;
            dyingAudioSource.Play();
        }
        else if (enemyType == "Ranged" && rangedDeathClip != null)
        {
            dyingAudioSource.clip = rangedDeathClip;
            dyingAudioSource.Play();
        }
    }



}
