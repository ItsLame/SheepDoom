using System;
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
    public float SecondsTimer = 0;
    public float MinutesTimer= 0;
    private TimeSpan timePlaying;  // <--------------------

    [Space(15)]
    //gameobjects to activate when time is up
    public GameObject CreepSpawner1;
    public GameObject CreepSpawner2;
    public GameObject BaseWall;
    public GameObject BossSpawner;

    [Space(15)]
    private bool TwentySecMarkPassed = false;
    private bool ThirtySecMarkPassed = false;
    private bool OnehundredTwentysecondSecMarkPassed = false;
    private bool OnehundredEightysecondSecMarkPassed = false;

    // Start is called before the first frame update
    void Start()
    {
        SecondsTimer = 0;
        TimerText.text = "0:00";
    }

    // Update is called once per frame
    void Update()
    {
        //updating and showing time
        SecondsTimer += Time.deltaTime;
        timePlaying = TimeSpan.FromSeconds(SecondsTimer);
        string timePlayingStr = timePlaying.ToString("mm':'ss");
        TimerText.text = timePlayingStr;


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
        if (SecondsTimer >= 120 && OnehundredTwentysecondSecMarkPassed == false)
        {
            AnnouncerText.GetComponent<AnnouncerTextScript>().ResetText(5);
            AnnouncerText.text = "The Boss Creep MegaBox have arrived!";
            BossSpawner.gameObject.SetActive(true);
            OnehundredTwentysecondSecMarkPassed = true;
        }
        if (SecondsTimer >= 180 && OnehundredEightysecondSecMarkPassed == false)
        {
            AnnouncerText.GetComponent<AnnouncerTextScript>().ResetText(5);
            AnnouncerText.text = "The Boss Creep MegaBox have begun patroling";
            OnehundredEightysecondSecMarkPassed = true;
        }
    }


}

/*
if (MinutesTimer == 0)
{
    //update the time


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
 */
/*
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

        TimerText.text = MinutesTimer.ToString() + ":" + "00";

        if (SecondsTimer >= 61)
        {
            MinutesTimer += 1;
            SecondsTimer = 1;
        }
    }
}

}



}*/

