using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectPaintShotgun : MonoBehaviour
{
    public bool collected;
    public Inventory playerInventory;
    public KrustyFollow krusty;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger has the "Player" layer
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collected = true;
            krusty.FollowPlayer();
            if (playerInventory != null)
            {
                playerInventory.shotgun = true;
            }
            // Optionally, deactivate the object after collection
            gameObject.SetActive(false);
        }
    }
}