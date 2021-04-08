﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{   
    // the server will only load the scene once...so it is the master timer?
    public class GameTimerScript : NetworkBehaviour
    {
        //text to show timer
        public Text TimerText;
        public Text AnnouncerText;

        [Space(15)]
        //the time we will use
        public float SecondsTimer = 0;
        public float MinutesTimer = 0;
        private TimeSpan timePlaying;  // <--------------------
        [SyncVar] private string timePlayingStr = string.Empty;

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
            timePlayingStr = timePlaying.ToString("mm':'ss");
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
                //     CreepSpawner1.gameObject.SetActive(true);
                //    CreepSpawner2.gameObject.SetActive(true);
                BaseWall.gameObject.SetActive(false);
                ThirtySecMarkPassed = true;
            }
            if (SecondsTimer >= 120 && OnehundredTwentysecondSecMarkPassed == false)
            {
                AnnouncerText.GetComponent<AnnouncerTextScript>().ResetText(5);
                AnnouncerText.text = "The Boss Creep MegaBox have arrived!";
                //      BossSpawner.gameObject.SetActive(true);
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
}


