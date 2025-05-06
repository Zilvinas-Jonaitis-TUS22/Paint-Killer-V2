using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public GameObject OriginalThinner;

    // Start is called before the first frame update
    void Start()
    {
        AssignOriginalThinner();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // This method is called when the projectile collides with something
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object we collided with is on the "Player" layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // Find the PlayerHealth component in the scene
            PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1, OriginalThinner.transform); // Deal 1 damage to the player
            }
            else
            {
                Debug.LogError("PlayerHealth component not found in the scene.");
            }
        }

        // Destroy the projectile (make it disappear) after it collides with anything
        Destroy(gameObject);
    }

    // Method to assign the OriginalThinner to the closest RangedEnemyAI
    private void AssignOriginalThinner()
    {
        RangedEnemyAI[] enemies = FindObjectsOfType<RangedEnemyAI>(); // Find all RangedEnemyAI scripts in the scene
        if (enemies.Length == 0)
        {
            Debug.LogWarning("No RangedEnemyAI found in the scene.");
            return;
        }

        RangedEnemyAI closestEnemy = null;
        float closestDistance = Mathf.Infinity; // Set initial distance to a very high number

        foreach (RangedEnemyAI enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestEnemy = enemy;
                closestDistance = distance;
            }
        }

        if (closestEnemy != null)
        {
            OriginalThinner = closestEnemy.gameObject; // Assign the closest enemy's gameObject as OriginalThinner
        }
    }
}
