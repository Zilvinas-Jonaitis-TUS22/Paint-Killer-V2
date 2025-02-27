using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 5;
    public int maxHealth = 5;

    private bool isInvincible = false;
    private float invincibilityDuration = 1f;

    public GameObject deathscreen;

    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        if (health <= 0)
        {
            deathscreen.SetActive(true);
        }
    }

    public void HealHealth(int heal)
    {
        health += heal;

        health = Mathf.Min(health, maxHealth);

        //Debug.Log("Player health after healing: " + health);
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible)
            return;

        health -= damage;

        health = Mathf.Max(health, 0);

        //Debug.Log("Player health after damage: " + health);

        StartCoroutine(InvincibilityTimer());
    }

    private IEnumerator InvincibilityTimer()
    {
        isInvincible = true;

        yield return new WaitForSeconds(invincibilityDuration);

        isInvincible = false;
    }
}
