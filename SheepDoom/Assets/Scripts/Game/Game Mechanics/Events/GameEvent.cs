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
        public Text AnnouncerText;

        [SyncVar] public string whoKilled;
        [SyncVar] public string gotKilled;

        [SyncVar] public bool isNeutral = false;
        [SyncVar] public bool isBoss = false;
        [SyncVar] public bool isMinion = false;

        // Start is called before the first frame update
        void Start()
        {
            GameObject gameEventText = FindMe.instance.P_GameEvent;
            AnnouncerText = gameEventText.GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            if (GetComponent<PlayerHealth>().isPlayerDead())
            {
                if (isServer)
                {
                    if (isNeutral)
                        RpcAnnouncers(gotKilled + " has been slain by a neutral");
                    else if(isBoss)
                        RpcAnnouncers(gotKilled + " has been slain by a boss");
                    else if (isMinion)
                        RpcAnnouncers(gotKilled + " has been slain by a minion");
                    else
                        RpcAnnouncers(whoKilled + " has slain " + gotKilled);
                }
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

