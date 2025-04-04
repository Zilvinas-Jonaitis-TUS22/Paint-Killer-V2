using UnityEngine;

public class DropOnDeath : MonoBehaviour
{
    [Header("Pickup Prefabs")]
    public GameObject pickupOption1;
    public GameObject pickupOption2;

    [Header("Pickup Options")]
    public bool spawnOption1;
    public bool spawnOption2;

    [Header("Drop Chance Settings")]
    public bool useDropChance = false;
    [Range(0f, 100f)] public float dropChance = 100f;

    [Header("Spawn Settings")]
    public Vector3 spawnOffset = Vector3.zero;

    private EnemyHealth enemyHealth;
    private bool hasDropped = false;

    void Start()
    {
        // Get reference to EnemyHealth on the same GameObject
        enemyHealth = GetComponent<EnemyHealth>();
        if (enemyHealth == null)
        {
            //Debug.LogError("EnemyHealth script not found on " + gameObject.name);
        }
    }

    void Update()
    {
        if (!hasDropped && enemyHealth != null && IsEnemyDead())
        {
            //Debug.Log($"Enemy '{gameObject.name}' is dead. Attempting to drop pickup.");
            TryDropPickup();
            hasDropped = true;
        }
    }

    bool IsEnemyDead()
    {
        // Detect when enemy is dead
        if (enemyHealth.currentHealth <= 0)
        {
            //Debug.Log($"isDead detected as true for enemy: {gameObject.name}");
            return true;
        }
        return false;
    }

    void TryDropPickup()
    {
        if (useDropChance)
        {
            float roll = Random.Range(0f, 100f);
            //Debug.Log($"Drop chance roll: {roll} (Needed: <= {dropChance})");

            if (roll > dropChance)
            {
                //Debug.Log($"No item dropped for enemy: {gameObject.name} (Roll: {roll}%)");
                return;
            }
        }

        DropPickup();
    }

    void DropPickup()
    {
        if (hasDropped) return;  // Prevent duplicate drops

        GameObject prefabToSpawn = null;

        if (spawnOption1 && pickupOption1 != null)
        {
            prefabToSpawn = pickupOption1;
        }
        else if (spawnOption2 && pickupOption2 != null)
        {
            prefabToSpawn = pickupOption2;
        }
        else
        {
            //Debug.LogWarning("No pickup option selected or prefab missing.");
            return;
        }

        // Log and instantiate the selected pickup
        //Debug.Log($"Spawning pickup: {prefabToSpawn.name} at {transform.position + spawnOffset}");
        Instantiate(prefabToSpawn, transform.position + spawnOffset, Quaternion.identity);
        hasDropped = true;
    }

    void OnDestroy()
    {
        // Drop pickup if it hasn't been dropped already
        if (!hasDropped)
        {
            //Debug.Log($"Enemy '{gameObject.name}' destroyed. Checking drop chance on destruction.");
            TryDropPickup();
        }
    }
}
