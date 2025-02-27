using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;  // The projectile to throw
    public Transform firePoint;          // The point from where the projectile will be fired
    public float projectileSpeed = 10f;  // Initial speed of the projectile
    public float attackCooldown = 2f;    // Cooldown between attacks
    public float gravity = 9.81f;        // Gravity force (same as Unity's default)
    public float extraLaunchAngle = 30f; // Additional angle added to the trajectory

    public float lastAttackTime = 0f;
    private Vector3 lastTargetPosition;

    void Update()
    {
        // Ensure the debug lines always show
        if (lastTargetPosition != Vector3.zero)
        {
            DrawDebugLines(lastTargetPosition);
        }
    }

    public void FireProjectile(Vector3 targetPosition)
    {
        if (Time.time - lastAttackTime >= attackCooldown) // Cooldown check
        {
            lastTargetPosition = targetPosition; // Store last target position for debug drawing

            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 velocity = CalculateLaunchVelocity(targetPosition);

                // Apply the calculated velocity to the projectile
                rb.velocity = velocity;
            }

            lastAttackTime = Time.time;
        }
    }

    private Vector3 CalculateLaunchVelocity(Vector3 target)
    {
        Vector3 direction = target - firePoint.position;
        float distance = new Vector3(direction.x, 0, direction.z).magnitude; // Horizontal distance
        float heightDifference = direction.y; // Vertical difference

        // Compute the base launch angle needed
        float baseAngle = Mathf.Atan2(heightDifference, distance) * Mathf.Rad2Deg;

        // Apply additional launch angle
        float finalLaunchAngle = baseAngle + extraLaunchAngle;
        float launchAngleRad = finalLaunchAngle * Mathf.Deg2Rad;

        // Calculate velocity using kinematics
        float v0Squared = (gravity * distance * distance) / (2 * (heightDifference - Mathf.Tan(launchAngleRad) * distance) * Mathf.Cos(launchAngleRad) * Mathf.Cos(launchAngleRad));

        if (v0Squared <= 0)
        {
            Debug.LogWarning("Invalid velocity calculation, using default speed.");
            return direction.normalized * projectileSpeed + Vector3.up * projectileSpeed * 0.5f;
        }

        float v0 = Mathf.Sqrt(v0Squared);

        // Split velocity into horizontal and vertical components
        Vector3 velocity = direction.normalized * v0 * Mathf.Cos(launchAngleRad) + Vector3.up * v0 * Mathf.Sin(launchAngleRad);

        return velocity;
    }

    private void DrawDebugLines(Vector3 target)
    {
        Vector3 toTarget = target - firePoint.position;
        Vector3 flatDirection = new Vector3(toTarget.x, 0, toTarget.z).normalized;

        // Direct aim line (Red)
        Debug.DrawRay(firePoint.position, toTarget.normalized * 5, Color.red);

        // Adjusted aim line (Green) - 30° higher (or whatever extraLaunchAngle is set to)
        Vector3 tiltedDirection = Quaternion.AngleAxis(-extraLaunchAngle, Vector3.right) * toTarget.normalized;
        Debug.DrawRay(firePoint.position, tiltedDirection * 5, Color.green);
    }
}
