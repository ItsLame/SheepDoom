using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Mirror;

namespace SheepDoom
{
    public class CapturePointScript : Objective
    {
        //attach the score gameobject to count the score
        public GameObject ScoreGameObject;

        //tower hp counters
        [Space(20)]
        
        //base hp
        [Tooltip("How much HP the tower has, edit this")]
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
        [SerializeField] private int NumOfCapturers; //logging number to check if tower is under capture or not

        //scoring bools
        [SerializeField]
        private bool GiveScoreToCapturers = false;

        //public event Action<float> OnHealthPctChangedTower = delegate { };
        protected override void InitHealth()
        {
            //P_captureGameObject = this.gameObject;
            P_scoreGameObject = ScoreGameObject;
            P_hp = HP;
            P_inGameHP = InGameHP;
            P_inGameHP = P_hp;
            P_captureRate = CaptureRate;
            P_regenRate = RegenRate;
            //CheckCaptureBool();
            P_numOfCapturers = NumOfCapturers;
        }

        // Update is called once per frame
        protected override void Update()
        {
            if (isServer)
            {
                //once HP = 0, notify the scoring and convert the tower
                if (P_inGameHP <= 0 && (!CapturedByBlue || !CapturedByRed))
                {
                    //show which point is captured, change point authority and max out towerHP
                    CapturedServer(CapturedByBlue, CapturedByRed);
                    RpcUpdateClients(true, false);
                }

                //regen hp if tower is not under capture
                if ((P_numOfCapturers == 0) && (P_inGameHP < P_hp))
                {
                    ModifyingHealth(P_regenRate * Time.deltaTime);
                    RpcUpdateClients(false, true);
                }  
            }
        }

        // causes a syncvar delay on client
        public void CapturedServer(bool _byBlue, bool _byRed)
        {
            if (!_byBlue && _byRed)
            {
                CapturedByBlue = true;
                CapturedByRed = false;
            }
            else if (!_byRed && _byBlue)
            {
                CapturedByRed = true;
                CapturedByBlue = false;
            }
            SetTowerColor();
            GiveScoreToCapturers = true;
            P_scoreGameObject.GetComponent<GameScore>().ScoreUp(CapturedByBlue, CapturedByRed);
            ModifyingHealth(P_hp);
        }

        [ClientRpc]
        void RpcUpdateClients(bool _isCapture, bool _isChangeHp)
        {
            if(_isCapture)
                // deals with syncvar delay
                StartCoroutine(WaitForUpdate(CapturedByBlue, CapturedByRed));
            else if(_isChangeHp)
                ModifyingHealth(0); // 0 because value from server will sync
        }

        /*[TargetRpc]
        void TargetUpdateClient(NetworkConnection conn, GameObject _player)
        {
            _player.GetComponent<PlayerAdmin>().increaseCaptureCount();
        }*/

        private IEnumerator WaitForUpdate(bool _oldBlue, bool _oldRed)
        {
            while (CapturedByBlue == _oldBlue && CapturedByRed == _oldRed)
                yield return null;
            SetTowerColor();
            ModifyingHealth(0); // 0 because value from server will sync
        }


        /*
        public void ModifyingHealth(float amount)
        {
            if(isServer) P_inGameHP += amount;

            // Debug.Log("health: tower in game hp:  " + P_inGameHP);
            float currenthealthPct = P_inGameHP / P_hp;
            OnHealthPctChangedTower(currenthealthPct);
        }
        */

        //check for player enter
        // runs on server only
        [Server]
        protected override void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                //get player's team ID
                float tID = other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
                bool isDed = other.gameObject.GetComponent<PlayerHealth>().isPlayerDead();
                if (((CapturedByRed && tID == 1) || (CapturedByBlue && tID == 2)) && !isDed)
                    P_numOfCapturers += 1;
            }
        }


        //for capture hp reduction when staying in area
        [Server]
        protected override void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                //get player teamID
                float tID = other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
                //get info of is player dead or alive
                bool isDed = other.gameObject.GetComponent<PlayerHealth>().isPlayerDead();

                //increase the player score when tower is captured
                if (GiveScoreToCapturers)
                {
                    Debug.Log("Giving score to capturers");

                    if (CapturedByRed && tID == 2 || CapturedByBlue && tID == 1)
                    {
                        other.GetComponent<PlayerAdmin>().IncreaseCount(true, false, false);
                        GiveScoreToCapturers = false;
                    }

                /*if (SDNetworkManager.LocalPlayersNetId.TryGetValue(other.GetComponent<PlayerObj>().ci.GetComponent<NetworkIdentity>(), out NetworkConnection conn))
                {
                    TargetUpdateClient(conn, other.gameObject);
                }
*/
            }

                if (((CapturedByRed && tID == 1) || (CapturedByBlue && tID == 2)) && !isDed)
                {
                    ModifyingHealth(-(P_captureRate * Time.deltaTime));
                    RpcUpdateClients(false, true);
                }
            }
        }

        public void GiveScoreToBluePlayers_Target(float tID, GameObject player)
        {
            if (tID == 2) return;
            Debug.Log("Giving Score to Blue Team Players in Range");
            //increasePlayerCaptureScore(player.gameObject);
            GiveScoreToCapturers = false;
            Debug.Log("End of score giving (blue)");
        }

        public void GiveScoreToRedPlayers_Target(float tID, GameObject player)
        {
            //if its red means it was previously blue, so give score to red player
            if (tID == 1) return;
            Debug.Log("Giving Score to Red Team Players in Range");
            //increasePlayerCaptureScore(player.gameObject);
            GiveScoreToCapturers = false;
            Debug.Log("End of score giving (red)");
        }


        //check for player exit
        [Server]
        protected override void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                //get player's team ID
                float tID = other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
                bool isDed = other.gameObject.GetComponent<PlayerHealth>().isPlayerDead();
                if (((CapturedByRed && tID == 1) || (CapturedByBlue && tID == 2)) && !isDed)
                    P_numOfCapturers -= 1;
            }
        }

        /*public void increasePlayerCaptureScore(GameObject player)
        {
            player.gameObject.GetComponent<PlayerAdmin>().increaseCaptureCount();
        }*/

        private void SetTowerColor()
        {
            if (CapturedByBlue && !CapturedByRed)
                GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
            else if (CapturedByRed && !CapturedByBlue)
                GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        }

        public override void OnStartServer()
        {
            SetTowerColor();
            P_inGameHP = P_hp; //set the tower's hp based on the settings
            P_numOfCapturers = 0; //no one is capturing it at start so put at 0
            GiveScoreToCapturers = false;
        }

        public override void OnStartClient()
        {
            SetTowerColor();
        }
    }
}