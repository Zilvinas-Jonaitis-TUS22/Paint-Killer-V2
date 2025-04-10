using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 120;
    public float currentHealth = 120;

    [Header("Red Flash Effect")]
    public float flashDuration = 0.5f; // Time to stay red
    private Coroutine flashCoroutine;

    [Header("UI")]
    public Slider healthSlider; // Assign in inspector

    public BossUI bossUI; // Reference to BossUI script
    public bool dead;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Prevent negative health

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

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
        yield return new WaitForSeconds(flashDuration);
    }

    void Die()
    {
        Debug.Log("TripleBDeadinBossScript");

        if (bossUI != null)
        {
            bossUI.bossDead = true;
        }

        dead = true;

        // Delay destruction to allow TimerTrigger to detect death
        StartCoroutine(DestroyAfterDelay());
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(0.1f); // Wait just a bit
        Destroy(gameObject);
    }
}
