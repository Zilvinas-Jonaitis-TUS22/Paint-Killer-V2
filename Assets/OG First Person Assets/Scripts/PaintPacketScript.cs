using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintPacketScript : MonoBehaviour
{
    private GameObject _player;
    private float _distanceFromPlayer;
    public float distanceToPickup = 3;
    public float pickUpAmount;
    private Rigidbody _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("/PlayerNest/PlayerCapsule");
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        _distanceFromPlayer = Vector3.Distance(transform.position, _player.transform.position);

        if (_player.GetComponent<ShootingScript>().InkAmount != _player.GetComponent<ShootingScript>().MaxInkAmount)
        {
            if (_distanceFromPlayer <= distanceToPickup)
            {
                transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, 1);
                if(_distanceFromPlayer <= 1)
                {
                    _player.GetComponent<ShootingScript>().InkRefill(pickUpAmount);
                    Destroy(gameObject);
                }
            }
        }
    }
}
