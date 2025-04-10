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
    public float timer = 0f; // This will hold the timer value, used to display the timer
    public TextMeshProUGUI rankText;
    public Image rankImageDisplay;

    public Rank[] ranks;

    private bool isTiming = false;
    private bool hasStarted = false;

    private float adjustedTimer;

    public BossHealth bossHealth;


    void Start()
    {
        timerText.gameObject.SetActive(false);
        finalTimeText.gameObject.SetActive(false);
        uiElements.SetActive(false);
        rankText.gameObject.SetActive(false);
        rankImageDisplay.gameObject.SetActive(false);

        adjustedTimer = timer;
    }

    void Update()
    {
        
        if (isTiming)
        {
            timer += Time.deltaTime;  // Keep adding real-time
            timerText.text = FormatTime(timer + adjustedTimer);

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

        /*  colliders = Physics.OverlapBox(endZone.bounds.center, endZone.bounds.extents);
          foreach (Collider col in colliders)
          {
              if (hasStarted && col.CompareTag("Player"))
              {
                  StopTimer();
                  break;
              }
          } */
        if (hasStarted && bossHealth != null && bossHealth.dead)
        {
            //Debug.Log("TripleBDead");
            StopTimer();
        }
    }

    void StartTimer()
    {
        if (!hasStarted)
        {
            hasStarted = true;
            isTiming = true;
            timer = 0f;  // Reset the adjusted timer to 0
            adjustedTimer = 0f;
            timerText.gameObject.SetActive(true);
            finalTimeText.gameObject.SetActive(false);
        }
    }

    void StopTimer()
    {
        isTiming = false;
        timerText.gameObject.SetActive(false);
        finalTimeText.text = FormatTime(timer + adjustedTimer);  // Display the final adjusted time
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
        Rank lowestRank = ranks[ranks.Length - 1];

        foreach (Rank rank in ranks)
        {
            if (timer + adjustedTimer <= rank.timeThreshold)
            {
                return rank;
            }
        }

        return lowestRank;
    }
    public void AdjustTimer(float timeChange)
    {
        adjustedTimer += timeChange;
        adjustedTimer = Mathf.Max(-timer, adjustedTimer); // ensure total time doesn't go below 0
        //Debug.Log("Adjusted Timer: " + (timer + adjustedTimer));
    }

}
