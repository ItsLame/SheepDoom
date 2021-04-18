﻿using System.Collections;
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

        //public event Action<float> OnHealthPctChangedTower = delegate { };
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            //set the Base's hp based on the settings
            P_inGameHP = P_hp;

            //no one is capturing it at start so put at 0
            P_numOfCapturers = 0;
        }

        protected override void InitHealth()
        {
            //P_captureGameObject = this.gameObject;
            P_scoreGameObject = ScoreGameObject;
            P_hp = HP;
            P_inGameHP = InGameHP;
            P_captureRate = CaptureRate;
            P_regenRate = RegenRate;
            //CheckCaptureBool();
            P_numOfCapturers = NumOfCapturers;
        }

        // Update is called once per frame
        protected override void Update()
        {
            //regen hp if tower is not under capture
            if ((P_numOfCapturers == 0) && (P_inGameHP < P_hp))
            {
                //P_inGameHP += BaseRegenRate * Time.deltaTime;
                ModifyingHealth(P_regenRate * Time.deltaTime);
                //debug showing base hp
                //     Debug.Log(this.name + " HP: " + P_inGameHP);
            }

            //blue team victory when base hp 0
            if (P_inGameHP <= 0 && !CapturedByBlue)
            {
                //reference the score script to END THE GAME IN BLUE VICTORY   <------------------------------------------------- GAME END CALL
                P_scoreGameObject.GetComponent<GameScore>().GameEnd(1);
            }

            //red team victory if base hp 0
            if (P_inGameHP <= 0 && !CapturedByRed)
            {
                //reference the score script to END THE GAME IN RED VICTORY   <------------------------------------------------- GAME END CALL
                P_scoreGameObject.GetComponent<GameScore>().GameEnd(2);
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

        /*
        public void ModifyingHealth(float amount)
        {
            P_inGameHP += amount;

            float currenthealthPct = P_inGameHP / BaseHP;
            OnHealthPctChangedTower(currenthealthPct);
        }
        */

        //check for player enter
        protected override void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                //get player's team ID
                float tID = other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();

                //if point belongs to red, it can be captured by blue players
                if (CapturedByRed && (tID == 1))
                {
                    P_numOfCapturers += 1;
                }

                //if point belongs to blue, it can be captured by red players
                if (CapturedByBlue && (tID == 2))
                {
                    P_numOfCapturers += 1;
                }
            }
        }

        //for capture hp reduction when staying in area
        protected override void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                float tID = other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();

                //if point belongs to red, it can be captured by blue
                if (CapturedByRed && (tID == 1))
                {
                    ModifyingHealth(-(P_captureRate * Time.deltaTime));
                    //TowerInGameHP -= TowerCaptureRate * Time.deltaTime;
                }

                //if point belongs to blue, it can be captured by red
                if (CapturedByBlue && (tID == 2))
                {
                    ModifyingHealth(-(P_captureRate * Time.deltaTime));
                    //TowerInGameHP -= TowerCaptureRate * Time.deltaTime;
                }
            }
        }

        //check for player exit
        protected override void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                //get player's team ID
                float tID = other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();

                //if point belongs to red, it can be captured by blue players
                if (CapturedByRed && (tID == 1))
                {
                    P_numOfCapturers -= 1;
                }

                //if point belongs to blue, it can be captured by red players
                if (CapturedByBlue && (tID == 2))
                {
                    P_numOfCapturers -= 1;
                }
            }
        }

    }
}

