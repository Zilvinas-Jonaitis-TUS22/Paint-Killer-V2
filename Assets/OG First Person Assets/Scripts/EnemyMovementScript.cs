using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementScript : MonoBehaviour
{
    private GameObject _player;
    private NavMeshAgent _agent;
    public float enemySightDistance;
    private float _enemyDistance;

    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        _enemyDistance = Vector3.Distance(transform.position, _player.transform.position);
        if (_enemyDistance <= enemySightDistance)
        {
            _agent.SetDestination(_player.transform.position);
        }
    }
}
