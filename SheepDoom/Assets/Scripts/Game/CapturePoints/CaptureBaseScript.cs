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
        }

        [ServerCallback]
        protected override void OnTriggerStay(Collider _collider)
        {
            if (_collider.CompareTag("Player"))
            {
                // get player teamID
                float tID = _collider.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
                // get info of is player dead or alive
                bool isDed = _collider.gameObject.GetComponent<PlayerHealth>().isPlayerDead();

                if(!isDed)
                {
                    //have another condition to check whether base can be captured
                    //if no outer tower has been captured base cant be captured
                    if (P_capturedByBlue && tID == 2 && ScoreGameObject.GetComponent<GameScore>().getBlueScore() < 2)
                    {
                        ModifyingHealth(-(P_captureRate * Time.deltaTime));
                        RpcUpdateClients(false, true, false);
                    }
                    else if(P_capturedByRed && tID == 1 && ScoreGameObject.GetComponent<GameScore>().getRedScore() < 2)
                    {
                        ModifyingHealth(-(P_captureRate * Time.deltaTime));
                        RpcUpdateClients(false, true, false);
                    }
                }
            }
        }

        protected override void Victory()
        {
            // if base owner is red team
            if (P_capturedByRed)
            {
                P_scoreGameObject.GetComponent<GameScore>().GameEnd(1);
            }
            // if base owner is blue team
            if (P_capturedByBlue)
            {
                P_scoreGameObject.GetComponent<GameScore>().GameEnd(2);
            } 
        }
    }
}