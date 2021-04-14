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
        public GameObject respawnLocation;
        [SerializeField]
        private float respawnTimerRef;
        [SerializeField]
        private float respawnTimerInGame;
        private float respawnDisplayNumber;
        [SyncVar] public bool isDead = false;

        [Space(15)]
        //the time we will use
        private TimeSpan timePlaying;


        [Space(15)]
        public GameObject deathOverlay;
        public GameObject deadTextObject;
        public GameObject respawninginObject;
        public GameObject PlayerRespawnTimerObject;

        // Start is called before the first frame update
        void Start()
        {
            //setting default respawn time, will be manipulated in future
            respawnTimerInGame = respawnTimerRef;

            //get UI objects
            deathOverlay = GameObject.Find("DeathOverlay");
            deadTextObject = GameObject.Find("DeathOverlay/deadText");
            respawninginObject = GameObject.Find("DeathOverlay/respawningIn");
            PlayerRespawnTimerObject = GameObject.Find("DeathOverlay/PlayerRespawnTimer");
        }


        // Update is called once per frame
        void Update()
        {
            //if dead, respawnTimer counts down
            if (isDead)
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
                {
                    //deactivate movement
                    this.gameObject.GetComponent<CharacterMovement>().isDead = true;
                    this.gameObject.GetComponent<PlayerAttack>().isDead = true;
                    RpcPlayerDead();
                }
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
            //splat body
            Rigidbody myRigidBody = GetComponent<Rigidbody>();
            Vector3 moveMe = new Vector3(0, 1, 0);
            myRigidBody.rotation = Quaternion.LookRotation(moveMe);
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
            this.gameObject.GetComponent<PlayerHealth>().RefillHealth();
            this.gameObject.GetComponent<PlayerHealth>().revivePlayer();
            this.gameObject.GetComponent<CharacterMovement>().isDead = false;
            this.gameObject.GetComponent<PlayerAttack>().isDead = false;
            isDead = false;
            RpcPlayerAlive();
        }

        [ClientRpc]
        void RpcPlayerAlive()
        {
            //move player to respawn position
            this.gameObject.transform.position = respawnLocation.transform.position;
            //flip body right up
            Rigidbody myRigidBody = GetComponent<Rigidbody>();
            Vector3 moveMe = new Vector3(0, 0, 0);
            myRigidBody.rotation = Quaternion.LookRotation(moveMe);
        }

        //reset death timer
        IEnumerator WaitForDeathStatus()
        {
            // wait for it to be false
            while (isDead)
                yield return null;
            respawnTimerInGame = respawnTimerRef;
        }


    }

}
