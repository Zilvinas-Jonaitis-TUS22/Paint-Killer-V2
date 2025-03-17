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
    public Transform projectileSpawnPoint;
    public Transform projectilePivot;
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
