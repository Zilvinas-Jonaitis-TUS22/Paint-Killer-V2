using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitScript : MonoBehaviour
{
    public float enemyHealth;
    public PlayerHurtScript playerHurtScript;
    public List<ParticleCollisionEvent> collisionEvents;
    private CapsuleCollider _Collider;
    public GameObject dropOnKill;

    // Start is called before the first frame update
    void Start()
    {
        _Collider = GetComponent<CapsuleCollider>();
        collisionEvents = new List<ParticleCollisionEvent>(16);

    }

    // Update is called once per frame
    void Update()
    {
        HealthCheck();
    }


    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            ParticleSystem ps = other.GetComponent<ParticleSystem>();
            int collisionAmount = ps.GetCollisionEvents(gameObject, collisionEvents);
            //Debug.Log(collisionAmount);

            enemyHealth -= 50f * collisionAmount;
        }
    }
    public void HealthCheck()
    {
        if (enemyHealth <= 0)
        {
            if (dropOnKill != null)
            {
                Instantiate(dropOnKill, new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), Quaternion.identity);
            }
            playerHurtScript.EnemyKilled();
            Destroy(gameObject);
        }
    }
}
