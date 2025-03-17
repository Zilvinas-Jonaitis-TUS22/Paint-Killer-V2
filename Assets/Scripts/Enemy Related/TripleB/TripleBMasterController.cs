using UnityEngine;

public class TripleBMasterController : MonoBehaviour
{
    [Header("Mechanics")]
    public int meleeDamageAmount = 1;

    [Header("Flood Attack")]
    public float floodAttackDuration = 1.0f;
    private bool isFlooding = false;
    private float floodTimer = 0f;

    public Transform floodStartPosition;
    public Transform floodEndPosition;
    private Vector3 startPos;
    private Vector3 endPos;

    public GameObject floodAttackObject;

    [Header("Sonar/Voice Attack")]

    [Header("Minion Spawn")]

    [Header("Ranged Spray Attack")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint; // The actual spawn point for projectiles
    public Transform projectilePivot; // NEW: The point around which the spawn point rotates
    public float projectileSpeed = 10f;
    public float sprayAngle = 30f;
    public float upwardTilt = 3f; // Adjustable upward tilt
    public float pivotSpeed = 5f; // Speed at which the spawn point pivots around the pivot point

    [Header("References")]
    private BossHealth _BossHealth;

    void Start()
    {
        _BossHealth = GetComponent<BossHealth>();
        if (_BossHealth == null) Debug.LogWarning("BossHealth component is missing!", this);
        floodAttackObject.transform.position = floodStartPosition.position;
    }

    void Update()
    {
        if (isFlooding && floodAttackObject != null)
        {
            floodTimer += Time.deltaTime;
            float lerpValue = Mathf.Clamp01(floodTimer / floodAttackDuration);
            floodAttackObject.transform.position = Vector3.Lerp(startPos, endPos, lerpValue);

            if (lerpValue >= 1f)
            {
                isFlooding = false;
            }
        }

        // Rotate the projectile spawn point around the pivot to face the player
        Transform player = FindObjectOfType<CharacterController>()?.transform;
        if (player != null && projectileSpawnPoint != null && projectilePivot != null)
        {
            Vector3 directionToPlayer = (player.position - projectilePivot.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            // Apply rotation to the spawn point around the pivot
            projectilePivot.rotation = Quaternion.Slerp(projectilePivot.rotation, targetRotation, Time.deltaTime * pivotSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CharacterController character = other.GetComponent<CharacterController>();
        if (character != null)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(meleeDamageAmount);
            }
        }
    }

    public void FloodAttack()
    {
        if (floodAttackObject != null && floodStartPosition != null && floodEndPosition != null)
        {
            startPos = floodStartPosition.position;
            endPos = floodEndPosition.position;
            floodTimer = 0f;
            isFlooding = true;
        }
    }

    public void FloodAttackRemove()
    {
        if (floodAttackObject != null && floodStartPosition != null && floodEndPosition != null)
        {
            startPos = floodEndPosition.position;
            endPos = floodStartPosition.position;
            floodTimer = 0f;
            isFlooding = true;
        }
    }

    public void RangedSprayAttack()
    {
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

        Transform player = FindObjectOfType<CharacterController>()?.transform;
        if (player == null)
        {
            Debug.LogWarning("No player with CharacterController found!");
            return;
        }

        // Get base direction towards the player
        Vector3 baseDirection = (player.position - projectileSpawnPoint.position).normalized;

        // Apply an upward tilt
        baseDirection.y += Mathf.Tan(upwardTilt * Mathf.Deg2Rad);
        baseDirection.Normalize();

        // Spawn projectiles with spray angles
        LaunchProjectile(baseDirection);
        LaunchProjectile(Quaternion.Euler(0, sprayAngle, 0) * baseDirection);
        LaunchProjectile(Quaternion.Euler(0, -sprayAngle, 0) * baseDirection);
        LaunchProjectile(Quaternion.Euler(0, sprayAngle * 2, 0) * baseDirection);
        LaunchProjectile(Quaternion.Euler(0, -sprayAngle * 2, 0) * baseDirection);
    }

    private void LaunchProjectile(Vector3 direction)
    {
        if (projectileSpawnPoint != null && projectilePrefab != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
            if (projectileRb != null)
            {
                projectileRb.velocity = direction * projectileSpeed;
            }
        }
    }
}
