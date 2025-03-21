using UnityEngine;
using System.Collections;
using UnityEngine.AI;

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
    private NavMeshAgent navMeshAgent;
    public SpawnNewBasic spawnNewBasicScript;
    public ParticleSystem ParticleSystem;
    bool isDead = false;
  


    void Start()
    {
        currentHealth = maxHealth;
        navMeshAgent = GetComponent<NavMeshAgent>();
     
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

        if (currentHealth <= 0 && !isDead) // Only run once
        {
            isDead = true;
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
        StopMovement(); // Ensure enemy stops moving

        if (ParticleSystem != null)
        {
            ParticleSystem.Play();
        }

        spawnNewBasicScript.InstantiateBlendShapeEnemy();

        Destroy(gameObject); // Destroy enemy immediately after playing effects
    }

    void StopMovement()
    {
       
            navMeshAgent.isStopped = true;  // Completely stop navigation
            navMeshAgent.velocity = Vector3.zero; // Ensure no lingering movement
        
    }

   
}

