using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorProximityOpen : MonoBehaviour
{
    [Header("Settings")]
    public Animator doorAnimator; // Assign the Animator component

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            doorAnimator.SetBool("DoorOpened", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            doorAnimator.SetBool("DoorOpened", false);
        }
    }
}
