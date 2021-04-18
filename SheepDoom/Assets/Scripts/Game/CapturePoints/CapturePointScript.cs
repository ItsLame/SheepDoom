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
        [SerializeField]
        private int numOfCapturers; //logging number to check if tower is under capture or not

        //scoring bools
        [SyncVar] private bool giveScoreToCapturers = false;

        public event Action<float> OnHealthPctChangedTower = delegate { };

        // Start is called before the first frame update
        void Start()
        {
            //set the tower's hp based on the settings
            TowerInGameHP = TowerHP;

            //no one is capturing it at start so put at 0
            numOfCapturers = 0;

            if (CapturedByBlue)
            {
                var captureRenderer = this.GetComponent<Renderer>();
                captureRenderer.material.SetColor("_Color", Color.blue);
            }

            else if (CapturedByRed)
            {
                var captureRenderer = this.GetComponent<Renderer>();
                captureRenderer.material.SetColor("_Color", Color.red);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (isServer)
            {
                //once HP = 0, notify the scoring and convert the tower
                //from red to blue (captured by blue team)
                if (TowerInGameHP <= 0 && !CapturedByBlue)
                {
                    //show which point is captured, change point authority and max out towerHP
                    Debug.Log(this.name + " Captured By Blue Team");

                    CapturedServer(CapturedByBlue, CapturedByRed);
                    // no change in logic, just used a more summarized method
                    //CapturedByBlueTeam();
                    //CapturedByBlueTeam_Client();

                    //reference the score script to increase score function
                    //scoreGameObject.GetComponent<GameScore>().ScoreUp(CapturedByBlue, CapturedByRed);

                    //give score to capturers
                    giveScoreToCapturers = true;
                    
                    RpcCapturedClient();
                    //  TowerInGameHP = TowerHP;
                }

                //from blue to red (captured by red team)
                else if (TowerInGameHP <= 0 && !CapturedByRed)
                {
                    //show which point is captured, change point authority and max out towerHP
                    Debug.Log(this.name + " Captured By Blue Team");

                    CapturedServer(CapturedByBlue, CapturedByRed);
                    // no change in logic, just used a more summarized method
                    //CapturedByRedTeam();
                    //CapturedByRedTeam_Client();

                    //reference the score script to increase score function
                    //scoreGameObject.GetComponent<GameScore>().ScoreUp(CapturedByBlue, CapturedByRed);

                    //give score to capturers
                    giveScoreToCapturers = true;
                    
                    RpcCapturedClient();
                    //modifyinghealth(TowerHP);
                    //  TowerInGameHP = TowerHP;
                }

            }

            /*if (isClient)
            {
                scoreGameObject.GetComponent<GameScore>().updateScoreDisplay(); // ugly solution
            }*/
                

            //regen hp if tower is not under capture
            if ((numOfCapturers == 0) && (TowerInGameHP < TowerHP))
            {
                modifyinghealth(TowerRegenRate * Time.deltaTime);

                //debug showing tower HP
                //Debug.Log(this.name + " HP: " + TowerInGameHP);
            }

        }

        // causes a syncvar delay on client
        public void CapturedServer(bool _byBlue, bool _byRed)
        {
            if (!_byBlue && _byRed)
            {
                CapturedByBlue = true;
                CapturedByRed = false;
                GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
            }
            else if (!_byRed && _byBlue)
            {
                CapturedByRed = true;
                CapturedByBlue = false;
                GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            }
            scoreGameObject.GetComponent<GameScore>().ScoreUp(CapturedByBlue, CapturedByRed);
            modifyinghealth(TowerHP);
        }

        /*public void CapturedByBlueTeam()
        {
            CapturedByBlue = true;
            CapturedByRed = false;
            var captureRenderer = this.GetComponent<Renderer>();
            captureRenderer.material.SetColor("_Color", Color.blue);
            modifyinghealth(TowerHP);
        }

        public void CapturedByRedTeam()
        {
            CapturedByBlue = false;
            CapturedByRed = true;
            var captureRenderer = this.GetComponent<Renderer>();
            captureRenderer.material.SetColor("_Color", Color.red);
            modifyinghealth(TowerHP);
        }*/

        [ClientRpc]
        void RpcCapturedClient()
        {
            // deals with syncvar delay
            StartCoroutine(WaitForUpdate(CapturedByBlue, CapturedByRed));
        }

        private IEnumerator WaitForUpdate(bool _oldBlue, bool _oldRed)
        {
            while (CapturedByBlue == _oldBlue && CapturedByRed == _oldRed)
                yield return null;
            if(CapturedByBlue && !CapturedByRed)
            {
                var captureRenderer = this.GetComponent<Renderer>();
                captureRenderer.material.SetColor("_Color", Color.blue);
            }
            else if (CapturedByRed && !CapturedByBlue)
            {
                var captureRenderer = this.GetComponent<Renderer>();
                captureRenderer.material.SetColor("_Color", Color.red);
            }
            modifyinghealth(TowerHP);
        }

        /*[ClientRpc]
        public void CapturedByBlueTeam_Client()
        {
            CapturedByBlue = true;
            CapturedByRed = false;
            var captureRenderer = this.GetComponent<Renderer>();
            captureRenderer.material.SetColor("_Color", Color.blue);
            modifyinghealth(TowerHP);
        }

        [ClientRpc]
        public void CapturedByRedTeam_Client()
        {
            CapturedByBlue = false;
            CapturedByRed = true;
            var captureRenderer = this.GetComponent<Renderer>();
            captureRenderer.material.SetColor("_Color", Color.red);
            modifyinghealth(TowerHP);
        }*/

        public void modifyinghealth(float amount)
        {
            if(isServer) TowerInGameHP += amount;

            //         Debug.Log("health: tower in game hp:  " + TowerInGameHP);
            float currenthealthPct = TowerInGameHP / TowerHP;
            OnHealthPctChangedTower(currenthealthPct);
        }

        //check for player enter
        // runs on both client and server
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                //get player's team ID
                float tID = other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
                bool isDed = other.gameObject.GetComponent<PlayerHealth>().isPlayerDead();

                //if point belongs to red, it can be captured by blue players
                if (CapturedByRed && (tID == 1) && !isDed)
                {
                    numOfCapturers += 1;
                }

                //if point belongs to blue, it can be captured by red players
                if (CapturedByBlue && (tID == 2) && !isDed)
                {
                    numOfCapturers += 1;
                }
            }
        }

        //for capture hp reduction when staying in area
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                //get player teamID
                float tID = other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
                //get info of is player dead or alive
                bool isDed = other.gameObject.GetComponent<PlayerHealth>().isPlayerDead();


                //increase the player score when tower is captured
                if (giveScoreToCapturers == true)
                {
                    if (CapturedByRed)
                    {
                        GiveScoreToRedPlayers_Target(tID, other.gameObject);
                        giveScoreToCapturers = false;
                    }

                    //else blue  team captured red point, give score to blue
                    if (CapturedByBlue)
                    {
                        GiveScoreToBluePlayers_Target(tID, other.gameObject);
                        giveScoreToCapturers = false;
                    }

                }

                //if point belongs to red, it can be captured by blue
                if (CapturedByRed && (tID == 1) && !isDed)
                {
                    modifyinghealth(-(TowerCaptureRate * Time.deltaTime));

                }

                //if point belongs to blue, it can be captured by red
                if (CapturedByBlue && (tID == 2) && !isDed)
                {
                    modifyinghealth(-(TowerCaptureRate * Time.deltaTime));
                }


            }
        }

        public void GiveScoreToBluePlayers_Target(float tID, GameObject player)
        {
            if (tID == 2) return;
            Debug.Log("Giving Score to Blue Team Players in Range");
            increasePlayerCaptureScore(player.gameObject);
            giveScoreToCapturers = false;
            Debug.Log("End of score giving (blue)");
        }

        public void GiveScoreToRedPlayers_Target(float tID, GameObject player)
        {
            //if its red means it was previously blue, so give score to red player
            if (tID == 1) return;
            Debug.Log("Giving Score to Red Team Players in Range");
            increasePlayerCaptureScore(player.gameObject);
            giveScoreToCapturers = false;
            Debug.Log("End of score giving (red)");
        }


        //check for player exit
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                //get player's team ID
                float tID = other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();

                //if point belongs to red, it can be captured by blue players
                if (CapturedByRed && (tID == 1))
                {
                    numOfCapturers -= 1;
                }

                //if point belongs to blue, it can be captured by red players
                if (CapturedByBlue && (tID == 2))
                {
                    numOfCapturers -= 1;
                }
            }
        }


        public void increasePlayerCaptureScore(GameObject player)
        {
            player.gameObject.GetComponent<PlayerAdmin>().increaseCaptureCount();
        }


    }
}