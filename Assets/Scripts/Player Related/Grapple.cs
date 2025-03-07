using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets; // For input support

public class Grapple : MonoBehaviour
{
    [Header("Grapple Mechanics")]
    public bool isGrappling = false;
    public float grappleRange = 15f;
    public float grappleSpeed = 10f;

    [Header("Grapple Timing")]
    public float maxGrappleTime = 3f; // Grapple effect lasts for 3 seconds
    private float grappleTimer = 0f;

    [Header("Grapple Cooldown")]
    public float grappleCooldownDuration = 3f; // Cooldown duration after a grapple ends
    private float grappleCooldownTimer = 0f;

    [Header("Propulsion Delay Settings")]
    // Set these values in the Inspector to define the minimum and maximum wait times.
    public float minPropulsionDelay = 1f;
    public float maxPropulsionDelay = 2f;
    private float propulsionDelayTimer = 0f;
    private float propulsionDelayDuration = 0f;

    [Header("Grapple Properties")]
    public Transform grappleOrigin; // Where the grapple ray is cast from
    public Transform grappleLineOrigin;

    [Header("Effects")]
    public LineRenderer grappleLine;           // Visualizes the grapple rope
    public ParticleSystem grappleImpactEffect; // Optional impact effect at the grapple point

    [Header("Layers")]
    // Set this to the "Grappable" layer in the Inspector or via code using LayerMask.GetMask("Grappable")
    public LayerMask grappableLayer;

    [Header("Scripts")]
    public CharacterController _controller;    // To move the player
    public StarterAssetsInputs _input;           // To read player input (ensure a "grapple" bool is added to your input system)

    [Header("Movement Control")]
    // Reference to your FirstPersonController that disables movement when grappled
    public FirstPersonController firstPersonController;

    // Internal state
    private Vector3 grapplePoint;

    void Start()
    {
        // Optionally, if firstPersonController is not set in the Inspector, find it automatically.
        if (firstPersonController == null)
        {
            firstPersonController = FindObjectOfType<FirstPersonController>();
        }

        // Ensure the LineRenderer is initially disabled
        if (grappleLine != null)
        {
            grappleLine.positionCount = 0;
        }
    }

    void Update()
    {
        // Always draw a debug ray to visualize the grapple direction and range in cyan
        Debug.DrawRay(grappleOrigin.position, grappleOrigin.forward * grappleRange, Color.cyan);

        // If not grappling, update the cooldown timer
        if (!isGrappling)
        {
            grappleCooldownTimer += Time.deltaTime;
        }

        // Check if the grapple input is triggered, we're not already grappling, and cooldown has elapsed
        if (_input.grapple && !isGrappling && CanGrapple())
        {
            AttemptGrapple();
        }

        // If grappling, update the effect, wait for the propulsion delay, then move the player toward the grapple point.
        if (isGrappling)
        {
            // Increment grapple timer and end grapple if max time is reached.
            grappleTimer += Time.deltaTime;
            if (grappleTimer >= maxGrappleTime)
            {
                Debug.Log("Max grapple time reached. Ending grapple.");
                EndGrapple();
                return;
            }

            UpdateGrappleLine();

            // Wait for the propulsion delay to elapse before propelling.
            if (propulsionDelayTimer < propulsionDelayDuration)
            {
                propulsionDelayTimer += Time.deltaTime;
            }
            else
            {
                PullPlayerTowardsGrapple();
            }

            // Draw a red debug line for the active grapple connection.
            Debug.DrawLine(grappleOrigin.position, grapplePoint, Color.red);

            // Stop grappling if close enough to the target.
            if (Vector3.Distance(transform.position, grapplePoint) < 1f)
            {
                EndGrapple();
            }
        }
    }

    // Determines if the grapple is allowed based on the cooldown timer.
    private bool CanGrapple()
    {
        return grappleCooldownTimer >= grappleCooldownDuration;
    }

    // Try to start grappling by raycasting forward from the grapple origin,
    // only hitting objects on the "Grappable" layer.
    void AttemptGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(grappleOrigin.position, grappleOrigin.forward, out hit, grappleRange, grappableLayer))
        {
            Debug.Log("Grapple target hit at: " + hit.point);
            // Begin grapple
            isGrappling = true;
            grapplePoint = hit.point;
            grappleTimer = 0f;           // Reset active grapple timer
            propulsionDelayTimer = 0f;   // Reset propulsion delay timer
            grappleCooldownTimer = 0f;   // Reset cooldown timer

            // Calculate the propulsion delay based on the distance from the player to the grapple point.
            float distance = Vector3.Distance(transform.position, grapplePoint);
            // Clamp the ratio between 0 and 1, then lerp between minPropulsionDelay and maxPropulsionDelay.
            propulsionDelayDuration = Mathf.Lerp(minPropulsionDelay, maxPropulsionDelay, Mathf.Clamp01(distance / grappleRange));

            // Disable movement by setting isGrappled to true.
            if (firstPersonController != null)
            {
                firstPersonController.isGrappled = true;
            }

            // Play impact effect at the grapple point if assigned.
            if (grappleImpactEffect != null)
            {
                grappleImpactEffect.transform.position = grapplePoint;
                grappleImpactEffect.Play();
            }

            // Initialize the LineRenderer to draw the grapple rope.
            if (grappleLine != null)
            {
                grappleLine.positionCount = 2;
                grappleLine.SetPosition(0, grappleOrigin.position);
                grappleLine.SetPosition(1, grapplePoint);
            }
        }
        else
        {
            Debug.Log("No grapple target hit.");
        }
    }

    // Updates the positions on the LineRenderer every frame.
    void UpdateGrappleLine()
    {
        if (grappleLine != null)
        {
            grappleLine.SetPosition(0, grappleLineOrigin.position);
            grappleLine.SetPosition(1, grapplePoint);
        }
    }

    // Moves the player toward the grapple point.
    void PullPlayerTowardsGrapple()
    {
        Vector3 direction = (grapplePoint - transform.position).normalized;
        _controller.Move(direction * grappleSpeed * Time.deltaTime);
    }

    // Ends the grappling effect, clears the visual line, and re-enables movement.
    void EndGrapple()
    {
        isGrappling = false;
        grappleTimer = 0f; // Reset the active grapple timer

        // Re-enable movement by setting isGrappled to false.
        if (firstPersonController != null)
        {
            firstPersonController.isGrappled = false;
        }

        if (grappleLine != null)
        {
            grappleLine.positionCount = 0;
        }
        Debug.Log("Grapple ended.");
    }
}
