using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrafeTilt : MonoBehaviour
{
    [SerializeField] private Transform target; // The object to rotate (e.g., the camera)
    [SerializeField] private float tiltAmount = 5f; // Max tilt in degrees
    [SerializeField] private float tiltSpeed = 5f; // How quickly the tilt adjusts

    private float currentTilt = 0f;

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal"); // A/D or Left/Right keys
        float targetTilt = -horizontalInput * tiltAmount;

        // Smoothly interpolate tilt
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSpeed);

        // Preserve the current X and Y rotation while modifying only the Z
        target.localRotation = Quaternion.Euler(target.localRotation.eulerAngles.x, target.localRotation.eulerAngles.y, currentTilt);
    }
}