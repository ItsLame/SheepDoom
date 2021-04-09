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
        public bool isDead = false;

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
                if (respawnTimerInGame > 0)
                {
                    showPlayerDeadUI();
                }

                //respawn once timer == 0
                else
                {
                    //disable their components
                    deathOverlay.GetComponent<Image>().enabled = false;
                    deadTextObject.GetComponent<Text>().enabled = false;
                    respawninginObject.GetComponent<Text>().enabled = false;
                    PlayerRespawnTimerObject.GetComponent<Text>().enabled = false;

                    //respawn player
                    RespawnPlayer();

                }
            }
        }

        [TargetRpc]
        private void showPlayerDeadUI()
        {
            //enable their components
            deathOverlay.GetComponent<Image>().enabled = true;
            deadTextObject.GetComponent<Text>().enabled = true;
            respawninginObject.GetComponent<Text>().enabled = true;
            PlayerRespawnTimerObject.GetComponent<Text>().enabled = true;

            //splat body
            Rigidbody myRigidBody = GetComponent<Rigidbody>();
            Vector3 moveMe = new Vector3(0, 1, 0);
            myRigidBody.rotation = Quaternion.LookRotation(moveMe);

            //deactivate movement
            this.gameObject.GetComponent<CharacterMovement>().isDead = true;
            this.gameObject.GetComponent<PlayerAttack>().isDead = true;

            //subtract time as respawn time passes
            respawnTimerInGame -= Time.deltaTime;


            //calculating time 
            respawnTimerInGame -= Time.deltaTime;
            timePlaying = TimeSpan.FromSeconds(respawnTimerInGame);

            //updating text
            Text respawnText = PlayerRespawnTimerObject.GetComponent<Text>();
            string timePlayingStr = timePlaying.ToString("ss");
            respawnText.text = timePlayingStr;


        }

        [TargetRpc]
        public void RespawnPlayer()
        {
            //move player to respawn position
            this.gameObject.transform.position = respawnLocation.transform.position;
            //not dead anymore
            this.gameObject.GetComponent<PlayerHealth>().RefillHealth();
            this.gameObject.GetComponent<CharacterMovement>().isDead = false;
            this.gameObject.GetComponent<PlayerAttack>().isDead = false;
            isDead = false;

            //reset death timer
            respawnTimerInGame = respawnTimerRef;

            //flip body right up
            Rigidbody myRigidBody = GetComponent<Rigidbody>();
            Vector3 moveMe = new Vector3(0, 0, 0);
            myRigidBody.rotation = Quaternion.LookRotation(moveMe);
        }

    }

}