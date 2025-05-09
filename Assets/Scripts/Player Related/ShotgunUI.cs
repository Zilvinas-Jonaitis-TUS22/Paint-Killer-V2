using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunUI : MonoBehaviour
{
    public Animator animator; // Public reference to the Animator
    private Grapple GrappleScript; // Reference to Inventory script

    void Start()
    {
        // Find the Inventory script in the scene
        GrappleScript = FindObjectOfType<Grapple>();
    }

    // Function to trigger the "Expand" parameter in the Animator
    public void TriggerExpand()
    {
        if (animator != null)
        {
            animator.SetTrigger("Expand");
        }
        else
        {
            Debug.LogWarning("Animator reference is missing in ShotgunUI.");
        }
    }

    public void TriggerNarrow()
    {
        if (animator != null)
        {
            animator.SetBool("Narrow", true);
        }
        else
        {
            Debug.LogWarning("Animator reference is missing in ShotgunUI.");
        }
    }

    public void TriggerUnNarrow()
    {
        if (animator != null)
        {
            animator.SetBool("Narrow", false);
        }
        else
        {
            Debug.LogWarning("Animator reference is missing in ShotgunUI.");
        }
    }

    public void Update()
    {
        if (!GrappleScript.isEquipped)
        {
            animator.SetBool("Equipped", true);
        }
        else
        {
            animator.SetBool("Equipped", false);
        }
    }
}
