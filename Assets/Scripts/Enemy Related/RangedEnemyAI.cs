using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class RangedEnemyAI : MonoBehaviour
{
    [Header("Detection & Patrolling")]
    public float detectionRadius = 10f;
    public float lostSightDuration = 5f;
    public float roamRadius = 5f;
    public float patrolWaitTime = 3f;
    public float stoppingDistance = 2f;

    [Header("Range Attack")]
    public float rangedAttackDistance = 5f;

    [Header("Pulsating Effect")]
    public float pulseSpeed = 2f;
    public float pulseIntensity = 0.05f;
    private Vector3 baseScale;
    private float randomOffset;

    [Header("References")]
    public Transform sprite;
    private NavMeshAgent agent;
    public RangedAttack rangedAttack;
    public Animator spriteAnimator;

    [Header("Shooting Audio")]
    public AudioSource shootAudioSource;

    private Transform player;  // Link to the player automatically
    private Vector3 originalPosition;
    private Vector3 lastKnownPosition;
    private bool playerInSight = false;
    private bool searchingForPlayer = false;

  


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        originalPosition = transform.position;

        // Automatically find the player by looking for the PlayerHealth script
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            player = playerHealth.transform; // Assign the player's transform
        }
        else
        {
            Debug.LogError("No object with PlayerHealth script found in the scene.");
        }

        StartCoroutine(PatrolRoutine());

        baseScale = sprite.localScale;
        randomOffset = Random.Range(0f, Mathf.PI * 2);
    }

    void PulseEffect()
    {
        float scaleOffset = Mathf.Sin(Time.time * pulseSpeed + randomOffset) * pulseIntensity;
        sprite.localScale = baseScale * (1 + scaleOffset);
    }

    void Update()
    {
        // Ensure the player is assigned
        if (player == null) return;

        PulseEffect();
        FacePlayer();

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            playerInSight = true;
            searchingForPlayer = false;
            StopAllCoroutines();
            lastKnownPosition = player.position;
            agent.SetDestination(player.position);

            // Use rangedAttackDistance instead of stoppingDistance
            if (distanceToPlayer <= rangedAttackDistance && (Time.time - rangedAttack.lastAttackTime >= rangedAttack.attackCooldown))
            {
                spriteAnimator.SetBool("Shooting", true);
            }
        }
        else if (playerInSight)
        {
            playerInSight = false;
            searchingForPlayer = true;
            StartCoroutine(LostPlayerRoutine());
        }
    }

    public void ShootProjectile()
    {
        if (player != null)
        {
            rangedAttack.FireProjectile(player.position); // Fire projectile at player
            shootAudioSource.Play();
        }
    }

    public void StopShooting()
    {
        spriteAnimator.SetBool("Shooting", false);
    }

    void FacePlayer()
    {
        if (sprite != null && player != null)
        {
            Vector3 direction = player.position - sprite.position;
            direction.y = 0; // Keep the sprite upright
            sprite.forward = direction.normalized;
        }
    }

    IEnumerator LostPlayerRoutine()
    {
        agent.SetDestination(lastKnownPosition);
        yield return new WaitForSeconds(lostSightDuration);
        if (!playerInSight)
        {
            searchingForPlayer = false;
            StartCoroutine(PatrolRoutine());
        }
    }

    IEnumerator PatrolRoutine()
    {
        while (!playerInSight && !searchingForPlayer)
        {
            Vector3 randomPoint = GetRandomPoint(originalPosition, roamRadius);
            agent.SetDestination(randomPoint);
            yield return new WaitForSeconds(patrolWaitTime);
        }
    }

    Vector3 GetRandomPoint(Vector3 center, float radius)
    {
        Vector3 randomPos = center + new Vector3(Random.Range(-radius, radius), 0, Random.Range(-radius, radius));
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(originalPosition, roamRadius);

        if (searchingForPlayer)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(lastKnownPosition, 0.5f);
        }

        // Debug sphere for ranged attack distance
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rangedAttackDistance);
    }
}
