using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UITimerScript : MonoBehaviour
{
    public StartTimeScript startTimeScript;
    private TextMeshProUGUI m_TextMeshProUGUI;
    private float minutes;
    private float seconds;
    private float milliseconds;

    // Start is called before the first frame update
    void Start()
    {
        m_TextMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        DisplayTime(startTimeScript.PlayerRunTime);
        if(minutes < 1)
        {
            m_TextMeshProUGUI.text = string.Format("{0:00}.{1:000}", seconds, milliseconds);
        }
        else
        {
            m_TextMeshProUGUI.text = string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);

        }
    }

    void DisplayTime(float timeToDisplay)
    {
        minutes = Mathf.FloorToInt(timeToDisplay / 60);
        seconds = Mathf.FloorToInt(timeToDisplay % 60);
        milliseconds = (timeToDisplay % 1) * 1000;
    }
}
