using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTimeScript : MonoBehaviour
{
    public float PlayerRunTime;
    public bool runStartTrue;
    public PlayerHurtScript playerHurtScript;
    public float PlayerFinishTime;
    public RunTimeRanks ranks;

    // Start is called before the first frame update
    void Start()
    {
        PlayerRunTime = 0;
        runStartTrue = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (runStartTrue)
        {
            PlayerRunTime += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            runStartTrue=true;
        }
    }

    public void PlayerRunEnd()
    {
        runStartTrue = false;
        PlayerFinishTime = PlayerRunTime;
        ranks.RunFinish();
    }
}
