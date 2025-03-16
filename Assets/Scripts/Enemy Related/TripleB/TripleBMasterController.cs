using UnityEngine;

public class TripleBMasterController : MonoBehaviour
{
    [Header("Mechanics")]
    public int meleeDamageAmount = 1;

    [Header("Flood Attack")]
    public float floodAttackDuration = 1.0f; // Duration for the flood attack to rise
    private bool isFlooding = false; // A flag to prevent simultaneous flooding
    private float floodTimer = 0f; // Timer to track the flood attack's progress

    // Positions for flood attack
    public Transform floodStartPosition; // Reference to the starting position
    public Transform floodEndPosition;   // Reference to the ending position
    private Vector3 startPos; // Starting position for the flood attack
    private Vector3 endPos;   // Ending position for the flood attack

    // The object representing the flood attack
    public GameObject floodAttackObject;

    [Header("Sonar/Voice Attack")]

    [Header("Minion Spawn")]

    [Header("Ranged Spray Attack")]
    public GameObject projectilePrefab; // The projectile prefab
    public Transform projectileSpawnPoint; // The spawn point for the projectiles
    public float projectileSpeed = 10f; // Speed at which projectiles travel
    public float sprayAngle = 30f; // Angle between projectiles

    [Header("References")]
    private BossHealth _BossHealth;

    void Start()
    {
        // Attempt to find and assign all the required components
        _BossHealth = GetComponent<BossHealth>();

        // Optional: Log a message if a component is missing (useful for debugging)
        if (_BossHealth == null) Debug.LogWarning("BossHealth component is missing!", this);

        // Store the original position of the GameObject
        floodAttackObject.transform.position = floodStartPosition.position;
    }

    void Update()
    {
        if (isFlooding && floodAttackObject != null)
        {
            // Update the position of the flood attack object based on the timer and Lerp
            floodTimer += Time.deltaTime;

            // Calculate the normalized time (0 to 1) for lerping
            float lerpValue = Mathf.Clamp01(floodTimer / floodAttackDuration);
            floodAttackObject.transform.position = Vector3.Lerp(startPos, endPos, lerpValue);

            // If the flood attack duration is over, stop the movement and end the flood attack
            if (lerpValue >= 1f)
            {
                isFlooding = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>(); // Get PlayerHealth component

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(meleeDamageAmount); // Deal damage to the player
            }
        }
    }

    public void FloodAttack()
    {
        if (floodAttackObject != null && floodStartPosition != null && floodEndPosition != null)
        {
            startPos = floodStartPosition.position; // Use floodStartPosition for the start position
            endPos = floodEndPosition.position;     // Use floodEndPosition for the end position
            floodTimer = 0f; // Reset the timer
            isFlooding = true; // Start flooding
        }
    }

    // Call this function to return the flood attack object back to its original position
    public void FloodAttackRemove()
    {
        if (floodAttackObject != null && floodStartPosition != null && floodEndPosition != null)
        {
            startPos = floodEndPosition.position; // Start at the end position
            endPos = floodStartPosition.position; // Go back to the start position
            floodTimer = 0f; // Reset the timer
            isFlooding = true; // Start flooding back to the start position
        }
    }

    public void RangedSprayAttack()
    {
        // Debugging: Check if projectilePrefab and projectileSpawnPoint are set
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Projectile prefab is not assigned!");
            return;
        }

        if (projectileSpawnPoint == null)
        {
            Debug.LogWarning("Projectile spawn point is not assigned!");
            return;
        }

        // Find the player and calculate the direction
        Transform player = GameObject.FindWithTag("Player").transform; // Find the player by tag
        Vector3 directionToPlayer = (player.position - transform.position).normalized; // Calculate direction towards player

        // Launch 5 projectiles at different angles
        LaunchProjectile(directionToPlayer); // 0 degree offset, directly at the player
        LaunchProjectile(Quaternion.Euler(0, sprayAngle, 0) * directionToPlayer); // 30 degree offset to the right
        LaunchProjectile(Quaternion.Euler(0, -sprayAngle, 0) * directionToPlayer); // 30 degree offset to the left
        LaunchProjectile(Quaternion.Euler(0, sprayAngle * 2, 0) * directionToPlayer); // 60 degree offset to the right
        LaunchProjectile(Quaternion.Euler(0, -sprayAngle * 2, 0) * directionToPlayer); // 60 degree offset to the left
    }

    private void LaunchProjectile(Vector3 direction)
    {
        // Ensure a valid spawn point and projectile prefab exist
        if (projectileSpawnPoint != null && projectilePrefab != null)
        {
            // Debugging: Log position and direction
            Debug.Log("Spawning projectile at: " + projectileSpawnPoint.position);
            Debug.Log("Direction: " + direction);

            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity); // Create the projectile at the spawn point
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>(); // Get the Rigidbody component

            if (projectileRb != null)
            {
                projectileRb.velocity = direction * projectileSpeed; // Set the velocity of the projectile
            }
            else
            {
                Debug.LogError("Rigidbody is missing on the projectile prefab.");
            }
        }
    }
}
