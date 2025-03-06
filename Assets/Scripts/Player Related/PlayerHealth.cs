using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int health = 5;
    public int maxHealth = 5;

    private bool isInvincible = false;
    private float invincibilityDuration = 1f;

    public GameObject deathscreen;

    [Header("UI Settings")]
    public Transform heartUIContainer;
    public GameObject fullHeartPrefab;
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;
    public Sprite halfHeartSprite; // New sprite for half-broken heart

    private List<GameObject> heartSprites = new List<GameObject>();

    void Start()
    {
        health = maxHealth;
        InitializeHeartUI();
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
        UpdateHeartUI();
        Debug.Log("updating health");
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible)
            return;

        int oldHealth = health;
        health -= damage;
        health = Mathf.Max(health, 0);
        StartCoroutine(AnimateHeartLoss(oldHealth, health)); // Animate heart loss

        StartCoroutine(InvincibilityTimer());
    }

    private IEnumerator InvincibilityTimer()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    private void InitializeHeartUI()
    {
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(fullHeartPrefab, heartUIContainer);
            heartSprites.Add(heart);
        }
        UpdateHeartUI();
    }

    private void UpdateHeartUI()
    {
        for (int i = 0; i < heartSprites.Count; i++)
        {
            Image heartImage = heartSprites[i].GetComponent<Image>();
            if (heartImage != null)
            {
                heartImage.sprite = (i < health) ? fullHeartSprite : emptyHeartSprite;
            }
        }
    }

    private IEnumerator AnimateHeartLoss(int oldHealth, int newHealth)
    {
        for (int i = oldHealth - 1; i >= newHealth; i--)
        {
            if (i >= 0 && i < heartSprites.Count)
            {
                Image heartImage = heartSprites[i].GetComponent<Image>();
                if (heartImage != null)
                {
                    heartImage.sprite = halfHeartSprite; // Set to half heart
                    yield return new WaitForSeconds(0.2f); // Small delay before falling animation
                    StartCoroutine(HeartFallAnimation(heartSprites[i]));
                }
            }
        }
    }

    private IEnumerator HeartFallAnimation(GameObject heart)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 originalPosition = heart.transform.position;
        Vector3 targetPosition = originalPosition + new Vector3(0, -50, 0);

        while (elapsed < duration)
        {
            heart.transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        heart.transform.position = targetPosition;
        heart.SetActive(false);
    }
}

