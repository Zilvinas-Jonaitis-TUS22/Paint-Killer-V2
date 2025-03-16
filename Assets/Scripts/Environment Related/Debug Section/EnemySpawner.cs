using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject enemyPrefab;  // Prefab to spawn
    public Transform spawnPoint;    // Location where enemies will spawn
    public float spawnCooldown = 2f; // Time between spawns

    private bool playerOnPlatform = false;
    public float nextSpawnTime = 0f;

    void Update()
    {
        // Check if the player is on the platform and the cooldown has passed
        if (playerOnPlatform && Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnCooldown;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnPlatform = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnPlatform = false;
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab != null && spawnPoint != null)
        {
            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning("Missing enemyPrefab or spawnPoint in EnemySpawner script.");
        }
    }
}
