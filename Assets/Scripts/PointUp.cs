using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointUp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Always face global up (0, 1, 0) by aligning the up direction
        transform.up = Vector3.up;
    }
}
