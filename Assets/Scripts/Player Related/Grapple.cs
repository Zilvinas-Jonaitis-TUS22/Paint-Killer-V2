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
    public bool grappable = false; // New bool to track if a surface is within range

    [Header("Grapple Timing")]
    public float maxGrappleTime = 3f;
    public float grappleTimer = 0f;

    [Header("Grapple Cooldown")]
    public float grappleCooldownDuration = 3f;
    private float grappleCooldownTimer = 0f;

    [Header("Propulsion Delay Settings")]
    public float minPropulsionDelay = 1f;
    public float maxPropulsionDelay = 2f;
    private float propulsionDelayTimer = 0f;
    private float propulsionDelayDuration = 0f;

    [Header("Grapple Properties")]
    public Transform grappleOrigin;
    public Transform grappleLineOrigin;

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

    void Start()
    {
        if (firstPersonController == null)
        {
            firstPersonController = FindObjectOfType<FirstPersonController>();
        }

        if (grappleLine != null)
        {
            grappleLine.positionCount = 0;
        }
    }

    void Update()
    {
        if (_input.grapple)
        {
            isEquipped = true;
        }
        else
        {
            isEquipped = false;
        }
        // Constantly check if there's a valid grapple surface
        CheckForGrappleSurface();

        Debug.DrawRay(grappleOrigin.position, grappleOrigin.forward * grappleRange, grappable ? Color.green : Color.cyan);

        if (!isGrappling)
        {
            grappleCooldownTimer += Time.deltaTime;
        }

        if (_input.grapple && !isGrappling && CanGrapple() && isEquipped && _input.shoot)
        {
            AttemptGrapple();
        }

        if (isGrappling)
        {
            grappleTimer += Time.deltaTime;
            if (grappleTimer >= maxGrappleTime)
            {
                EndGrapple();
                return;
            }

            UpdateGrappleLine();

            if (propulsionDelayTimer < propulsionDelayDuration)
            {
                propulsionDelayTimer += Time.deltaTime;
            }
            else
            {
                PullPlayerTowardsGrapple();
            }

            Debug.DrawLine(grappleOrigin.position, grapplePoint, Color.red);

            if (Vector3.Distance(transform.position, grapplePoint) < 1f)
            {
                EndGrapple();
            }
        }
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
                grappleLine.SetPosition(0, grappleOrigin.position);
                grappleLine.SetPosition(1, grapplePoint);
            }
        }
    }

    void UpdateGrappleLine()
    {
        if (grappleLine != null)
        {
            grappleLine.SetPosition(0, grappleLineOrigin.position);
            grappleLine.SetPosition(1, grapplePoint);
        }
    }

    void PullPlayerTowardsGrapple()
    {
        Vector3 direction = (grapplePoint - transform.position).normalized;
        _controller.Move(direction * grappleSpeed * Time.deltaTime);
    }

    void EndGrapple()
    {
        isGrappling = false;
        grappleTimer = 0f;

        if (firstPersonController != null)
        {
            firstPersonController.isGrappled = false;
        }

        if (grappleLine != null)
        {
            grappleLine.positionCount = 0;
        }
    }

    void CheckForGrappleSurface()
    {
        // Cast a ray to check if there's a valid grapple surface
        grappable = Physics.Raycast(grappleOrigin.position, grappleOrigin.forward, grappleRange, grappableLayer);
    }
}
