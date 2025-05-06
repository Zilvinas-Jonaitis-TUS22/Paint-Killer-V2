using UnityEngine;
using System.Collections;

public class TripleBMasterController : MonoBehaviour
{
    [Header("Mechanics")]
    public int meleeDamageAmount = 1;
    public bool playerInZone = false;

    [Header("Sonar/Voice Attack")]
    public int sonarLimit = 0;
    public GameObject sonarProjectilePrefab;
    public Transform sonarSpawnPoint;
    public Transform sonarPivot;
    public float sonarProjectileSpeed = 15f;
    public float sonarUpwardTilt = 5f;
    public float sonarPivotSpeed = 3f;

    [Header("Ranged Spray Attack")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public Transform projectilePivot;
    public float projectileSpeed = 10f;
    public float sprayAngle = 30f;
    public float upwardTilt = 3f;
    public float pivotSpeed = 5f;

    [Header("Minion Spawn Attack")]
    public GameObject minionPrefab;
    public GameObject minionSpawnArea1;
    public GameObject minionSpawnArea2;
    public GameObject minionSpawnArea3;

    [Header("Delays")]
    public float spawnAttackDelay = 3f;
    public float sprayAttackDelay = 4f;
    public float sonarAttackDelay = 2f;

    [Header("References")]
    public BossHealth _BossHealth;
    public Animator Animator;

    public bool isAttackSequenceActive = false;

    void Start()
    {
        Animator = GetComponent<Animator>();
        _BossHealth = GetComponent<BossHealth>();
    }

    void Update()
    {
        AimPivotAtPlayer(projectilePivot, projectileSpawnPoint, upwardTilt, pivotSpeed);
        AimPivotAtPlayer(sonarPivot, sonarSpawnPoint, sonarUpwardTilt, sonarPivotSpeed);

        if (_BossHealth != null && _BossHealth.currentHealth > 0 && !isAttackSequenceActive && playerInZone)
        {
            StartCoroutine(AttackSequence());
        }
    }

    private void AimPivotAtPlayer(Transform pivot, Transform spawnPoint, float tilt, float speed)
    {
        Transform player = FindObjectOfType<CharacterController>()?.transform;
        if (player != null && pivot != null && spawnPoint != null)
        {
            Vector3 directionToPlayer = (player.position - pivot.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            targetRotation *= Quaternion.Euler(-tilt, 0, 0);
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
                playerHealth.TakeDamage(meleeDamageAmount, gameObject.transform);
            }
        }
    }

    public void AnimationEventDead() => Animator.SetBool("Dead", true);
    public void AnimationEventSonar() => Animator.SetBool("SonarAttack", true);
    public void AnimationEventSpray() => Animator.SetBool("SprayAttack", true);
    public void AnimationEventFlood() => Animator.SetBool("FloodAttack", true);
    public void AnimationEventFalseDead() => Animator.SetBool("Dead", false);
    public void AnimationEventFalseSonar() => Animator.SetBool("SonarAttack", false);
    public void AnimationEventFalseSpray() => Animator.SetBool("SprayAttack", false);
    public void AnimationEventFalseFlood() => Animator.SetBool("FloodAttack", false);
    public void AnimationEventFalseSpawn() => Animator.SetBool("SpawnEnemies", false);
    public void AnimationEventSpawn() => Animator.SetBool("SpawnEnemies", true);

    private IEnumerator AttackSequence()
    {
        isAttackSequenceActive = true;

        yield return StartCoroutine(SpawnEnemiesCoroutine());
        yield return new WaitForSeconds(spawnAttackDelay);

        yield return StartCoroutine(SprayAttackCoroutine(2));
        yield return new WaitForSeconds(sprayAttackDelay);

        yield return StartCoroutine(SpawnEnemiesCoroutine());
        yield return new WaitForSeconds(spawnAttackDelay);

        yield return StartCoroutine(SprayAttackCoroutine(3));
        yield return new WaitForSeconds(sprayAttackDelay);

        yield return StartCoroutine(FloodAttackCoroutine());
        yield return new WaitForSeconds(1f); // kept short delay just in case

        yield return StartCoroutine(SonarAttackCoroutine());
        yield return new WaitForSeconds(sonarAttackDelay);

        yield return StartCoroutine(DestroyFloodCoroutine());

        isAttackSequenceActive = false;
    }

    private IEnumerator SpawnEnemiesCoroutine()
    {
        AnimationEventSpawn();
        yield return null;
        AnimationEventFalseSpawn();
    }

    private IEnumerator SprayAttackCoroutine(int times)
    {
        for (int i = 0; i < times; i++)
        {
            AnimationEventSpray();
            yield return new WaitForSeconds(sprayAttackDelay);
        }
        AnimationEventFalseSpray();
    }

    private IEnumerator FloodAttackCoroutine()
    {
        AnimationEventFlood();
        yield return null;
        AnimationEventFalseFlood();
    }

    private IEnumerator SonarAttackCoroutine()
    {
        AnimationEventSonar();
        yield return null;
        AnimationEventFalseSonar();
    }

    private IEnumerator DestroyFloodCoroutine()
    {
        yield return null;
    }

    public void FloodAttack()
    {
        // Flood attack function intentionally left empty
    }

    public void FloodAttackRemove()
    {
        // Flood attack remove function intentionally left empty
        Animator.SetBool("FloodAttack", false);
    }

    public void SonarAttack()
    {
        if (sonarProjectilePrefab == null || sonarSpawnPoint == null)
            return;

        LaunchSonarProjectile(sonarSpawnPoint.forward, sonarProjectileSpeed);
    }

    private void LaunchSonarProjectile(Vector3 direction, float speed)
    {
        if (sonarProjectilePrefab != null && sonarSpawnPoint != null)
        {
            GameObject projectile = Instantiate(sonarProjectilePrefab, sonarSpawnPoint.position, Quaternion.LookRotation(direction));
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
            return;

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
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.LookRotation(direction));
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
            if (projectileRb != null)
            {
                projectileRb.velocity = direction * speed;
            }
        }
    }

    public void MinionSpawnAttack()
    {
        Instantiate(minionPrefab, minionSpawnArea1.transform.position, Quaternion.identity);
        Instantiate(minionPrefab, minionSpawnArea2.transform.position, Quaternion.identity);
        Instantiate(minionPrefab, minionSpawnArea3.transform.position, Quaternion.identity);
    }
}
