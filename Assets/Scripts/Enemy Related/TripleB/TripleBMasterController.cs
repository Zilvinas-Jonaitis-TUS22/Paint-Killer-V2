using UnityEngine;
using System.Collections;

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

    [Header("Ranged Spray Attack")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public Transform projectilePivot;
    public float projectileSpeed = 10f;
    public float sprayAngle = 30f;
    public float upwardTilt = 3f;
    public float pivotSpeed = 5f;

    [Header("Minion Spawn Attack")]
    public GameObject minionPrefab; // Minion prefab to spawn
    public BoxCollider minionSpawnArea1; // First spawn area
    public BoxCollider minionSpawnArea2; // Second spawn area

    [Header("Delays")]
    public float spawnAttackDelay = 3f; // Delay after spawning enemies
    public float sprayAttackDelay = 4f; // Delay after spray attack
    public float floodAttackDelay = 2f; // Delay after flood attack
    public float sonarAttackDelay = 2f; // Delay after sonar attack

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

    // Attack sequencing (called via Animation Events)
    public void AnimationEventDead()
    {
        Animator.SetBool("Dead", true); // Trigger death animation
    }

    public void AnimationEventSonar()
    {
        Animator.SetTrigger("Sonar Attack"); // Trigger sonar attack animation
    }

    public void AnimationEventSpray()
    {
        Animator.SetTrigger("Spray Attack"); // Trigger spray attack animation
    }

    public void AnimationEventFlood()
    {
        Animator.SetTrigger("Flood Attack"); // Trigger flood attack animation
    }

    public void AnimationEventDestroyFlood()
    {
        Animator.SetTrigger("Destroy Flood"); // Trigger destroy flood animation
    }

    public void AnimationEventSpawn()
    {
        Animator.SetTrigger("Spawn Enemies"); // Trigger spawn enemies animation
    }

    // Attack Sequencer (for internal control, triggered by animation)
    public void AttackSequencers()
    {
        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        // 1. Spawn enemies
        AnimationEventSpawn();
        yield return new WaitForSeconds(spawnAttackDelay); // Wait for delay after spawn

        // 2. Spray attack 2 times
        for (int i = 0; i < 2; i++)
        {
            AnimationEventSpray();
            yield return new WaitForSeconds(sprayAttackDelay); // Wait after each spray attack
        }

        // 3. Spawn enemies again
        AnimationEventSpawn();
        yield return new WaitForSeconds(spawnAttackDelay); // Delay after spawn

        // 4. Spray attack 3 times
        for (int i = 0; i < 3; i++)
        {
            AnimationEventSpray();
            yield return new WaitForSeconds(sprayAttackDelay); // Wait after each spray attack
        }

        // 5. Summon flood
        AnimationEventFlood();
        yield return new WaitForSeconds(floodAttackDelay); // Wait after flood attack

        // 6. Sonar attack
        AnimationEventSonar();
        yield return new WaitForSeconds(sonarAttackDelay); // Wait after sonar attack

        // 7. Destroy flood
        AnimationEventDestroyFlood();

        // Repeat the sequence until death
        if (_BossHealth != null && _BossHealth.currentHealth > 0)
        {
            yield return new WaitForSeconds(2f); // Optional delay before restarting the sequence
            StartCoroutine(AttackSequence());
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

    public void MinionSpawnAttack()
    {
        if (minionPrefab == null || minionSpawnArea1 == null || minionSpawnArea2 == null)
        {
            Debug.LogWarning("Minion prefab or spawn areas are not assigned!");
            return;
        }

        // Get random spawn positions inside the box colliders
        Vector3 spawnPos1 = GetRandomPointInCollider(minionSpawnArea1);
        Vector3 spawnPos2 = GetRandomPointInCollider(minionSpawnArea2);

        // Spawn a minion at each spawn position
        Instantiate(minionPrefab, spawnPos1, Quaternion.identity);
        Instantiate(minionPrefab, spawnPos2, Quaternion.identity);
    }

    private Vector3 GetRandomPointInCollider(BoxCollider collider)
    {
        if (collider != null)
        {
            Vector3 center = collider.bounds.center;
            Vector3 size = collider.bounds.size;
            return new Vector3(
                Random.Range(center.x - size.x / 2, center.x + size.x / 2),
                Random.Range(center.y - size.y / 2, center.y + size.y / 2),
                Random.Range(center.z - size.z / 2, center.z + size.z / 2)
            );
        }
        return Vector3.zero;
    }
}
