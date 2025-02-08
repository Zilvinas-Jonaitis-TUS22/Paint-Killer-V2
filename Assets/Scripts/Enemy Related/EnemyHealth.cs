using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 7;

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(gameObject.name + " took damage! HP: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has been destroyed!");
        Destroy(gameObject); // Destroy the enemy
    }
}
