using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimerScript : MonoBehaviour
{
    //text to show timer
    public Text TimerText;
    public Text AnnouncerText;

    [Space(15)]
    //the time we will use
    public float SecondsTimer = 10f;
    public float MinutesTimer= 0;

    [Space(15)]
    //gameobjects to activate when time is up
    public GameObject CreepSpawner1;
    public GameObject CreepSpawner2;
    public GameObject BaseWall;

    [Space(15)]
    private bool TwentySecMarkPassed = false;
    private bool ThirtySecMarkPassed = false;


    // Start is called before the first frame update
    void Start()
    {
        //the time starts now
        SecondsTimer = 0;
        MinutesTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (MinutesTimer == 0)
        {
            //update the time
            SecondsTimer += Time.deltaTime;

            //if less than 10s
            if (SecondsTimer < 10)
            {
                TimerText.text = "0" + SecondsTimer.ToString("0");
            }

            //else more than 10s
            else
            {
                TimerText.text = SecondsTimer.ToString("0");
            }

            if (SecondsTimer >= 20 && TwentySecMarkPassed == false)
            //Announce that 10 seconds to start
            {
                AnnouncerText.GetComponent<AnnouncerTextScript>().ResetText(5);
                AnnouncerText.text = "Game will begin in 10 seconds!";
                TwentySecMarkPassed = true;

            }

            //start spawning creeps when 30s
            if (SecondsTimer >= 30 && ThirtySecMarkPassed == false)
            {
                AnnouncerText.GetComponent<AnnouncerTextScript>().ResetText(5);
                AnnouncerText.text = "Go forth and be victorious!";
                CreepSpawner1.gameObject.SetActive(true);
                CreepSpawner2.gameObject.SetActive(true);
                BaseWall.gameObject.SetActive(false);
                ThirtySecMarkPassed = true;
            }

            //when hit one min
            if (SecondsTimer >= 60)
            {
                TimerText.text = "1:" + "00";

                if (SecondsTimer >= 61)
                {
                    SecondsTimer = 1;
                    MinutesTimer += 1;
                }
            }
        }

        //after one minute
        else
        {
            //update the time
            SecondsTimer += Time.deltaTime;

            //if less than 10s
            if (SecondsTimer < 10)
            {
                TimerText.text = MinutesTimer.ToString() + ":" + "0" + SecondsTimer.ToString("0");
            }

            else
            {
                TimerText.text = MinutesTimer.ToString() + ":" + SecondsTimer.ToString("0");

                //once a minute passes, reset seconds to 0 and add to minutes counter and change the displayed text

                if (SecondsTimer >= 60)
                {
                    MinutesTimer += 1;
                    TimerText.text = MinutesTimer.ToString() + ":" + "00";

                    if (SecondsTimer >= 61)
                    {
                        SecondsTimer = 1;
                    }
                }
            }

        }



    }
}
