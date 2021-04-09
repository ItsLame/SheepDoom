using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SheepDoom
{
    public class PlayerRespawn : MonoBehaviour
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
                //this.gameObject.GetComponent<CharacterMovement>().isDead = true;
                this.gameObject.GetComponent<PlayerAttack>().isDead = true;

                //subtract time as respawn time passes
                respawnTimerInGame -= Time.deltaTime;

                //round off for respawn display number
                respawnDisplayNumber = Mathf.Round(respawnTimerInGame + 0.5f);
                Text respawnText = PlayerRespawnTimerObject.GetComponent<Text>();
                respawnText.text = respawnDisplayNumber.ToString();

                //respawn once timer == 0
                if (respawnTimerInGame <= 0)
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

        public void RespawnPlayer()
        {
            respawnTimerInGame = respawnTimerRef;
            isDead = false;

            this.gameObject.transform.position = respawnLocation.transform.position;
            this.gameObject.transform.position = respawnLocation.transform.position;
            Rigidbody myRigidBody = GetComponent<Rigidbody>();
            Vector3 moveMe = new Vector3(0, 0, 0);
            myRigidBody.rotation = Quaternion.LookRotation(moveMe);

            //activate all buttons
            //this.gameObject.GetComponent<CharacterMovement>().isDead = false;
            this.gameObject.GetComponent<PlayerAttack>().isDead = false;
            this.gameObject.GetComponent<PlayerHealth>().RefillHealth();
        }

        /*
        GameObject GetChildWithName(GameObject obj, string name)
        {
            Transform trans = obj.transform;
            Transform childTrans = trans.Find(name);
            if (childTrans != null)
            {
                return childTrans.gameObject;
            }
            else
            {
                return null;
            }
        }*/
    }

}