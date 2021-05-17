using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{
    public class PlayerRespawn : NetworkBehaviour
    {
        //target respawn location
        [SerializeField] private GameObject respawnLocation = null;
        [SerializeField] private float respawnTimerRef;
        [SerializeField] private float respawnTimerInGame;
        private float respawnDisplayNumber;

        [Space(15)]
        //the time we will use
        private TimeSpan timePlaying;


        [Space(15)]
        [SerializeField] private GameObject deathOverlay;
        [SerializeField] private GameObject deadTextObject;
        [SerializeField] private GameObject respawninginObject;
        [SerializeField] private GameObject PlayerRespawnTimerObject;

        [SyncVar] [SerializeField] bool playedDead = false;
        void Start()
        {
            if(isClient)
                respawnLocation = PlayerObj.instance.ci.GetComponent<SpawnManager>().P_playerSpawnPoint;
        }
                

        // Update is called once per frame
        void Update()
        {
            //if dead, respawnTimer counts down
            if (GetComponent<PlayerHealth>().isPlayerDead())
            {
                //if dead show dead UI
                if (isClient && hasAuthority)
                {
                    if (respawnTimerInGame > 0)
                        showPlayerDeadUI();
                    else //respawn once timer == 0
                        RespawnPlayer(); //respawn player
                }

                if (isServer)
                    RpcPlayerDead();
            }
        }

        private void showPlayerDeadUI() // Show on local client which is urself that u died
        {
            //enable their components
            deathOverlay.GetComponent<Image>().enabled = true;
            deadTextObject.GetComponent<Text>().enabled = true;
            respawninginObject.GetComponent<Text>().enabled = true;
            PlayerRespawnTimerObject.GetComponent<Text>().enabled = true;

            //subtract time as respawn time passes
            respawnTimerInGame -= Time.deltaTime;

            timePlaying = TimeSpan.FromSeconds(respawnTimerInGame);

            //updating text
            Text respawnText = PlayerRespawnTimerObject.GetComponent<Text>();
            string timePlayingStr = timePlaying.ToString("ss");
            respawnText.text = timePlayingStr;
        }

        [ClientRpc]
        void RpcPlayerDead() // Show everyone that u died
        {
            if (playedDead == false)
            {
                this.gameObject.GetComponent<NetworkAnimator>().ResetTrigger("Revive");
                this.gameObject.GetComponent<NetworkAnimator>().SetTrigger("Dead");
                playedDead = true;
            }
        }

        private void RespawnPlayer()
        {
            // components here only need to be disabled on local client that died
            deathOverlay.GetComponent<Image>().enabled = false;
            deadTextObject.GetComponent<Text>().enabled = false;
            respawninginObject.GetComponent<Text>().enabled = false;
            PlayerRespawnTimerObject.GetComponent<Text>().enabled = false;

            CmdPlayerAlive();
            StartCoroutine(WaitForDeathStatus());
        }

        [Command]
        void CmdPlayerAlive()
        {
            // change components on server, syncvared to client
            GetComponent<PlayerHealth>().RefillHealth();
            GetComponent<PlayerHealth>().revivePlayer();
            RpcPlayerAlive();
            playedDead = false;

        }

        [ClientRpc]
        void RpcPlayerAlive()
        {
            //move player to respawn position
            gameObject.transform.position = respawnLocation.transform.position;
            this.gameObject.GetComponent<NetworkAnimator>().SetTrigger("Revive");
            playedDead = false;
        }

        //reset death timer
        private IEnumerator WaitForDeathStatus()
        {
            // wait for it to be false
            while (GetComponent<PlayerHealth>().isPlayerDead())
                yield return null;
            respawnTimerInGame = respawnTimerRef;
        }

        public override void OnStartClient()
        {
            if (!hasAuthority) return;
            respawnTimerInGame = respawnTimerRef;
            deathOverlay = FindMe.instance.P_DeathOverlay;
            deadTextObject = FindMe.instance.P_DeadText;
            respawninginObject = FindMe.instance.P_RespawningIn;
            PlayerRespawnTimerObject = FindMe.instance.P_RespawnTimer;
        }
    }

}
