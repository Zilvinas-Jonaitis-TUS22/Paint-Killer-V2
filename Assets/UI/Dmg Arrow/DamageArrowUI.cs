using UnityEngine;
using System.Collections;

public class DamageArrowUI : MonoBehaviour
{
    public RectTransform arrowUI;
    public Transform playerTransform;
    public float displayDuration = 1f;
    public PlayerHealth playerHealth; // Reference to the PlayerHealth script

    [Header("Arrow Settings")]
    public float maxRotationSpeed = 360f; // Maximum rotation speed in degrees per second

    private float timer;
    private bool isShowing = false;
    private Transform target;
    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        canvasGroup = arrowUI.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = arrowUI.gameObject.AddComponent<CanvasGroup>();
        }

        arrowUI.gameObject.SetActive(false);
        canvasGroup.alpha = 0f;
    }

    void Update()
    {
        if (isShowing)
        {
            // Hide arrow if the enemy has been destroyed
            if (target == null)
            {
                isShowing = false;
                if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
                fadeCoroutine = StartCoroutine(FadeArrow(canvasGroup.alpha, 0f, 0.2f)); // Quick fade out
                return;
            }

            // Calculate direction to target
            Vector3 toEnemy = target.position - playerTransform.position;
            toEnemy.y = 0f;

            Vector3 playerForward = playerTransform.forward;
            playerForward.y = 0f;

            float angle = Vector3.SignedAngle(playerForward, toEnemy, Vector3.up);

            // Smooth rotation with max speed limit
            Quaternion currentRotation = arrowUI.rotation;
            Quaternion targetRotation = Quaternion.Euler(0, 0, -angle);
            arrowUI.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, maxRotationSpeed * Time.deltaTime);

            // Countdown timer
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                isShowing = false;
                if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
                fadeCoroutine = StartCoroutine(FadeArrow(1f, 0f, 0.5f)); // Fade out over 0.5s
            }
        }
    }


    public void ShowArrow(Transform enemy)
    {
        // Don't show if the enemy is null or player is dead
        if (enemy == null || playerHealth == null || playerHealth.health <= 0)
            return;

        target = enemy;
        timer = displayDuration;
        arrowUI.gameObject.SetActive(true);
        isShowing = true;

        // Fade in the arrow
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeArrow(0f, 1f, 0.2f)); // Fade in over 0.2s
    }

    private IEnumerator FadeArrow(float from, float to, float duration)
    {
        float elapsed = 0f;
        canvasGroup.alpha = from;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;

        if (to == 0f)
        {
            arrowUI.gameObject.SetActive(false);
        }
    }
}
