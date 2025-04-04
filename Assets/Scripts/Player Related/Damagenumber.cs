using UnityEngine;
using TMPro;

public class Damagenumber : MonoBehaviour
{
    public TextMeshProUGUI damageText;

    [Header("Timing")]
    public float displayDuration = 2f; // Adjustable display time
    private float resetTimer;
    private float currentTimer;

    private float accumulatedDamage = 0f;
    private float lastDisplayedDamage = 0f;

    [Header("Color")]
    public Color startColor = Color.white;
    public Color endColor = Color.red;
    private float lerpTime = 0f;

    [Header("Scale Animation")]
    private Vector3 initialScale = Vector3.one;
    private Vector3 maxScale = new Vector3(1.2f, 1.2f, 1.2f); // Jump scale
    public float scaleLerpSpeed = 5f;
    private bool isScaling = false;

    [Header("Fade Out")]
    private bool isFading = false;
    public float fadeDuration = 1f; // Duration of fade-out
    private float fadeTimer = 0f;

    void Start()
    {
        resetTimer = displayDuration;
        currentTimer = resetTimer;
        damageText.text = "";
        damageText.color = startColor;
        transform.localScale = initialScale;
    }

    void Update()
    {
        // Pulse color if not fading
        if (!isFading)
        {
            lerpTime += Time.deltaTime * 1.5f;
            Color lerpedColor = Color.Lerp(startColor, endColor, Mathf.PingPong(lerpTime, 1));
            damageText.color = new Color(lerpedColor.r, lerpedColor.g, lerpedColor.b, 1f);
        }

        // Update damage display
        if (accumulatedDamage > 0)
        {
            float displayedDamage = Mathf.Ceil(accumulatedDamage);
            damageText.text = displayedDamage.ToString();

            if (displayedDamage != lastDisplayedDamage)
            {
                transform.localScale = maxScale;
                isScaling = true;
                lastDisplayedDamage = displayedDamage;
            }
        }

        // Smoothly scale back to original
        if (isScaling)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, initialScale, Time.deltaTime * scaleLerpSpeed);
            if (Vector3.Distance(transform.localScale, initialScale) < 0.01f)
            {
                transform.localScale = initialScale;
                isScaling = false;
            }
        }

        // Timer countdown
        if (currentTimer > 0)
        {
            currentTimer -= Time.deltaTime;
        }
        else if (!isFading && accumulatedDamage > 0)
        {
            // Start fade-out
            isFading = true;
            fadeTimer = 0f;
        }

        // Handle fading
        if (isFading)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, fadeTimer / fadeDuration);
            Color currentColor = damageText.color;
            damageText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);

            if (alpha <= 0.01f)
            {
                damageText.text = "";
                accumulatedDamage = 0f;
                lastDisplayedDamage = 0f;
                isFading = false;
            }
        }
    }

    // Call this to show damage and reset timer
    public void ShowDamageNumber(float damageAmount, Vector3 position)
    {
        resetTimer = displayDuration; // Apply adjustable duration

        if (currentTimer > 0)
        {
            accumulatedDamage += damageAmount;
        }
        else
        {
            accumulatedDamage = damageAmount;
        }

        currentTimer = resetTimer;
        lerpTime = 0f;

        // Reset fade if currently fading
        if (isFading)
        {
            isFading = false;
            Color currentColor = damageText.color;
            damageText.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
        }
    }
}
