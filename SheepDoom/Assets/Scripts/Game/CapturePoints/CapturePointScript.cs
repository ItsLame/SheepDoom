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
        [Header("--- Which Team? ---")]
        [SerializeField] [SyncVar] private bool CapturedByBlue;
        [SerializeField] [SyncVar] private bool CapturedByRed;

        protected override bool P_capturedByBlue { get => CapturedByBlue; set => CapturedByBlue = value; }
        protected override bool P_capturedByRed { get => CapturedByRed; set => CapturedByRed = value; }
        //protected override float P_inGameHP { get => InGameHP; set => InGameHP = value; }

        protected override void InitObjective()
        {
            // set the Tower's hp based on the settings
            P_inGameHP = P_hp;

            // no one is capturing it at start so put at 0
            P_numOfCapturers = 0;

            // this is tower's script
            P_isBase = false;   
        }

        protected override void OnStay(Collider other, float tID)
        {
            // increase the player score when tower is captured
            if (P_giveScoreToCapturers)
            {
                if ((P_capturedByBlue && tID == 1) || (P_capturedByRed && tID == 2))
                {
                    other.GetComponent<PlayerAdmin>().IncreaseCount(true, false, false);
                    P_giveScoreToCapturers = false;
                }
            }
        }

        public float getNumOfCapturers()
        {
            float x =  P_numOfCapturers;
            return x;
        }
    }
}

#region archive

/*
P_scoreGameObject = ScoreGameObject;
P_hp = HP;
P_inGameHP = InGameHP;
P_captureRate = CaptureRate;
P_regenRate = RegenRate;
P_numOfCapturers = NumOfCapturers;
P_giveScoreToCapturers = GiveScoreToCapturers;

// set the Tower's hp based on the settings
P_inGameHP = P_hp;
*/

//attach the score gameobject to count the score
//[Header("Scoreboard")]
//public GameObject ScoreGameObject;

//[Header("Capture Point Stats")]
//tower hp counters
//[Space(20)]
//base hp
// [Tooltip("How much HP the tower has, edit this")]
//[SerializeField] private float HP;
//[SyncVar] [SerializeField] private float InGameHP; //to be used in game, gonna be the one fluctuating basically

//rate of capture
//[SerializeField] private float CaptureRate;

//regeneration rate if not under capture
//[SerializeField] private float RegenRate;

//[Header("Capture Point Capture and Scoring Bools")]
//captured bools
//[Space(20)]

//private int NumOfCapturers; //logging number to check if tower is under capture or not

//scoring bools
//[SerializeField] 
//private bool GiveScoreToCapturers = false;

//P_inGameHP = P_hp; // set the tower's hp based on the settings
//P_numOfCapturers = 0; // no one is capturing it at start so put at 0

// Update is called once per frame
/*
protected override void Update()
{
    if (isServer)
    {
        // once HP = 0, notify the scoring and convert the tower
        if (P_inGameHP <= 0 && (!P_capturedByBlue || !P_capturedByRed))
        {
            //show which point is captured, change point authority and max out towerHP
            CapturedServer(P_capturedByBlue, P_capturedByRed);
            RpcUpdateClients(true, false);
        }

        // regen hp if tower is not under capture
        if ((P_numOfCapturers == 0) && (P_inGameHP < P_hp))
        {
            ModifyingHealth(P_regenRate * Time.deltaTime);
            RpcUpdateClients(false, true);
        }  
    }
}
*/

// causes a syncvar delay on client
/*
private void CapturedServer(bool _byBlue, bool _byRed)
{
    if (!_byBlue && _byRed)
    {
        P_capturedByBlue = true;
        P_capturedByRed = false;
    }
    else if (!_byRed && _byBlue)
    {
        P_capturedByRed = true;
        P_capturedByBlue = false;
    }
    SetTowerColor();
    GiveScoreToCapturers = true;
    P_scoreGameObject.GetComponent<GameScore>().ScoreUp(P_capturedByBlue, P_capturedByRed);
    ModifyingHealth(P_hp);
}
*/

