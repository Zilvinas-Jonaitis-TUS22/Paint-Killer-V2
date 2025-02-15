using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunUI : MonoBehaviour
{
    public Animator animator; // Public reference to the Animator
    private Inventory _inventory; // Reference to Inventory script

    void Start()
    {
        // Find the Inventory script in the scene
        _inventory = FindObjectOfType<Inventory>();
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

    public void Update()
    {
        if (_inventory.shotgun == true)
        {
            animator.SetBool("Equipped", true);
        }
        else
        {
            animator.SetBool("Equipped", false);
        }
    }
}
