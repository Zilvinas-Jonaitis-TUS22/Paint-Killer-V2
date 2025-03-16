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

        // Rotate the projectile spawn point around the parent and make it face the player
        Transform player = FindObjectOfType<CharacterController>()?.transform;
        if (projectileSpawnPoint != null && player != null)
        {
            projectileSpawnPoint.RotateAround(transform.position, Vector3.up, rotationSpeed * Time.deltaTime);
            projectileSpawnPoint.LookAt(player.position);
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
