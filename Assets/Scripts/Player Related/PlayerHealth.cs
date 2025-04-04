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
    public Sprite halfHeartSprite;
    public Sprite fallingHeartSprite;
    public Sprite slashEffectSprite;
    public Sprite healingEffectSprite; // New healing effect sprite

    private List<GameObject> heartSprites = new List<GameObject>();
    private List<GameObject> fallingHearts = new List<GameObject>();
    private List<GameObject> slashEffects = new List<GameObject>();
    private List<GameObject> healingEffects = new List<GameObject>(); // List to hold healing effects

    private float heartSpacing = 100f;

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
        int oldHealth = health;
        health += heal;
        health = Mathf.Min(health, maxHealth);
        UpdateHeartUI();
        StartCoroutine(ShowHealingEffect(oldHealth, health)); // Trigger the healing effect
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible)
            return;

        int oldHealth = health;
        health -= damage;
        health = Mathf.Max(health, 0);
        StartCoroutine(AnimateHeartLoss(oldHealth, health));

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
            heart.transform.localPosition = new Vector3(i * heartSpacing, 0, 0);
            heartSprites.Add(heart);

            GameObject fallingHeart = Instantiate(fullHeartPrefab, heartUIContainer);
            fallingHeart.GetComponent<Image>().sprite = fallingHeartSprite;
            fallingHeart.SetActive(false);
            fallingHearts.Add(fallingHeart);

            GameObject slash = new GameObject("SlashEffect");
            slash.transform.SetParent(heartUIContainer);
            slash.transform.localPosition = heart.transform.localPosition;
            Image slashImage = slash.AddComponent<Image>();
            slashImage.sprite = slashEffectSprite;
            slashImage.color = new Color(1, 1, 1, 0);
            slash.transform.localScale = new Vector3(1.5f, 1.5f, 1f); // Make slash bigger
            slashEffects.Add(slash);

            // Add healing effects for each heart
            GameObject healingEffect = new GameObject("HealingEffect");
            healingEffect.transform.SetParent(heartUIContainer);
            healingEffect.transform.localPosition = heart.transform.localPosition;
            Image healingImage = healingEffect.AddComponent<Image>();
            healingImage.sprite = healingEffectSprite;
            healingImage.color = new Color(1, 1, 1, 0); // Initially invisible
            healingEffects.Add(healingEffect);
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
                heartSprites[i].SetActive(true);
                heartImage.sprite = i < health ? fullHeartSprite : emptyHeartSprite;
            }

            // Hide healing effect for all hearts by default
            healingEffects[i].SetActive(false);
        }
    }

    private IEnumerator AnimateHeartLoss(int oldHealth, int newHealth)
    {
        //Debug.Log($"oldHealth: {oldHealth}, newHealth: {newHealth}, heartSprites.Count: {heartSprites.Count}");
        for (int i = oldHealth - 1; i >= newHealth && i < heartSprites.Count; i--)
        {
            if (i >= 0 && i < heartSprites.Count)
            {
                Image heartImage = heartSprites[i].GetComponent<Image>();
                if (heartImage != null)
                {
                    heartImage.sprite = halfHeartSprite;
                    StartCoroutine(HeartFallAndSlashAnimation(heartSprites[i], fallingHearts[i], slashEffects[i]));
                }
            }
        }
        yield return null;
    }

    private IEnumerator HeartFallAndSlashAnimation(GameObject heart, GameObject fallingHeart, GameObject slash)
    {
        fallingHeart.SetActive(true);
        fallingHeart.transform.position = heart.transform.position;

        Image slashImage = slash.GetComponent<Image>();
        slashImage.color = new Color(1, 1, 1, 1);

        float duration = 0.3f; // Duration for faster disappearance
        float elapsed = 0f;
        Vector3 originalPosition = fallingHeart.transform.position;

        // Heart falls straight down
        Vector3 targetPositionHeart = originalPosition + new Vector3(0, -50, 0); // Heart moves straight down
                                                                                 // Slash moves at a 45-degree angle (opposite direction from heart)
        Vector3 targetPositionSlash = originalPosition + new Vector3(50, -50, 0); // Slash moves top-right

        // Animate heart and slash movement
        while (elapsed < duration)
        {
            // Move the heart straight down and the slash at 45-degree angle
            fallingHeart.transform.position = Vector3.Lerp(originalPosition, targetPositionHeart, elapsed / duration);
            slash.transform.position = Vector3.Lerp(originalPosition, targetPositionSlash, elapsed / duration);

            // Fade out the heart and slash
            fallingHeart.GetComponent<Image>().color = new Color(1, 1, 1, 1 - elapsed / duration);
            slashImage.color = new Color(1, 1, 1, 1 - elapsed / duration); // Fade the slash faster

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Set final positions and deactivate
        fallingHeart.transform.position = targetPositionHeart;
        fallingHeart.SetActive(false);
        slashImage.color = new Color(1, 1, 1, 0);
        slash.SetActive(false); // Make sure the slash also gets hidden
    }

    private IEnumerator ShowHealingEffect(int oldHealth, int newHealth)
    {
        // Show healing effect on restored hearts
        for (int i = oldHealth; i < newHealth; i++)
        {
            if (i >= 0 && i < healingEffects.Count)
            {
                GameObject healingEffect = healingEffects[i];
                healingEffect.SetActive(true);
                Image healingImage = healingEffect.GetComponent<Image>();
                healingImage.color = new Color(1, 1, 1, 1); // Make it visible
                float duration = 0.5f; // Duration for healing effect visibility
                float elapsed = 0f;

                // Fade out the healing effect
                while (elapsed < duration)
                {
                    healingImage.color = new Color(1, 1, 1, 1 - elapsed / duration);
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                healingEffect.SetActive(false); // Hide the healing effect after it fades out
            }
        }
    }
}