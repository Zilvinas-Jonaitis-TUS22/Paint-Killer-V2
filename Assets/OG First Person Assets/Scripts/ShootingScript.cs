using StarterAssets;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class ShootingScript : MonoBehaviour
{
    public ParticleSystem ParticleSystem;
    private StarterAssetsInputs _inputs;
    public float shootTimeout;
    private float _shootTimeoutDelta;
    public Animator animatorController;
    private GrapplingScript _grapplingScript;
    public float InkAmount;
    public float MaxInkAmount = 4;
    public Slider InkAmountSlider;

    // Start is called before the first frame update
    void Start()
    {
        _inputs = GetComponent<StarterAssetsInputs>();
        _grapplingScript = GetComponent<GrapplingScript>();
        InkAmount = MaxInkAmount;
    }

    // Update is called once per frame
    void Update()
    {

        ShootCheck();
        InkAmountSlider.value = InkAmount;
    }

    public void ShootCheck()
    {
        if (!_grapplingScript._isGrappling)
        {

            if (_inputs.Shoot)
            {
                if (_shootTimeoutDelta <= 0 && InkAmount > 0)
                {
                    ParticleSystem.Play();
                    animatorController.SetTrigger("Shoot");
                    _shootTimeoutDelta = shootTimeout;
                    InkAmount -= 1;
                    Invoke("reload", 0.1f);
                }

                _inputs.Shoot = false;

            }

            if (_shootTimeoutDelta >= 0)
            {
                _shootTimeoutDelta -= Time.deltaTime;
            }
        } else
        {
            _inputs.Shoot = false;
        }
    }

    public void InkRefill(float amount)
    {
        InkAmount += amount;
        if (InkAmount > MaxInkAmount)
        {
            InkAmount = MaxInkAmount;
        }
    }

    public void reload()
    {
        animatorController.SetTrigger("Reload");
    }
}
