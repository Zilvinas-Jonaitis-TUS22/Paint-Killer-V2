using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // For NavMeshAgent

public class MeleeDamage : MonoBehaviour
{
    public NavMeshAgent enemyNavMeshAgent; // Link the enemy's NavMeshAgent via the Inspector
    private Transform player; // Link the player object in the Inspector (will be auto-assigned)
    public float sphereRadius = 1f; // Radius of the sphere area
    public float sphereDistance = 1.5f; // Distance from the enemy's main body to position the sphere
    public int damageAmount = 1; // Amount of damage to deal
    public float damageInterval = 1f; // How often the damage is dealt (in seconds)

    private Vector3 spherePosition;
    private float lastDamageTime;

    // Start is called before the first frame update
    void Start()
    {
        // Find the player by looking for the PlayerHealth script attached to any object in the scene
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            player = playerHealth.transform; // Get the transform of the player object with the PlayerHealth script
        }
        else
        {
            Debug.LogError("No object with PlayerHealth script found in the scene.");
        }

        lastDamageTime = 0f; // Initialize the last damage time
    }

    // Update is called once per frame
    void Update()
    {
        // Update the position of the sphere area slightly in front of the enemy, pointing towards the player
        UpdateSpherePosition();

        // Show the debug sphere to visualize the area
        Debug.DrawRay(transform.position, (spherePosition - transform.position), Color.green);
        Debug.DrawRay(transform.position, transform.forward * sphereDistance, Color.red);

        // Check if the player is within the sphere's area
        CheckForPlayerInSphere();
    }

    // Update the position of the sphere relative to the enemy, facing the player
    void UpdateSpherePosition()
    {
        // Calculate the direction from the enemy to the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Position the sphere slightly outside the enemy's body, pointing towards the player
        spherePosition = transform.position + (directionToPlayer * sphereDistance);
    }

    // Check if the player is within the sphere and deal damage
    void CheckForPlayerInSphere()
    {
        // Calculate the distance between the enemy and the player
        float distanceToPlayer = Vector3.Distance(spherePosition, player.position);

        // If the player is within the sphere radius and the damage interval has passed
        if (distanceToPlayer <= sphereRadius && Time.time >= lastDamageTime + damageInterval)
        {
            // Apply damage to the player
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount); // Deal damage to the player
                lastDamageTime = Time.time; // Update the last damage time
            }
            else
            {
                Debug.LogError("PlayerHealth component not found on the player object.");
            }
        }
    }

    // Visualize the sphere using a debug sphere for easy debugging
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(spherePosition, sphereRadius);
    }
}
