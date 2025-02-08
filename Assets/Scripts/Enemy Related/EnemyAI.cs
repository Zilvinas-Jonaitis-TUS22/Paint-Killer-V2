using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public Transform sprite; // Reference to the child sprite object
    public float detectionRadius = 10f;
    public float lostSightDuration = 5f;
    public float roamRadius = 5f;
    public float patrolWaitTime = 3f;
    public float stoppingDistance = 2f;

    private NavMeshAgent agent;
    private Vector3 originalPosition;
    private Vector3 lastKnownPosition;
    private bool playerInSight = false;
    private bool searchingForPlayer = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        originalPosition = transform.position;
        StartCoroutine(PatrolRoutine());
    }

    void Update()
    {
        FacePlayer(); // Make the sprite face the player

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            playerInSight = true;
            searchingForPlayer = false;
            StopAllCoroutines();
            lastKnownPosition = player.position;
            agent.SetDestination(player.position);
        }
        else if (playerInSight)
        {
            playerInSight = false;
            searchingForPlayer = true;
            StartCoroutine(LostPlayerRoutine());
        }
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
    }
}
