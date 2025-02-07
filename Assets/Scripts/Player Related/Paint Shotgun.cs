using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class PaintShotgun : MonoBehaviour
{
    public bool isPlayerBusy = false;
    public bool reloading = false;
    public int ammoLoaded = 0;
    public int reserveAmmo = 0;

    public Animator armsAnimator;

    public CharacterController _controller;
    public StarterAssetsInputs _input;

        void Update()
    {
        if (!isPlayerBusy && !_input.sprint)
        {
            if (_input.shoot)
            {
                ShootGun();
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
        if (ammoLoaded > 0)
        {
            reloading = false;
            armsAnimator.SetTrigger("Shooting");
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
        ammoLoaded--;
        //fire shell here
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
