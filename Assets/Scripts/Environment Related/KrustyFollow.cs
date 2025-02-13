using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class KrustyFollow : MonoBehaviour
{
    public Transform targetLocation1;
    public Transform targetLocation2;
    public Transform player;
    private NavMeshAgent agent;
    private bool isFollowingPlayer = false;
    public float followDistance = 3f; // Distance to start following
    public float stopDistance = 2f;   // Distance to stop following
    private float originalSpeed;      // Store the original speed of the agent

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        originalSpeed = agent.speed; // Store the original speed
        agent.SetDestination(targetLocation1.position);
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (isFollowingPlayer)
            {
                if (distanceToPlayer <= stopDistance)
                {
                    // Stop moving by setting speed to 0
                    agent.speed = 0f;
                }
                else
                {
                    // Continue following the player
                    Vector3 playerPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
                    agent.SetDestination(playerPosition);

                    // Restore original speed when following the player
                    agent.speed = originalSpeed;
                }
            }
        }
    }

    public void FollowPlayer()
    {
        isFollowingPlayer = true;
    }

    public void UnFollowPlayer()
    {
        isFollowingPlayer = false;
    }

    public void OnDoorOpened()
    {
        if (!isFollowingPlayer)
        {
            MoveToFirstLocation();
        }
    }

    public void MoveToFirstLocation()
    {
        if (targetLocation2 != null)
        {
            Vector3 targetPosition = new Vector3(targetLocation2.position.x, transform.position.y, targetLocation2.position.z);
            agent.SetDestination(targetPosition);
        }
    }
}
