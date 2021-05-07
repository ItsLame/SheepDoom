using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Mirror;

namespace SheepDoom
{
    public class CaptureBaseScript : Objective
    {
        //attach the score gameobject to count the score
        public GameObject ScoreGameObject;


        [Space(15)]
        [SerializeField] private GameObject BaseModel;
        [SerializeField] private GameObject BaseProjectile;
        [SerializeField] private bool hasClosed = false;
        [SerializeField] private float towerAtkCD;
        [SerializeField] private float towerAtkCDinGame;
        private GameObject baseBullet;
        //    [SerializeField] private bool towerAtk;
        //Base hp counters
        [Space(20)]

        //base hp
        [Tooltip("How much HP the Base has, edit this")]
        [SerializeField] private float HP;
        [SyncVar] [SerializeField] private float InGameHP; //to be used in game, gonna be the one fluctuating basically

        //rate of capture
        [SerializeField] private float CaptureRate;

        //regeneration rate if not under capture
        [SerializeField] private float RegenRate;

        //captured bools
        [Space(20)]
        [SyncVar] [SerializeField] private bool CapturedByBlue;
        [SyncVar] [SerializeField] private bool CapturedByRed;
        [SerializeField] private int NumOfCapturers; //logging number to check if Base is under capture or not

        protected override bool P_capturedByBlue { get => CapturedByBlue; set => CapturedByBlue = value; }
        protected override bool P_capturedByRed { get => CapturedByRed; set => CapturedByRed = value; }


        protected override void InitObjective()
        {
            P_scoreGameObject = ScoreGameObject;
            P_hp = HP;
            P_inGameHP = InGameHP;
            P_captureRate = CaptureRate;
            P_regenRate = RegenRate;
            P_numOfCapturers = NumOfCapturers;
            P_giveScoreToCapturers = false;

            // set the Tower's hp based on the settings
            P_inGameHP = P_hp;

            // no one is capturing it at start so put at 0
            //P_numOfCapturers = 0;

            // this is tower's script
            P_isBase = true;

            towerAtkCDinGame = towerAtkCD;
        }

        // check for player enter
        [ServerCallback]
        protected override void OnTriggerEnter(Collider _collider)
        {
            if (_collider.tag == "Player")
            {
                //get player's team ID
                float tID = _collider.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
                bool isDed = _collider.gameObject.GetComponent<PlayerHealth>().isPlayerDead();
                if (((P_capturedByRed && tID == 1) || (P_capturedByBlue && tID == 2)) && !isDed)
                {
                    P_numOfCapturers += 1;

                    //animation to close
                    if (!hasClosed)
                    {
                        BaseModel.GetComponent<NetworkAnimator>().SetTrigger("Close");
                        hasClosed = true;
                    }

                }

            }
        }


        [ServerCallback]
        // check for player exit
        protected override void OnTriggerExit(Collider _collider)
        {
            if (_collider.CompareTag("Player"))
            {
                //get player's team ID
                float tID = _collider.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
                bool isDed = _collider.gameObject.GetComponent<PlayerHealth>().isPlayerDead();
                if (((P_capturedByRed && tID == 1) || (P_capturedByBlue && tID == 2)) && !isDed)
                    P_numOfCapturers -= 1;
            }
        }

        [ServerCallback]
        protected override void OnTriggerStay(Collider _collider)
        {
            if (_collider.CompareTag("Player"))
            {
                // get player teamID
                float tID = _collider.GetComponent<PlayerAdmin>().getTeamIndex();
                // get info of is player dead or alive
                bool isDed = _collider.GetComponent<PlayerHealth>().isPlayerDead();

                if (!isDed)
                {
                    //have another condition to check whether base can be captured
                    //if no outer tower has been captured base cant be captured
                    if (P_capturedByBlue && tID == 2)
                    {
                        towerAtkCDinGame -= Time.deltaTime;

                        if (towerAtkCDinGame <= 0)
                        {
                            Vector3 additionalDistance = new Vector3(0, 50, 0);
                            //attack!!! rain bullets from ze sky
                            baseBullet = Instantiate(BaseProjectile, transform);
                            baseBullet.transform.SetParent(null, false);
                            baseBullet.transform.SetPositionAndRotation(_collider.transform.position + additionalDistance, _collider.transform.rotation);
                            NetworkServer.Spawn(baseBullet);

                            towerAtkCDinGame = towerAtkCD;
                        }

                        if (ScoreGameObject.GetComponent<GameScore>().getBlueScore() < 2)
                        {
                            ModifyingHealth(-(P_captureRate * Time.deltaTime));
                            RpcUpdateClients(false, true, false);
                        }
                    }
                    else if (P_capturedByRed && tID == 1)
                    {
                        towerAtkCDinGame -= Time.deltaTime;

                        if (towerAtkCDinGame <= 0)
                        {
                            Vector3 additionalDistance = new Vector3(0, 50, 0);
                            //attack!!! rain bullets from ze sky
                            baseBullet = Instantiate(BaseProjectile, transform);
                            baseBullet.transform.SetParent(null, false);
                            baseBullet.transform.SetPositionAndRotation(_collider.transform.position + additionalDistance, _collider.transform.rotation);
                            NetworkServer.Spawn(baseBullet);

                            towerAtkCDinGame = towerAtkCD;
                        }

                        if (ScoreGameObject.GetComponent<GameScore>().getRedScore() < 2)
                        {
                            ModifyingHealth(-(P_captureRate * Time.deltaTime));
                            RpcUpdateClients(false, true, false);
                        }
                    }
                }

            }
        }

        protected override void Victory()
        {
            gameStatus.GetComponent<GameStatus>().P_gameEnded = true;
            // if base owner is red team
            if (P_capturedByRed)
                P_scoreGameObject.GetComponent<GameScore>().GameEnd(1);
            // if base owner is blue team
            if (P_capturedByBlue)
                P_scoreGameObject.GetComponent<GameScore>().GameEnd(2);
        }

        [ServerCallback]
        protected override void OpenBaseAnim()
        {
            if (hasClosed)
            {
                BaseModel.GetComponent<NetworkAnimator>().SetTrigger("Open");
                hasClosed = false;
            }
        }


        //accessor method to get teamID
        public float getTeamID()
        {
            if (CapturedByBlue)
            {
                return 1;
            }

            else 
            {
                return 2;
            }

        }

        [Server]
        // method called by enemy player message object to reduce capturers
        public void reduceNumOfCapturers()
        {
            P_numOfCapturers -= 1;
            Debug.Log("Number Of Capturers: " + P_numOfCapturers);
        }
    }
}
