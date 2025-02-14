using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;  // The projectile to throw
    public Transform firePoint;          // The point from where the projectile will be fired
    public float projectileSpeed = 10f;  // Speed at which the projectile travels
    public float attackCooldown = 2f;    // Cooldown between attacks

    private float lastAttackTime = 0f;

    void Update()
    {
        // Check if enough time has passed since last attack
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            // Ready to fire the projectile again
            // We can add logic here to launch the projectile if the enemy is in range (handled in the RangedEnemyAI1 script)
        }
    }

    // Call this function to fire a projectile towards the target
    public void FireProjectile(Vector3 targetPosition)
    {
        if (Time.time - lastAttackTime >= attackCooldown)  // Cooldown check
        {
            // Instantiate the projectile at the fire point
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            // Calculate direction to the player and apply force to the projectile
            Vector3 direction = (targetPosition - firePoint.position).normalized;
            rb.velocity = direction * projectileSpeed;

            // Set the time of the last attack
            lastAttackTime = Time.time;
        }
    }
}
