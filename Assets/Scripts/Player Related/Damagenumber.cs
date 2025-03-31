using UnityEngine;
using TMPro;

public class Damagenumber : MonoBehaviour
{
    public TextMeshProUGUI damageText; // Reference to the TextMeshPro component
    private float resetTimer = 2f; // Time in seconds to wait before resetting damage text
    private float currentTimer;

    private float accumulatedDamage = 0f; // To store accumulated damage

    public Color startColor = Color.white; // The initial color
    public Color endColor = Color.red; // The color when damage is dealt
    private float lerpTime = 0f; // To track the lerp progress

    private Vector3 initialScale = Vector3.one; // Default scale
    private Vector3 maxScale = new Vector3(1.4f, 1.4f, 1.4f); // Max scale when damage is registered

    public float scaleLerpSpeed = 5f; // Speed of the scale lerping (adjustable from Unity editor)

    void Start()
    {
        currentTimer = resetTimer; // Initialize timer
        damageText.text = ""; // Make sure damage text starts empty
        damageText.color = startColor; // Set the initial color
        transform.localScale = initialScale; // Set the initial scale
    }

    void Update()
    {
        // Lerp the color between startColor and endColor constantly
        lerpTime += Time.deltaTime * 1.5f; // Adjust the speed of lerping (1.5f can be changed to control speed)
        damageText.color = Color.Lerp(startColor, endColor, Mathf.PingPong(lerpTime, 1)); // Smooth color transition between the two

        // Update the displayed damage value
        if (accumulatedDamage > 0)
        {
            damageText.text = Mathf.Ceil(accumulatedDamage).ToString(); // Round up the accumulated damage and display it
        }

        // Animate scaling (grow to 1.1 and shrink back to 1)
        if (currentTimer < resetTimer && transform.localScale != maxScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, maxScale, Time.deltaTime * scaleLerpSpeed); // Smooth scale up
        }
        else if (transform.localScale != initialScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, initialScale, Time.deltaTime * scaleLerpSpeed); // Smooth scale back to normal
        }

        // Count down the timer
        if (currentTimer > 0)
        {
            currentTimer -= Time.deltaTime;
        }
        else
        {
            // Reset the damage number when time is up
            accumulatedDamage = 0f; // Reset the accumulated damage
            damageText.text = ""; // Make the text disappear
        }
    }

    // Call this method to display and accumulate damage
    public void ShowDamageNumber(float damageAmount, Vector3 position)
    {
        // If the timer is still running, accumulate the damage
        if (currentTimer > 0)
        {
            accumulatedDamage += damageAmount; // Add new damage to the accumulated damage
        }
        else
        {
            accumulatedDamage = damageAmount; // If the timer expired, set the new damage as the initial value
        }

        // Reset the timer to hide the text after a while
        currentTimer = resetTimer; // Reset the countdown timer

        // Reset lerpTime for color lerping when new damage is registered
        lerpTime = 0f; // Reset lerp progress for smooth color transition
    }
}
