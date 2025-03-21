using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for UI elements
using Cinemachine; // Required for Cinemachine

public class GrappleUI : MonoBehaviour
{
    public Animator animator; // Reference to the Animator
    private Grapple GrappleScript; // Reference to Grapple script

    [Header("Motion Lines UI")]
    public RectTransform motionLines; // Assign in Inspector
    public float scaleSpeed = 5f; // Speed of Lerp scaling
    public Vector3 scaleWhenGrappling;
    public Vector3 scaleWhenNotGrappling;

    [Header("Cinemachine FOV")]
    public CinemachineVirtualCamera cinemachineCam; // Assign in Inspector
    public float normalFOV = 85f;
    public float grappleFOV = 95f;
    public float fovLerpSpeed = 5f;

    void Start()
    {
        // Find the Grapple script in the scene
        GrappleScript = FindObjectOfType<Grapple>();

        // Ensure CinemachineCam is set
        if (cinemachineCam == null)
        {
            cinemachineCam = FindObjectOfType<CinemachineVirtualCamera>();
        }
    }

    void Update()
    {
        if (GrappleScript.isEquipped)
        {
            animator.SetBool("Equipped", true);
        }
        else
        {
            animator.SetBool("Equipped", false);
        }

        if (GrappleScript.grappable)
        {
            animator.SetBool("Grappable", true);
        }
        else
        {
            animator.SetBool("Grappable", false);
        }

        // Smoothly scale motion lines based on grappling state
        if (motionLines != null)
        {
            Vector3 targetScale = GrappleScript.isGrappling ? scaleWhenGrappling : scaleWhenNotGrappling;
            motionLines.localScale = Vector3.Lerp(motionLines.localScale, targetScale, Time.deltaTime * scaleSpeed);
        }

        // Smoothly Lerp FOV when grappling
        if (cinemachineCam != null)
        {
            float targetFOV = GrappleScript.isGrappling ? grappleFOV : normalFOV;
            cinemachineCam.m_Lens.FieldOfView = Mathf.Lerp(cinemachineCam.m_Lens.FieldOfView, targetFOV, Time.deltaTime * fovLerpSpeed);
        }
    }
}
