using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class PaintShotgun : MonoBehaviour
{
    public bool isPlayerBusy = false;
    public int ammoLoaded = 0;
    public int reserveAmmo = 0;

    private CharacterController _controller;
    private StarterAssetsInputs _input;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<StarterAssetsInputs>();
    }
        void Update()
    {
        if (_input.sprint)
        {
            isPlayerBusy = false;
        }
        if (!isPlayerBusy && !_input.sprint)
        {

        }
    }

    public void ReloadGun()
    {
        isPlayerBusy = true;
    }
    public void ShootGun()
    {
        isPlayerBusy = true;
    }
}
