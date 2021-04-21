using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{
    public class GameEvent : NetworkBehaviour
    {
        //[Space(15)]
        //public GameObject gameEventText;
        public Text AnnouncerText;

        [SyncVar] public string whoKilled;
        [SyncVar] public string gotKilled;

        // Start is called before the first frame update
        void Start()
        {
            GameObject gameEventText = GameObject.Find("GameEventText");
            AnnouncerText = gameEventText.GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            if (GetComponent<PlayerHealth>().isPlayerDead())
            {
                if (isServer)
                {
                    RpcAnnouncers(whoKilled + " has slain " + gotKilled);
                }

                //if (isClient)
                //countdownTimer = 0.0f;
            }
        }

        [ClientRpc]
        void RpcAnnouncers(string announcement)
        {
            AnnouncerText.GetComponent<AnnouncerTextScript>().ResetText(3);
            AnnouncerText.text = announcement;
        }
    }
}

