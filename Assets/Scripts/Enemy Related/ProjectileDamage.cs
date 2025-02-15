using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // This method is called when the projectile collides with something
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object we collided with is on the "Player" layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // Find the PlayerHealth component in the scene
            PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1); // Deal 1 damage to the player
            }
            else
            {
                Debug.LogError("PlayerHealth component not found in the scene.");
            }
        }

        // Destroy the projectile (make it disappear) after it collides with anything
        Destroy(gameObject);
    }
}
