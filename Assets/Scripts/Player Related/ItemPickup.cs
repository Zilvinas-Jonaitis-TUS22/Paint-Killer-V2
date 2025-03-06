using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private PlayerHealth PlayerHealth;

 
    void Start()
    {
        PlayerHealth = GetComponent<PlayerHealth>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Health"))
        {
            if (PlayerHealth.health< PlayerHealth.maxHealth)
            {
                PlayerHealth.HealHealth(1);
                Destroy(other.gameObject);
                Debug.Log(PlayerHealth.health);
            }
        }
    }

}
