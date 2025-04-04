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
    public int ammoLoaded = 8;
    public int maximumAmmoLoaded = 8;
    public int reserveAmmo = 32;
    public Animator armsAnimator;
    public bool isEquipped = true;

    [Header("UI Elements")]
    public TextMeshProUGUI ammoLoadedText;
    public TextMeshProUGUI reserveAmmoText;
    public Image[] crosshairImages; // Array to store crosshair images
    private Color defaultCrosshairColor = Color.white;
    public Color hitCrosshairColor = Color.red;
    public ShotgunUI shotgunUI;

    [Header("Slug Properties")]
    public Transform slugSpawnPoint;
    public float slugRange = 10f;
    public float pelletDamage = 3f;

    [Header("Scripts")]
    public CharacterController _controller;
    public StarterAssetsInputs _input;
    public Grapple GrappleScript;
    public Damagenumber damageNumberScript; // Reference to the Damagenumber script

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public ParticleSystem muzzleFlash2;

    [Header("Muzzle Flash Light")]
    public Light muzzleFlashLight;
    public float lightDuration = 0.25f;

    void Start()
    {
        UpdateAmmoUI(); // Initialize UI on start
    }

    void Update()
    {
        UpdateCrosshairColor();

        if (!isPlayerBusy)
        {
            if (_input.shoot && !GrappleScript.isEquipped)
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

        if (ammoLoaded == maximumAmmoLoaded || reserveAmmo == 0)
        {
            isPlayerBusy = false;
            reloading = false;
            armsAnimator.SetBool("Reloading", reloading);
        }

        armsAnimator.SetBool("Sprinting", _input.sprint);
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

            if (muzzleFlash != null) muzzleFlash.Play();
            if (muzzleFlash2 != null) muzzleFlash2.Play();

            // Enable light flash
            if (muzzleFlashLight != null)
            {
                muzzleFlashLight.enabled = true;
                StartCoroutine(DisableMuzzleFlashLight());
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
                if (damageNumberScript != null)
                {
                    damageNumberScript.ShowDamageNumber(damage, hit.point);
                }
            }

            BossHealth enemyBoss = hit.collider.GetComponent<BossHealth>();
            if (enemyBoss != null)
            {
                enemyBoss.TakeDamage(damage);
                if (damageNumberScript != null)
                {
                    damageNumberScript.ShowDamageNumber(damage, hit.point);
                }
            }
        }
    }

    private IEnumerator DisableMuzzleFlashLight()
    {
        yield return new WaitForSeconds(lightDuration);
        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.enabled = false;
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

    public void UpdateAmmoUI()
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
