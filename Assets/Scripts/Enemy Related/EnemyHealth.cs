using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 7;
    public float currentHealth = 7;

    [Header("Red Flash Effect")]
    public float flashDuration = 0.5f; // Time to stay red
    private Coroutine flashCoroutine;

    [Header("References")]
    public Animator spriteAnimator;
    public SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        // If 0% health
        if (currentHealth <= 0)
        {
            spriteAnimator.SetBool("Dead", true);
            spriteAnimator.SetBool("Dying", true);
            spriteAnimator.SetBool("Injured", true);
        }
        // If less than 25% health
        else if (currentHealth <= (maxHealth / 4) * 1)
        {
            spriteAnimator.SetBool("Dying", true);
            spriteAnimator.SetBool("Injured", true);
            spriteAnimator.SetBool("Dead", false);
        }
        // If less than 75% health
        else if (currentHealth <= (maxHealth / 4) * 3)
        {
            spriteAnimator.SetBool("Injured", true);
            spriteAnimator.SetBool("Dying", false);
            spriteAnimator.SetBool("Dead", false);
        }
        // If more than 75% health
        else if (currentHealth > (maxHealth / 4) * 3)
        {
            spriteAnimator.SetBool("Injured", false);
            spriteAnimator.SetBool("Dying", false);
            spriteAnimator.SetBool("Dead", false);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        // Start the red flash effect
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashRedEffect());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashRedEffect()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red; // Turn red
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = Color.white; // Reset to normal
        }
    }

    void Die()
    {
        Destroy(gameObject); // Destroy the enemy
    }
}
