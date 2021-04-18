using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Mirror;

namespace SheepDoom
{
    public class CapturePointScript : NetworkBehaviour
    {
        //attach the score gameobject to count the score
        public GameObject scoreGameObject;

        //tower hp counters
        [Space(20)]
        [SerializeField]
        //base hp
        [Tooltip("How much HP the tower has, edit this")]
        private float TowerHP;
        [SerializeField]
        [SyncVar] private float TowerInGameHP; //to be used in game, gonna be the one fluctuating basically

        //rate of capture
        [SerializeField]
        private float TowerCaptureRate;

        //regeneration rate if not under capture
        [SerializeField]
        private float TowerRegenRate;

        //captured bools
        [Space(20)]
        [SerializeField]
        [SyncVar] private bool CapturedByBlue;
        [SerializeField]
        [SyncVar] private bool CapturedByRed;

        // Server only
        [SerializeField]
        private int numOfCapturers; //logging number to check if tower is under capture or not
        //scoring bools
        private bool giveScoreToCapturers = false;

        public event Action<float> OnHealthPctChangedTower = delegate { };

        // Update is called once per frame
        void Update()
        {
            if (isServer)
            {
                //once HP = 0, notify the scoring and convert the tower
                if (TowerInGameHP <= 0 && (!CapturedByBlue || !CapturedByRed))
                {
                    //show which point is captured, change point authority and max out towerHP
                    CapturedServer(CapturedByBlue, CapturedByRed);
                    RpcUpdateClients(true, false);
                }

                //regen hp if tower is not under capture
                if ((numOfCapturers == 0) && (TowerInGameHP < TowerHP))
                {
                    modifyinghealth(TowerRegenRate * Time.deltaTime);
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
            giveScoreToCapturers = true;
            scoreGameObject.GetComponent<GameScore>().ScoreUp(CapturedByBlue, CapturedByRed);
            modifyinghealth(TowerHP);
        }

        [ClientRpc]
        void RpcUpdateClients(bool _isCapture, bool _isChangeHp)
        {
            if(_isCapture)
                // deals with syncvar delay
                StartCoroutine(WaitForUpdate(CapturedByBlue, CapturedByRed));
            else if(_isChangeHp)
                modifyinghealth(0); // 0 because value from server will sync
        }

        [TargetRpc]
        void TargetUpdateClient(NetworkConnection conn, GameObject _player)
        {
            _player.GetComponent<PlayerAdmin>().increaseCaptureCount();
        }

        private IEnumerator WaitForUpdate(bool _oldBlue, bool _oldRed)
        {
            while (CapturedByBlue == _oldBlue && CapturedByRed == _oldRed)
                yield return null;
            SetTowerColor();
            modifyinghealth(0); // 0 because value from server will sync
        }

        public void modifyinghealth(float amount)
        {
            if(isServer) TowerInGameHP += amount;

            // Debug.Log("health: tower in game hp:  " + TowerInGameHP);
            float currenthealthPct = TowerInGameHP / TowerHP;
            OnHealthPctChangedTower(currenthealthPct);
        }

        //check for player enter
        // runs on server only
        [Server]
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                //get player's team ID
                float tID = other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
                bool isDed = other.gameObject.GetComponent<PlayerHealth>().isPlayerDead();
                if (((CapturedByRed && tID == 1) || (CapturedByBlue && tID == 2)) && !isDed)
                    numOfCapturers += 1;
            }
        }


        //for capture hp reduction when staying in area
        [Server]
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                //get player teamID
                float tID = other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
                //get info of is player dead or alive
                bool isDed = other.gameObject.GetComponent<PlayerHealth>().isPlayerDead();

                //increase the player score when tower is captured
                if (giveScoreToCapturers)
                {
                    if(SDNetworkManager.LocalPlayersNetId.TryGetValue(other.GetComponent<PlayerObj>().ci.GetComponent<NetworkIdentity>(), out NetworkConnection conn))
                    {
                        TargetUpdateClient(conn, other.gameObject);
                        giveScoreToCapturers = false;
                    }
                    /*if (CapturedByRed)
                    {
                        GiveScoreToRedPlayers_Target(tID, other.gameObject);
                        giveScoreToCapturers = false;
                    }

                    //else blue  team captured red point, give score to blue
                    if (CapturedByBlue)
                    {
                        GiveScoreToBluePlayers_Target(tID, other.gameObject);
                        giveScoreToCapturers = false;
                    }*/
                }

                if (((CapturedByRed && tID == 1) || (CapturedByBlue && tID == 2)) && !isDed)
                {
                    modifyinghealth(-(TowerCaptureRate * Time.deltaTime));
                    RpcUpdateClients(false, true);
                }
            }
        }

        public void GiveScoreToBluePlayers_Target(float tID, GameObject player)
        {
            if (tID == 2) return;
            Debug.Log("Giving Score to Blue Team Players in Range");
            //increasePlayerCaptureScore(player.gameObject);
            giveScoreToCapturers = false;
            Debug.Log("End of score giving (blue)");
        }

        public void GiveScoreToRedPlayers_Target(float tID, GameObject player)
        {
            //if its red means it was previously blue, so give score to red player
            if (tID == 1) return;
            Debug.Log("Giving Score to Red Team Players in Range");
            //increasePlayerCaptureScore(player.gameObject);
            giveScoreToCapturers = false;
            Debug.Log("End of score giving (red)");
        }


        //check for player exit
        [Server]
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                //get player's team ID
                float tID = other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
                bool isDed = other.gameObject.GetComponent<PlayerHealth>().isPlayerDead();
                if (((CapturedByRed && tID == 1) || (CapturedByBlue && tID == 2)) && !isDed)
                    numOfCapturers -= 1;
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
            TowerInGameHP = TowerHP; //set the tower's hp based on the settings
            numOfCapturers = 0; //no one is capturing it at start so put at 0
        }

        public override void OnStartClient()
        {
            SetTowerColor();
        }
    }
}