using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class GrapplingScript : MonoBehaviour
{
    private CharacterController _Controller;
    private StarterAssetsInputs _inputs;
    private Vector3 _grappleTarget;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LayerMask grappleHit;
    public LineRenderer lineRenderer;
    public float grappleSpeed;
    public float grappleSpeedBuildUp;

    public float maxGrappleDistance;
    public float grappleDelayTime;

    public float grappleCd;
    private float _grappleCdTimer;

    public bool _isGrappling;

    public Animator animatorController;
    public RawImage crossHair;


    // Start is called before the first frame update
    void Start()
    {
        _Controller = GetComponent<CharacterController>();
        _inputs = GetComponent<StarterAssetsInputs>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_inputs.grapple)
        {
            grappleStart();
            _inputs.grapple = false;
        }

        if (_grappleCdTimer >= 0 )
        {
            _grappleCdTimer -= Time.deltaTime;
        }

        if(_isGrappling && _grappleTarget != null)
        {
            _Controller.Move((_grappleTarget - transform.position).normalized  * (grappleSpeed + grappleSpeedBuildUp) * Time.deltaTime);
            grappleSpeedBuildUp += 5 * Time.deltaTime;
            if ((_grappleTarget - transform.position).magnitude < 3)
            {
                StopGrapple();
                grappleSpeedBuildUp = 0;
            }
        }

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            crossHair.color = Color.blue;
        } else
        {
            crossHair.color = Color.white;

        }

    }

    private void LateUpdate()
    {

            lineRenderer.SetPosition(0, gunTip.position);

    }

    public void grappleStart()
    {
        if (_grappleCdTimer < 0)
        {
            RaycastHit hit;
            RaycastHit ignoreHit;

            if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
            {
                if(!Physics.Raycast(cam.position, cam.forward, out ignoreHit, Vector3.Distance(cam.position, hit.transform.position), grappleHit))
                {
                    _grappleTarget = hit.transform.position;
                    Invoke(nameof(ExecuteGrapple), grappleDelayTime);
                }
                else
                {
                    Invoke(nameof(StopGrapple), grappleDelayTime);
                }
            }
            else
            {
                _grappleTarget = cam.position + cam.forward * maxGrappleDistance;
                Invoke(nameof(StopGrapple), grappleDelayTime);

            }

            animatorController.SetBool("isGrappling", true);
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(1, _grappleTarget);
        }


    }

    public void ExecuteGrapple()
    {
        //GetComponent<ShootingScript>().InkAmount -= 1;
        _isGrappling = true;

    }

    public void StopGrapple()
    {
        _isGrappling = false;
        _grappleCdTimer = grappleCd;
        lineRenderer.enabled = false;
        animatorController.SetBool("isGrappling", false);

    }
}
