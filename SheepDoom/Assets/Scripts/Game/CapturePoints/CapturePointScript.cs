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
        [SyncVar]
        private float TowerInGameHP; //to be used in game, gonna be the one fluctuating basically

        //rate of capture
        [SerializeField]
        private float TowerCaptureRate;

        //regeneration rate if not under capture
        [SerializeField]
        private float TowerRegenRate;

        //captured bools
        [Space(20)]
        [SerializeField]
        [SyncVar]
        private bool CapturedByBlue;
        [SerializeField]
        [SyncVar]
        private bool CapturedByRed;
        [SerializeField]
        private int numOfCapturers; //logging number to check if tower is under capture or not

        //public event Action<float> OnHealthPctChangedTower = delegate { };

        // Start is called before the first frame update
        void Start()
        {
            //set the tower's hp based on the settings
            TowerInGameHP = TowerHP;

            //no one is capturing it at start so put at 0
            numOfCapturers = 0;
        }

        // Update is called once per frame
        void Update()
        {
            //regen hp if tower is not under capture
            if ((numOfCapturers == 0) && (TowerInGameHP < TowerHP))
            {
                modifyinghealth(TowerRegenRate * Time.deltaTime);

                //debug showing tower HP
                //Debug.Log(this.name + " HP: " + TowerInGameHP);
            }

            //once HP = 0, notify the scoring and convert the tower
            //from red to blue (captured by blue team)
            if (TowerInGameHP <= 0 && !CapturedByBlue)
            {
                //show which point is captured, change point authority and max out towerHP
                Debug.Log(this.name + " Captured By Blue Team");
                CapturedByBlue = true;
                CapturedByRed = false;

                modifyinghealth(TowerHP);
                //  TowerInGameHP = TowerHP;

                //reference the score script to increase score function
                scoreGameObject.GetComponent<GameScore>().blueScoreUp();
            }

            //from blue to red (captured by red team)
            else if (TowerInGameHP <= 0 && !CapturedByRed)
            {
                //show which point is captured, change point authority and max out towerHP
                Debug.Log(this.name + " Captured By Blue Team");
                CapturedByRed = true;
                CapturedByBlue = false;

                modifyinghealth(TowerHP);
                //  TowerInGameHP = TowerHP;

                //reference the score script to increase score function
                scoreGameObject.GetComponent<GameScore>().redScoreUp();
            }

            //change color when captured by blue
            if (CapturedByBlue)
            {
                var captureRenderer = this.GetComponent<Renderer>();
                captureRenderer.material.SetColor("_Color", Color.blue);
            }

            //else its red
            else
            {
                var captureRenderer = this.GetComponent<Renderer>();
                captureRenderer.material.SetColor("_Color", Color.red);
            }
        }


        public void modifyinghealth(float amount)
        {
            TowerInGameHP += amount;

            Debug.Log("health: tower in game hp:  " + TowerInGameHP);
            //float currenthealthPct = TowerInGameHP /TowerHP;
            //OnHealthPctChangedTower(currenthealthPct);
        }

        //check for player enter
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                //get player's team ID
                float tID = other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();

                //if point belongs to red, it can be captured by blue players
                if (CapturedByRed && (tID == 1))
                {
                    numOfCapturers += 1;
                }

                //if point belongs to blue, it can be captured by red players
                if (CapturedByBlue && (tID == 2))
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
                float tID = other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();

                //if point belongs to red, it can be captured by blue
                if (CapturedByRed && (tID == 1))
                {
                    modifyinghealth(-(TowerCaptureRate * Time.deltaTime));
                    //TowerInGameHP -= TowerCaptureRate * Time.deltaTime;

                }

                //if point belongs to blue, it can be captured by red
                if (CapturedByBlue && (tID == 2))
                {
                    modifyinghealth(-(TowerCaptureRate * Time.deltaTime));
                    //TowerInGameHP -= TowerCaptureRate * Time.deltaTime;
                }


            }
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

    }
}