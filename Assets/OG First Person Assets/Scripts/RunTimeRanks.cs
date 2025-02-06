using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunTimeRanks : MonoBehaviour
{
    private GameObject _player;
    public StartTimeScript StartTimeScript;

    public string[] RankMessages;

    public float SRank;
    public float ARank;
    public float BRank;
    public float CRank;
    public float DRank;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("/PlayerNest/PlayerCapsule");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RunFinish()
    {

        _player.GetComponent<FirstPersonController>().enabled = false;
        _player.GetComponent<ShootingScript>().enabled = false;
        float runTime = StartTimeScript.PlayerFinishTime;
        Debug.Log(runTime);
        TextMeshProUGUI textBox = GetComponent<TextMeshProUGUI>();
        if (runTime < SRank)
        {
            textBox.text = RankMessages[0];
        }
        else if (runTime < ARank)
        {
            textBox.text = RankMessages[1];
        }
        else if (runTime < BRank)
        {
            textBox.text = RankMessages[2];
        }
        else if (runTime < CRank)
        {
            textBox.text = RankMessages[3];
        }
        else if (runTime < DRank)
        {
            textBox.text = RankMessages[4];
        }

    }
}
