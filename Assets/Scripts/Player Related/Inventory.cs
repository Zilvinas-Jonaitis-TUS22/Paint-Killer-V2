using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Currently Equipped")]
    public bool hands = true;
    public bool shotgun = false;
    public bool grapple = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (shotgun)
        {
            hands = false;
            grapple = false;
        }
        else if (grapple)
        {
            hands = false;
            shotgun = false;
        }
        else
        {
            hands = true;
        }
    }
}
