using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private PlayerHealth playerHealth;
    public PaintShotgun playerShotgun;
    public int ammoAmount;
 
    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
       

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Health"))
        {
            if (playerHealth.health < playerHealth.maxHealth)
            {
                playerHealth.HealHealth(1);
                Destroy(other.gameObject);
                //Debug.Log(playerHealth.health);
            }
        }
        if (other.CompareTag("Ammo"))
        {

            Debug.Log("picked up ammo");
            playerShotgun.reserveAmmo += ammoAmount;
                playerShotgun.UpdateAmmoUI();
                Destroy(other.gameObject);
           
            
        }
    }

}
