using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera m_Camera;
   
    void Start()
    {
        m_Camera = Camera.main;
    }


    void Update()
    {
        if (m_Camera != null)
        {
            transform.LookAt(m_Camera.transform);
            transform.Rotate(90,90,-270); // Flip to face forward
        }

    }
}
