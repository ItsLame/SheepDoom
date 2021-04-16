using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Mirror;

namespace SheepDoom
{
    public class CaptureBaseScript : NetworkBehaviour
    {
        //attach the score gameobject to count the score
        public GameObject scoreGameObject;

        //Base hp counters
        [Space(20)]
        [SerializeField]
        //base hp
        [Tooltip("How much HP the Base has, edit this")]
        private float BaseHP;
        [SerializeField]
        [SyncVar]
        private float BaseInGameHP; //to be used in game, gonna be the one fluctuating basically

        //rate of capture
        [SerializeField]
        private float BaseCaptureRate;

        //regeneration rate if not under capture
        [SerializeField]
        private float BaseRegenRate;

        //captured bools
        [Space(20)]
        [SerializeField]
        [SyncVar]
        private bool CapturedByBlue2;
        [SerializeField]
        [SyncVar]
        private bool CapturedByRed2;
        [SerializeField]
        private int numOfCapturersBase; //logging number to check if Base is under capture or not

        public event Action<float> OnHealthPctChangedTower = delegate { };

        // Start is called before the first frame update
        void Start()
        {
            //set the Base's hp based on the settings
            BaseInGameHP = BaseHP;

            //no one is capturing it at start so put at 0
            numOfCapturersBase = 0;
        }

        // Update is called once per frame
        void Update()
        {
            //regen hp if tower is not under capture
            if ((numOfCapturersBase == 0) && (BaseInGameHP < BaseHP))
            {
                //BaseInGameHP += BaseRegenRate * Time.deltaTime;
                modifyinghealth(BaseRegenRate * Time.deltaTime);
                //debug showing base hp
                //     Debug.Log(this.name + " HP: " + BaseInGameHP);
            }

            //blue team victory when base hp 0
            if (BaseInGameHP <= 0 && !CapturedByBlue2)
            {
                //reference the score script to END THE GAME IN BLUE VICTORY   <------------------------------------------------- GAME END CALL
                scoreGameObject.GetComponent<GameScore>().GameEnd(1);
            }

            //red team victory if base hp 0
            if (BaseInGameHP <= 0 && !CapturedByRed2)
            {
                //reference the score script to END THE GAME IN RED VICTORY   <------------------------------------------------- GAME END CALL
                scoreGameObject.GetComponent<GameScore>().GameEnd(2);
            }

            //change color when captured by blue
            if (CapturedByBlue2)
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
            BaseInGameHP += amount;

            float currenthealthPct = BaseInGameHP / BaseHP;
            OnHealthPctChangedTower(currenthealthPct);
        }

        //check for player enter
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                //get player's team ID
                float tID = other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();

                //if point belongs to red, it can be captured by blue players
                if (CapturedByRed2 && (tID == 1))
                {
                    numOfCapturersBase += 1;
                }

                //if point belongs to blue, it can be captured by red players
                if (CapturedByBlue2 && (tID == 2))
                {
                    numOfCapturersBase += 1;
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
                if (CapturedByRed2 && (tID == 1))
                {
                    modifyinghealth(-(BaseCaptureRate * Time.deltaTime));
                    //TowerInGameHP -= TowerCaptureRate * Time.deltaTime;



                }

                //if point belongs to blue, it can be captured by red
                if (CapturedByBlue2 && (tID == 2))
                {
                    modifyinghealth(-(BaseCaptureRate * Time.deltaTime));
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
                if (CapturedByRed2 && (tID == 1))
                {
                    numOfCapturersBase -= 1;
                }

                //if point belongs to blue, it can be captured by red players
                if (CapturedByBlue2 && (tID == 2))
                {
                    numOfCapturersBase -= 1;
                }
            }
        }

    }
}

