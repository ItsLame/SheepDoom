using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

namespace SheepDoom
{
    // the server will only load the scene once...so it is the master timer?
    public class GameTimerScript : NetworkBehaviour
    {
        //text to show timer
        public Text TimerText;
        //public Text AnnouncerText;
        public TextMeshProUGUI AnnouncerText;
        private const string TwentySeconds = "Game will begin in 10 seconds!";
        private const string ThirtySeconds = "Go forth and be victorious!";

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
        public GameObject BaseWall2;

        [Space(15)]
        [SyncVar] private bool TwentySecMarkPassed = false;
        [SyncVar] private bool ThirtySecMarkPassed = false;
        [SyncVar] private bool OnehundredTwentysecondSecMarkPassed = false;


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
            if (isServer)
            {
                SecondsTimer += Time.deltaTime;
                timePlaying = TimeSpan.FromSeconds(SecondsTimer);
                timePlayingStr = timePlaying.ToString("mm':'ss");
                TimerText.text = timePlayingStr;

                if (SecondsTimer >= 20 && TwentySecMarkPassed == false)
                //Announce that 10 seconds to start
                {
                    TwentySecMarkPassed = true;
                    RpcAnnouncers(TwentySeconds); // can be any boolean since no gameobject actions are needed here
                }

                //start spawning creeps when 30s
                if (SecondsTimer >= 30 && ThirtySecMarkPassed == false)
                {
                    CreepSpawner1.GetComponent<BaseCreepSpawner>().setSpawn();
                    CreepSpawner2.GetComponent<BaseCreepSpawner>().setSpawn();
                    NetworkServer.Destroy(BaseWall);
                    NetworkServer.Destroy(BaseWall2);
                    ThirtySecMarkPassed = true;
                    RpcAnnouncers(ThirtySeconds);
                }
            }

            if (isClient)
                TimerText.text = timePlayingStr;
        }

        [ClientRpc]
        void RpcAnnouncers(string announcement)
        {
            AnnouncerText.GetComponent<AnnouncerTextScript>().ResetText(5);
            AnnouncerText.text = announcement;
            if (ThirtySecMarkPassed)
                BaseWall.SetActive(false);
        }
    }
}


