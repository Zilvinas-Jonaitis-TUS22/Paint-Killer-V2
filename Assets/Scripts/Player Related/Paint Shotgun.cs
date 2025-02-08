using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class PaintShotgun : MonoBehaviour
{
    [Header("Weapon Mechanics")]
    public bool isPlayerBusy = false;
    public bool reloading = false;
    public int ammoLoaded = 0;
    public int reserveAmmo = 0;
    public Animator armsAnimator;

    [Header("Slug Properties")]
    public Transform slugSpawnPoint;
    //public GameObject shell;
    public float slugRange = 10f;

    [Header("Scripts")]
    public CharacterController _controller;
    public StarterAssetsInputs _input;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public ParticleSystem muzzleFlash2;

    void Update()
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

        if (ammoLoaded == 6 || reserveAmmo == 0 || _input.sprint)
        {
            isPlayerBusy = false;
            reloading = false;
            armsAnimator.SetBool("Reloading", reloading);
        }
    }

    public void ReloadGun()
    {
        
        if (!reloading && reserveAmmo > 0 && ammoLoaded < 7)
        {
            reloading = true;
            armsAnimator.SetBool("Reloading" ,reloading);
            //reload gun
        }
        else if (ammoLoaded == 6)
        {
            //max ammo in gun
        }
        else if (reserveAmmo == 0)
        {
            //out of reserve ammo
        }
        else
        {
            //Cannot reload right now
        }
    }
    public void ShootGun()
    {
        Debug.Log(_input.shoot);
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
        
    }

    public void LoadShell()
    {
        ammoLoaded ++;
        reserveAmmo --;
}

    public void ShootShell()
    {
        if (ammoLoaded > 0)
        {
            ammoLoaded--;

            // Play muzzle flash effect
            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
                muzzleFlash2.Play();
            }

            // Fire central ray
            FireRaycast(slugSpawnPoint.position);

            // Define hexagonal offsets (relative positions)
            Vector3[] offsets = new Vector3[]
            {
            new Vector3(0.15f, 0, 0.1f),   // Top Right
            new Vector3(-0.15f, 0, 0.1f),  // Top Left
            new Vector3(0.2f, 0, -0.1f),   // Mid-Right
            new Vector3(-0.2f, 0, -0.1f),  // Mid-Left
            new Vector3(0.12f, 0, -0.2f),  // Bottom Right
            new Vector3(-0.12f, 0, -0.2f)  // Bottom Left
            };

            // Fire offset rays
            foreach (Vector3 offset in offsets)
            {
                FireRaycast(slugSpawnPoint.position + offset);
            }
        }
    }

    // Function to handle individual raycast logic
    private void FireRaycast(Vector3 startPosition)
    {
        RaycastHit hit;
        Vector3 direction = slugSpawnPoint.forward;

        Debug.DrawRay(startPosition, direction * slugRange, Color.green, 1.0f);

        if (Physics.Raycast(startPosition, direction, out hit, slugRange, LayerMask.GetMask("Hurtable")))
        {
            //Debug.Log("Success: Hit " + hit.collider.name);

            // Check if the hit object has EnemyHealth component
            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(1); // Deal 1 damage per ray hit
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
}
