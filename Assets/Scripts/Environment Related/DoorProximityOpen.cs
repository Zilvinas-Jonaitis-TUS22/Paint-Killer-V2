using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorProximityOpen : MonoBehaviour
{
    [Header("Settings")]
    public Animator doorAnimator; // Assign the Animator component
    public KrustyFollow krusty;
    public bool opened = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && opened == false)
        {
            doorAnimator.SetBool("DoorOpened", true);
            opened = true;
            krusty.OnDoorOpened();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //doorAnimator.SetBool("DoorOpened", false);
        }
    }
}