/*
[ClientRpc]
private void RpcUpdateClients(bool _isCapture, bool _isChangeHp)
{
    if(_isCapture)
        // deals with syncvar delay
        StartCoroutine(WaitForUpdate(P_capturedByBlue, P_capturedByRed));
    else if(_isChangeHp)
        ModifyingHealth(0); // 0 because value from server will sync
}

private IEnumerator WaitForUpdate(bool _oldBlue, bool _oldRed)
{
    while (P_capturedByBlue == _oldBlue && P_capturedByRed == _oldRed)
        yield return null;
    SetTowerColor();
    ModifyingHealth(0); // 0 because value from server will sync
}*/

// check for player enter
// runs on server only
//[Server]
/*
protected override void OnTriggerEnter(Collider other)
{
    if(isServer)
        base.OnTriggerEnter(other);
}*/

// for capture hp reduction when staying in area
//[Server]
/*
protected override void OnTriggerStay(Collider other)
{
    
}
*/

/*
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
*/

/*
Debug.Log("--- START INIT ---");
Debug.Log(this.name + " blue? " + P_capturedByBlue);
Debug.Log(this.name + " P_blue? " + P_capturedByBlue);
Debug.Log(this.name + " red? " + CapturedByRed);
Debug.Log(this.name + " P_red? " + P_capturedByRed);
Debug.Log("--- END INIT ---");
*/

/*
Debug.Log("--- START START+ ---");
Debug.Log(this.name + " hp? " + HP);
Debug.Log(this.name + " P_hp? " + P_hp);
Debug.Log(this.name + " P_ingamehp? " + P_inGameHP);
Debug.Log(this.name + " CapturedByBlue? " + CapturedByBlue);
Debug.Log(this.name + " P_capturedByBlue? " + P_capturedByBlue);
Debug.Log(this.name + " CapturedByRed? " + CapturedByRed);
Debug.Log(this.name + " P_capturedByRed? " + P_capturedByRed);
Debug.Log("--- END START+ ---");
*/

/*[TargetRpc]
void TargetUpdateClient(NetworkConnection conn, GameObject _player)
{
    _player.GetComponent<PlayerAdmin>().increaseCaptureCount();
}*/

/*
public void ModifyingHealth(float amount)
{
    if(isServer) P_inGameHP += amount;

    // Debug.Log("health: tower in game hp:  " + P_inGameHP);
    float currenthealthPct = P_inGameHP / P_hp;
    OnHealthPctChangedTower(currenthealthPct);
}
*/

// from onenter()

/*
if (other.tag == "Player")
{
    //get player's team ID
    float tID = other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
    bool isDed = other.gameObject.GetComponent<PlayerHealth>().isPlayerDead();
    if (((P_capturedByRed && tID == 1) || (P_capturedByBlue && tID == 2)) && !isDed)
        P_numOfCapturers += 1;
}
*/

// from onstay()

/*if (SDNetworkManager.LocalPlayersNetId.TryGetValue(other.GetComponent<PlayerObj>().ci.GetComponent<NetworkIdentity>(), out NetworkConnection conn))
{
    TargetUpdateClient(conn, other.gameObject);
    }
    if (P_capturedByRed)
    {
        GiveScoreToRedPlayers_Target(tID, other.gameObject);
        GiveScoreToCapturers = false;
    }

    //else blue  team captured red point, give score to blue
    if (P_capturedByBlue)
    {
        GiveScoreToBluePlayers_Target(tID, other.gameObject);
        GiveScoreToCapturers = false;
}*/

// from onexit()

/*
if (other.CompareTag("Player"))
{
    //get player's team ID
    float tID = other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
    bool isDed = other.gameObject.GetComponent<PlayerHealth>().isPlayerDead();
    if (((P_capturedByRed && tID == 1) || (P_capturedByBlue && tID == 2)) && !isDed)
        P_numOfCapturers -= 1;
}*/

/*public void increasePlayerCaptureScore(GameObject player)
{
    player.gameObject.GetComponent<PlayerAdmin>().increaseCaptureCount();
}*/

#endregion