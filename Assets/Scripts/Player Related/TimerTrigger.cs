using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public struct Rank
{
    public string name;
    public float timeThreshold;
    public Sprite rankImage;
}

public class TimerTrigger : MonoBehaviour
{
    public BoxCollider startZone;
    public BoxCollider endZone;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI finalTimeText;
    public GameObject uiElements; // UI & description, shown after timer ends
    public GameObject crosshairElements; // UI crosshair, hidden after timer ends
    public GameObject speedLines;
    public float timer = 0f;
    public TextMeshProUGUI rankText;
    public Image rankImageDisplay;

    public Rank[] ranks;

    private bool isTiming = false;
    private bool hasStarted = false;

    void Start()
    {
        timerText.gameObject.SetActive(false);
        finalTimeText.gameObject.SetActive(false);
        uiElements.SetActive(false);
        rankText.gameObject.SetActive(false);
        rankImageDisplay.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isTiming)
        {
            timer += Time.deltaTime;
            timerText.text = FormatTime(timer);
        }

        Collider[] colliders = Physics.OverlapBox(startZone.bounds.center, startZone.bounds.extents);
        foreach (Collider col in colliders)
        {
            if (!hasStarted && col.CompareTag("Player"))
            {
                StartTimer();
                break;
            }
        }

        colliders = Physics.OverlapBox(endZone.bounds.center, endZone.bounds.extents);
        foreach (Collider col in colliders)
        {
            if (hasStarted && col.CompareTag("Player"))
            {
                StopTimer();
                break;
            }
        }
    }

    void StartTimer()
    {
        hasStarted = true;
        isTiming = true;
        timer = 0f;
        timerText.gameObject.SetActive(true);
        finalTimeText.gameObject.SetActive(false);
    }

    void StopTimer()
    {
        isTiming = false;
        timerText.gameObject.SetActive(false);
        finalTimeText.text = FormatTime(timer);  // Use the formatted time here
        finalTimeText.gameObject.SetActive(true);
        speedLines.gameObject.SetActive(false);

        Rank achievedRank = GetRank();
        rankText.text = achievedRank.name;
        rankText.gameObject.SetActive(true);

        if (achievedRank.rankImage != null)
        {
            rankImageDisplay.sprite = achievedRank.rankImage;
            rankImageDisplay.gameObject.SetActive(true);
        }

        uiElements.SetActive(true);
        crosshairElements.SetActive(false);
    }

    // Helper function to format time in MM:SS
    string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return string.Format("{0:D2}:{1:D2}", minutes, seconds);
    }

    Rank GetRank()
    {
        // Default to lowest rank
        Rank lowestRank = ranks[ranks.Length - 1];

        foreach (Rank rank in ranks)
        {
            if (timer <= rank.timeThreshold)
            {
                return rank;
            }
        }

        // If no rank found (timer is higher than all thresholds), return the lowest rank (F rank)
        return lowestRank;
    }

}
