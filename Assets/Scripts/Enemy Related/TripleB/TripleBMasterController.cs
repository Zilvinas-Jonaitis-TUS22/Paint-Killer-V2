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
    public int sonarLimit = 0;
    public GameObject sonarProjectilePrefab;
    public Transform sonarSpawnPoint; // Where the sonar projectile spawns
    public Transform sonarPivot; // The pivot point for aiming
    public float sonarProjectileSpeed = 15f; // Speed of the sonar attack
    public float sonarUpwardTilt = 5f; // Adjustable upward tilt
    public float sonarPivotSpeed = 3f; // Speed of the pivot rotation

    [Header("Minion Spawn")]

    [Header("Ranged Spray Attack")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint; // The actual spawn point for projectiles
    public Transform projectilePivot; // NEW: The point around which the spawn point rotates
    public float projectileSpeed = 10f;
    public float sprayAngle = 30f;

    public float upwardTilt = 3f;
    public float pivotSpeed = 5f;

    [Header("References")]
    private BossHealth _BossHealth;
    private Animator Animator;

    void Start()
    {
        Animator = GetComponent<Animator>();
        if (Animator == null) Debug.LogWarning("Animator component is missing!", this);
        _BossHealth = GetComponent<BossHealth>();
        if (_BossHealth == null) Debug.LogWarning("BossHealth component is missing!", this);
        floodAttackObject.transform.position = floodStartPosition.position;

        AimPivotAtPlayer(projectilePivot, projectileSpawnPoint, upwardTilt, pivotSpeed);
        AimPivotAtPlayer(sonarPivot, sonarSpawnPoint, sonarUpwardTilt, sonarPivotSpeed);
    }

    private void AimPivotAtPlayer(Transform pivot, Transform spawnPoint, float tilt, float speed)
    {
        Transform player = FindObjectOfType<CharacterController>()?.transform;
        
            if (player != null && pivot != null && spawnPoint != null)
            {
                Vector3 directionToPlayer = (player.position - pivot.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

                targetRotation *= Quaternion.Euler(-tilt, 0, 0); // Apply upward tilt
                pivot.rotation = Quaternion.Slerp(pivot.rotation, targetRotation, Time.deltaTime * speed);
            }
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
            if (projectilePrefab == null || projectileSpawnPoint == null)
            {
                Debug.LogWarning("Projectile prefab or spawn point is not assigned!");
                return;
            }

        Vector3 baseDirection = projectileSpawnPoint.forward;

        LaunchProjectile(baseDirection, projectileSpeed);
        LaunchProjectile(Quaternion.Euler(0, sprayAngle, 0) * baseDirection, projectileSpeed);
        LaunchProjectile(Quaternion.Euler(0, -sprayAngle, 0) * baseDirection, projectileSpeed);
        LaunchProjectile(Quaternion.Euler(0, sprayAngle * 2, 0) * baseDirection, projectileSpeed);
        LaunchProjectile(Quaternion.Euler(0, -sprayAngle * 2, 0) * baseDirection, projectileSpeed);
    }

    public void SonarAttack()
    {
        if (sonarProjectilePrefab == null || sonarSpawnPoint == null)
        {
            Debug.LogWarning("Sonar projectile prefab or spawn point is not assigned!");
            return;
        }

        // Fire a single sonar projectile
        LaunchSonarProjectile(sonarSpawnPoint.forward, sonarProjectileSpeed);
    }
    private void LaunchProjectile(Vector3 direction, float speed)
    {
        if (projectileSpawnPoint != null && projectilePrefab != null)
        {
            // Instantiate the projectile and set its rotation
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.LookRotation(direction));

            // Apply velocity
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
            if (projectileRb != null)
            {
                projectileRb.velocity = direction * speed;
            }
        }
    }

    private void LaunchSonarProjectile(Vector3 direction, float speed)
    {
        if (projectileSpawnPoint != null && projectilePrefab != null)
            if (sonarProjectilePrefab != null && sonarSpawnPoint != null)
            {
                // Instantiate the sonar projectile and set its rotation
                GameObject projectile = Instantiate(sonarProjectilePrefab, sonarSpawnPoint.position, Quaternion.LookRotation(direction));

                // Apply velocity
                Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
                if (projectileRb != null)
                {
                    projectileRb.velocity = direction * speed;
                }
                sonarLimit++;
                Animator.SetInteger("SonarLimit", sonarLimit);
            }
    }
}
