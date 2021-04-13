using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScore : MonoBehaviour
{
    //the display text for tower scores
    [Space(20)]
    [SerializeField]
    private Text blueCaptureCounter;

    [SerializeField]
    private Text redCaptureCounter;

    [SerializeField]
    public GameObject completeGameUI;

    //counters for tower captures per team
    //hard coded for now
    [Space(20)]
    [SerializeField]
    private float blueCaptureScore;
    [SerializeField]
    private float redCaptureScore;

    // Start is called before the first frame update
    void Start()
    {
        //get the attached score counters text component
        blueCaptureCounter = blueCaptureCounter.GetComponent<Text>();
        redCaptureCounter = redCaptureCounter.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        //display both of em on screen
        blueCaptureCounter.text = blueCaptureScore.ToString();
        redCaptureCounter.text =  redCaptureScore.ToString();
    }

    //scoring functions
    //its bad to make this public right? <----------------------------------------------- help
    public void blueScoreUp()
    {
        //if blue scores, red will -1
        blueCaptureScore += 1;
        redCaptureScore -= 1;
    }

    public void redScoreUp()
    {
        //if blue scores, red will -1
        blueCaptureScore -= 1;
        redCaptureScore += 1;
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
        }
    }
}
