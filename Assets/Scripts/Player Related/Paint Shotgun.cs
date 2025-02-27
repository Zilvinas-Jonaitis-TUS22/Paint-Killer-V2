using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI; // For UI Image support
using TMPro; // For UI text support

public class PaintShotgun : MonoBehaviour
{
    [Header("Weapon Mechanics")]
    public bool isPlayerBusy = false;
    public bool reloading = false;
    public int ammoLoaded = 0;
    public int maximumAmmoLoaded = 7;
    public int reserveAmmo = 0;
    public Animator armsAnimator;

    [Header("UI Elements")]
    public TextMeshProUGUI ammoLoadedText;
    public TextMeshProUGUI reserveAmmoText;
    public Image[] crosshairImages; // Array to store crosshair images
    private Color defaultCrosshairColor = Color.white;
    private Color hitCrosshairColor = Color.red;
    public ShotgunUI shotgunUI;

    [Header("Slug Properties")]
    public Transform slugSpawnPoint;
    public float slugRange = 10f;
    public float pelletDamage = 1f;

    [Header("Scripts")]
    public CharacterController _controller;
    public StarterAssetsInputs _input;
    public Inventory inventory;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public ParticleSystem muzzleFlash2;

    

    void Start()
    {
        UpdateAmmoUI(); // Initialize UI on start
    }

    void Update()
    {
        UpdateCrosshairColor();

        if (inventory.shotgun)
        {
            if (!isPlayerBusy && !_input.sprint)
            {
                if (_input.shoot)
                {
                    ShootGun();
                }
                else
                {
                    armsAnimator.SetBool("Shooting", false);
                }
                if (_input.reload)
                {
                    ReloadGun();
                }
            }

            if (ammoLoaded == maximumAmmoLoaded || reserveAmmo == 0 || _input.sprint)
            {
                isPlayerBusy = false;
                reloading = false;
                armsAnimator.SetBool("Reloading", reloading);
            }

            armsAnimator.SetBool("Sprinting", _input.sprint);
        }
    }

    public void ExpandCrosshair()
    {
        shotgunUI.TriggerExpand();
    }

    public void NarrowCrosshair()
    {
        shotgunUI.TriggerNarrow();
    }
    public void UnNarrowCrosshair()
    {
        shotgunUI.TriggerUnNarrow();
    }

    private void UpdateCrosshairColor()
    {
        RaycastHit hit;
        bool isHittingTarget = Physics.Raycast(slugSpawnPoint.position, slugSpawnPoint.forward, out hit, slugRange, LayerMask.GetMask("Hurtable"));

        // Set crosshair color based on hit detection
        Color targetColor = isHittingTarget ? hitCrosshairColor : defaultCrosshairColor;

        foreach (Image crosshair in crosshairImages)
        {
            if (crosshair != null)
            {
                crosshair.color = targetColor;
            }
        }
    }

    public void ReloadGun()
    {
        if (!reloading && reserveAmmo > 0 && ammoLoaded < maximumAmmoLoaded)
        {
            reloading = true;
            armsAnimator.SetBool("Reloading", reloading);
        }
        UpdateAmmoUI(); // Update UI after reload
    }

    public void ShootGun()
    {
        if (ammoLoaded > 0)
        {
            reloading = false;
            armsAnimator.SetBool("Shooting", true);
            armsAnimator.SetBool("Reloading", reloading);
        }
        else
        {
            reloading = true;
            armsAnimator.SetBool("Reloading", reloading);
        }
        UpdateAmmoUI(); // Update UI after shooting
    }

    public void LoadShell()
    {
        if (reserveAmmo > 0 && ammoLoaded < maximumAmmoLoaded)
        {
            ammoLoaded++;
            reserveAmmo--;
            UpdateAmmoUI(); // Update UI after loading
        }
    }

    public void ShootShell()
    {
        if (ammoLoaded > 0)
        {
            ammoLoaded--;

            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
                muzzleFlash2.Play();
            }

            FireRaycast(slugSpawnPoint.position);

            Vector3[] offsets = new Vector3[]
            {
                new Vector3(0.15f, 0, 0.1f),
                new Vector3(-0.15f, 0, 0.1f),
                new Vector3(0.2f, 0, -0.1f),
                new Vector3(-0.2f, 0, -0.1f),
                new Vector3(0.12f, 0, -0.2f),
                new Vector3(-0.12f, 0, -0.2f)
            };

            foreach (Vector3 offset in offsets)
            {
                FireRaycast(slugSpawnPoint.position + offset);
            }

            UpdateAmmoUI(); // Update UI after shooting
        }
    }

    private void FireRaycast(Vector3 startPosition)
    {
        RaycastHit hit;
        Vector3 direction = slugSpawnPoint.forward;

        Debug.DrawRay(startPosition, direction * slugRange, Color.green, 1.0f);

        if (Physics.Raycast(startPosition, direction, out hit, slugRange, LayerMask.GetMask("Hurtable")))
        {
            float hitDistance = Vector3.Distance(slugSpawnPoint.position, hit.point);
            float damage = pelletDamage;

            if (hitDistance > slugRange * 0.66f)
            {
                damage *= 0.5f;
            }
            else if (hitDistance > slugRange * 0.33f)
            {
                damage *= 0.75f;
            }

            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                //Debug.Log(damage);
            }
        }
    }

    public void BecomeBusy()
    {
        isPlayerBusy = true;
    }

    public void BecomeUnBusy()
    {
        isPlayerBusy = false;
    }

    private void UpdateAmmoUI()
    {
        if (ammoLoadedText != null)
        {
            ammoLoadedText.text = $"{ammoLoaded} / {maximumAmmoLoaded}";
        }

        if (reserveAmmoText != null)
        {
            reserveAmmoText.text = $"{reserveAmmo}";
        }
    }
}
