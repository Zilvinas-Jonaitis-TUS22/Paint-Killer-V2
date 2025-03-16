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
    public Transform projectileSpawnPoint;
    public float projectileSpeed = 10f;
    public float sprayAngle = 30f;
    public float rotationSpeed = 50f; // Speed of rotation around the parent
    public float tiltValue = 3;

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

        // Find the player
        Transform player = FindObjectOfType<CharacterController>()?.transform;
        if (projectileSpawnPoint != null && player != null)
        {
            // Get the direction to the player but ignore the Y-axis to prevent tilting
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0; // Keep rotation level

            // Normalize the direction
            directionToPlayer.Normalize();

            // Calculate the new spawn point position (keep the same distance from parent)
            float distance = Vector3.Distance(transform.position, projectileSpawnPoint.position);
            projectileSpawnPoint.position = transform.position + directionToPlayer * distance;

            // Adjust target Y position by tiltValue
            Vector3 targetPosition = new Vector3(player.position.x, player.position.y + tiltValue, player.position.z);

            // Ensure the spawn point is **always looking at the adjusted target**
            projectileSpawnPoint.LookAt(targetPosition);
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

        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        LaunchProjectile(directionToPlayer);
        LaunchProjectile(Quaternion.Euler(0, sprayAngle, 0) * directionToPlayer);
        LaunchProjectile(Quaternion.Euler(0, -sprayAngle, 0) * directionToPlayer);
        LaunchProjectile(Quaternion.Euler(0, sprayAngle * 2, 0) * directionToPlayer);
        LaunchProjectile(Quaternion.Euler(0, -sprayAngle * 2, 0) * directionToPlayer);
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
