using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNewBasic : MonoBehaviour
{
    public GameObject enemyPrefab;

    public string enemyType;

    private Renderer enemyRenderer;

    public Material basicMaterial;
    public Material rangedMaterial;
 
    public void InstantiateBlendShapeEnemy()
    {
     
        Quaternion newRotation = Quaternion.Euler(90, -180, 0);
        GameObject newEnemy = Instantiate(enemyPrefab, transform.position, newRotation);
        enemyRenderer = newEnemy.GetComponent<Renderer>();

        if (enemyType == "Basic")
        {
            Vector3 flippedScale = newEnemy.transform.localScale;
            flippedScale.x *= -1;
            newEnemy.transform.localScale = flippedScale;
            // If the enemy is of type "basic", change the material to basicMaterial
            enemyRenderer.material = basicMaterial;

        }
        else if (enemyType == "Ranged")
        {
            newEnemy.transform.localScale = new Vector3(-2f, 2f, 2f);
            enemyRenderer.material = rangedMaterial;
        }
        Animator newEnemyAnimator = newEnemy.GetComponent<Animator>();
        if (newEnemyAnimator != null)
        {
            newEnemyAnimator.Play("CrumpleAnimation");
        }
      
    }

   
    
}
