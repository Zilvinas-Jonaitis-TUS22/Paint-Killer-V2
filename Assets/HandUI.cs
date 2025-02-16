using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    public Animator animator; // Public reference to the Animator
    private Inventory _inventory; // Reference to Inventory script

    void Start()
    {
        // Find the Inventory script in the scene
        _inventory = FindObjectOfType<Inventory>();
    }

    public void Update()
    {
        if (_inventory.hands == true)
        {
            animator.SetBool("Equipped", true);
        }
        else
        {
            animator.SetBool("Equipped", false);
        }
    }
}
