using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Hands")]
    public GameObject handsGO;
    public GameObject handsGO2;
    public bool hands = true;

    [Header("Shotgun")]
    public GameObject shotgunGO;
    public GameObject shotgunGO2;
    public bool shotgun = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (shotgun)
        {
            handsGO.SetActive(false);
            handsGO2.SetActive(false);
            shotgunGO.SetActive(true);
            shotgunGO2.SetActive(true);
            hands = false;
        }
        else
        {
            handsGO.SetActive(true);
            handsGO2.SetActive(true);
            shotgunGO.SetActive(false);
            shotgunGO2.SetActive(false);
            hands = true;
        }
    }
}
