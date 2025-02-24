using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Currently Equipped")]
    public bool hands = true;
    public bool shotgun = false;
    public bool grapple = false;

    [Header("Item GameObjects")]
    public GameObject[] handsGO;        // Array for hands items
    public GameObject[] grappleGO;      // Array for grapple items
    public GameObject[] shotgunGO;      // Array for shotgun items

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (shotgun)
        {
            DeactivateAllItems();           // Deactivate all items
            SetItemsActive(shotgunGO);      // Activate all shotgun items
            hands = false;
            grapple = false;
        }
        else if (grapple)
        {
            DeactivateAllItems();           // Deactivate all items
            SetItemsActive(grappleGO);      // Activate all grapple items
            hands = false;
            shotgun = false;
        }
        else
        {
            DeactivateAllItems();           // Deactivate all items
            SetItemsActive(handsGO);        // Activate all hands items
            hands = true;
        }
    }

    // Method to deactivate all items in the arrays
    private void DeactivateAllItems()
    {
        SetItemsInactive(handsGO);        // Deactivate all hands items
        SetItemsInactive(grappleGO);      // Deactivate all grapple items
        SetItemsInactive(shotgunGO);      // Deactivate all shotgun items
    }

    // Method to deactivate all items in the array
    private void SetItemsInactive(GameObject[] itemArray)
    {
        foreach (var item in itemArray)
        {
            item.SetActive(false);
        }
    }

    // Method to activate all items in the array
    private void SetItemsActive(GameObject[] itemArray)
    {
        foreach (var item in itemArray)
        {
            item.SetActive(true);
        }
    }
}
