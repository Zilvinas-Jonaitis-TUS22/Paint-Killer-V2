using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets; // For input support

public class Grapple : MonoBehaviour
{
    [Header("Grapple Mechanics")]
    public bool isEquipped = false;
    public bool isGrappling = false;
    public float grappleRange = 15f;
    public float grappleSpeed = 10f;
    public float grappleLineSpeed = 10f;
    public bool grappable = false;

    [Header("Grapple Timing")]
    public float maxGrappleTime = 3f;
    private float grappleTimer = 0f;

    [Header("Grapple Cooldown")]
    public float grappleCooldownDuration = 3f;
    private float grappleCooldownTimer = 0f;

    [Header("Propulsion Delay Settings")]
    public float minPropulsionDelay = 1f;
    public float maxPropulsionDelay = 2f;
    private float propulsionDelayTimer = 0f;
    private float propulsionDelayDuration = 0f;
    private bool grapplePullTriggered = false; // Flag to track if momentum has started

    [Header("Grapple Properties")]
    public Transform grappleOrigin;
    public Transform grappleLineOrigin;
    public Animator armsAnimator;
    public GameObject grappleHead; // NEW: Reference to the grapple head model

    [Header("Effects")]
    public LineRenderer grappleLine;
    public ParticleSystem grappleImpactEffect;

    [Header("Layers")]
    public LayerMask grappableLayer;

    [Header("Scripts")]
    public CharacterController _controller;
    public StarterAssetsInputs _input;
    public FirstPersonController firstPersonController;

    private Vector3 grapplePoint;
    private Vector3 lineEndPosition;  // Variable to track the lerping endpoint of the line

    void Start()
    {
        if (firstPersonController == null)
        {
            firstPersonController = FindObjectOfType<FirstPersonController>();
        }

        if (grappleLine != null)
        {
            grappleLine.positionCount = 2;
            grappleLine.SetPosition(0, grappleOrigin.position);  // Initialize first point at gun tip
        }

        if (grappleHead != null)
        {
            grappleHead.SetActive(true); // Ensure the grapple head is visible at start
        }
    }

    void Update()
    {
        // Handle grapple equipment
        if (_input.grapple)
        {
            isEquipped = true;
            armsAnimator.SetBool("GrappleEquipped", true);
        }
        else
        {
            isEquipped = false;
            armsAnimator.SetBool("GrappleEquipped", false);
        }

        // Constantly check if there's a valid grapple surface
        CheckForGrappleSurface();

        // Update grapple cooldown if not grappling
        if (!isGrappling)
        {
            grappleCooldownTimer += Time.deltaTime;
        }

        // If the player presses the grapple button and conditions are right, start grappling
        if (_input.grapple && !isGrappling && CanGrapple() && isEquipped && _input.shoot)
        {
            AttemptGrapple();
            armsAnimator.SetBool("Grappling", true);
        }

        // If player is currently grappling
        if (isGrappling)
        {
            grappleTimer += Time.deltaTime;
            if (grappleTimer >= maxGrappleTime)
            {
                EndGrapple();
                return;
            }

            UpdateGrappleLine();

            // Wait for propulsion delay before applying momentum
            if (propulsionDelayTimer < propulsionDelayDuration)
            {
                propulsionDelayTimer += Time.deltaTime;
            }
            else if (grapplePullTriggered) // Only move when GrapplePull() is called
            {
                PullPlayerTowardsGrapple();
            }

            Debug.DrawLine(grappleOrigin.position, grapplePoint, Color.red);

            // End grapple when player is close enough to the grapple point
            if (Vector3.Distance(transform.position, grapplePoint) < 1f)
            {
                EndGrapple();
            }

            // If the player releases the grapple input but is being pulled, do nothing
            if (!_input.grapple && grapplePullTriggered && Vector3.Distance(transform.position, grapplePoint) > 1f)
            {
                return;
            }
        }

        // Check if the player has released the grapple button, then immediately end grappling if no longer being pulled
        if (!_input.grapple && !grapplePullTriggered && isGrappling)
        {
            EndGrapple();
        }
    }

    public void GrapplingOff()
    {
        armsAnimator.SetBool("Grappling", false);
    }

    private bool CanGrapple()
    {
        return grappleCooldownTimer >= grappleCooldownDuration;
    }

    void AttemptGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(grappleOrigin.position, grappleOrigin.forward, out hit, grappleRange, grappableLayer))
        {
            isGrappling = true;
            grapplePoint = hit.point;
            grappleTimer = 0f;
            propulsionDelayTimer = 0f;
            grappleCooldownTimer = 0f;
            grapplePullTriggered = false; // Reset flag until animation triggers it

            float distance = Vector3.Distance(transform.position, grapplePoint);
            propulsionDelayDuration = Mathf.Lerp(minPropulsionDelay, maxPropulsionDelay, Mathf.Clamp01(distance / grappleRange));

            if (firstPersonController != null)
            {
                firstPersonController.isGrappled = true;
            }

            if (grappleImpactEffect != null)
            {
                grappleImpactEffect.transform.position = grapplePoint;
                grappleImpactEffect.Play();
            }

            if (grappleLine != null)
            {
                grappleLine.positionCount = 2;
                grappleLine.SetPosition(0, grappleOrigin.position); // Start at the gun tip
                lineEndPosition = grappleOrigin.position; // Start at the gun tip as well
            }

            if (grappleHead != null)
            {
                grappleHead.SetActive(false); // Hide the grapple head when the line is active
            }
        }
    }

    void UpdateGrappleLine()
    {
        // Smoothly move the line's second point towards the grapple point
        if (grappleLine != null)
        {
            lineEndPosition = Vector3.MoveTowards(lineEndPosition, grapplePoint, grappleLineSpeed * Time.deltaTime);
            grappleLine.SetPosition(0, grappleLineOrigin.position); // Gun tip stays at the origin
            grappleLine.SetPosition(1, lineEndPosition); // Line end moves towards the grapple point
        }
    }

    void PullPlayerTowardsGrapple()
    {
        Vector3 direction = (grapplePoint - transform.position).normalized;
        _controller.Move(direction * grappleSpeed * Time.deltaTime);
    }

    void EndGrapple()
    {
        // Reset the grapple state
        isGrappling = false;
        grappleTimer = 0f;
        grapplePullTriggered = false; // Reset flag when grapple ends

        if (firstPersonController != null)
        {
            firstPersonController.isGrappled = false;
        }

        if (grappleLine != null)
        {
            grappleLine.positionCount = 0;
        }

        if (grappleHead != null)
        {
            grappleHead.SetActive(true); // Show the grapple head again when the grapple ends
        }

        armsAnimator.SetBool("Grappling", false);
    }

    void CheckForGrappleSurface()
    {
        grappable = Physics.Raycast(grappleOrigin.position, grappleOrigin.forward, grappleRange, grappableLayer);
    }

    public void GrapplePull()
    {
        grapplePullTriggered = true; // Called via animation event
    }
}
