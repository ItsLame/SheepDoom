﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class GameScore : NetworkBehaviour
{
    //the display text for tower scores
    [Space(20)]
    [SerializeField]
    private Text blueCaptureCounter;

    [SerializeField]
    private Text redCaptureCounter;

    [SerializeField]
    public GameObject completeGameUI;
    public GameObject completeGameUI2;

    //counters for tower captures per team
    //hard coded for now
    [Space(20)]
    [SerializeField]
    //[SyncVar(hook = nameof(updateScoreDisplayClient))] private float blueCaptureScore;
    [SyncVar] private float blueCaptureScore;
    [SerializeField]
    //[SyncVar(hook = nameof(updateScoreDisplayClient))] private float redCaptureScore;
    [SyncVar] private float redCaptureScore;

    // Start is called before the first frame update
    void Start()
    {
        // changed to initialize on server only, if u put on start method, everytime new player joins, it will reinitialize to 1-1
        //initialize score
        //blueCaptureScore = 1;
        //redCaptureScore = 1;

        // they are already text components...
        //get the attached score counters text component
        //blueCaptureCounter = blueCaptureCounter.GetComponent<Text>();
        //redCaptureCounter = redCaptureCounter.GetComponent<Text>();

        //display score
        blueCaptureCounter.text = blueCaptureScore.ToString();
        redCaptureCounter.text = redCaptureScore.ToString();
    }

    //update score display on all clients
    public void updateScoreDisplay()
    {
        Debug.Log("blue capture score: " + blueCaptureScore);
        Debug.Log("red capture score: " + redCaptureScore);
        blueCaptureCounter.text = blueCaptureScore.ToString();
        redCaptureCounter.text = redCaptureScore.ToString();
    }

    /*
    public void updateScoreDisplayClient(float oldValue, float newValue)
    {
        blueCaptureCounter.text = blueCaptureScore.ToString();
        redCaptureCounter.text = redCaptureScore.ToString();
    }*/

    //scoring functions
    /*public void blueScoreUp()
    {
        Debug.Log("Blue score +1");
        //if blue scores, red will -1
        blueCaptureScore += 1;
        redCaptureScore -= 1;
        //update display
        updateScoreDisplay();
    }

    public void redScoreUp()
    {
        Debug.Log("Red score +1");
        //if blue scores, red will -1
        blueCaptureScore -= 1;
        redCaptureScore += 1;
        //update display
        updateScoreDisplay();
    }*/

    // causes a syncvar delay cuz this is only run on server
    public void ScoreUp(bool _byBlue, bool _byRed)
    {
        if(_byBlue && !_byRed)
        {
            blueCaptureScore += 1;
            redCaptureScore -= 1;
        }
        else if(_byRed && !_byBlue)
        {
            redCaptureScore += 1;
            blueCaptureScore -= 1;
        }
        updateScoreDisplay();
        RpcUpdateClientScoreDisplay();
    }

    // deals with syncvar delay
    [ClientRpc]
    void RpcUpdateClientScoreDisplay()
    {
        StartCoroutine(WaitForUpdate(blueCaptureScore, redCaptureScore));
    }

    private IEnumerator WaitForUpdate(float _oldBlueScore, float _oldRedScore)
    {
        while (blueCaptureScore == _oldBlueScore && redCaptureScore == _oldRedScore)
            yield return null;
        updateScoreDisplay();
    }

    // game winning condition (will be called when base is taken)
    // shows scoreboard etc when game ends, will add timer counter condition in the future
    public void GameEnd(int TeamID)
    {
        //if blue team wins
        if (TeamID == 1)
        {
            Debug.Log("Blue Team Wins!");
            completeGameUI.SetActive(true);
            completeGameUI.GetComponent<Animator>().SetTrigger("Complete");
        }

        //if red team wins, not gonna use else for precision
        if (TeamID == 2)
        {
            Debug.Log("Red Team Wins!");
            completeGameUI2.SetActive(true);
            completeGameUI2.GetComponent<Animator>().SetTrigger("Complete");
        }
    }

    public override void OnStartServer()
    {
        blueCaptureScore = 1;
        redCaptureScore = 1;
    }

    public override void OnStartClient()
    {
        updateScoreDisplay();
    }
}
