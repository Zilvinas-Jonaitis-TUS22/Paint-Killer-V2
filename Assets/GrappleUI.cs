using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for UI elements

public class GrappleUI : MonoBehaviour
{
    public Animator animator; // Public reference to the Animator
    private Grapple GrappleScript; // Reference to Grapple script

    [Header("Motion Lines UI")]
    public RectTransform motionLines; // Assign this in the Inspector
    public float scaleSpeed = 5f; // Speed of Lerp scaling
    public Vector3 scaleWhenGrappling; 
    public Vector3 scaleWhenNotGrappling; 

    void Start()
    {
        // Find the Grapple script in the scene
        GrappleScript = FindObjectOfType<Grapple>();
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
    }
}
