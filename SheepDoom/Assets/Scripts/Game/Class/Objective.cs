using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public abstract class Objective : NetworkBehaviour
    {
        private GameObject scoreGameObject;
        
        // hp: base/tower hp
        // inGameHP: to be used in game, gonna be the one fluctuating basically
        // captureRate: rate of capture
        // regenRate: regeneration rate if not under capture
        private float hp, captureRate, regenRate;

        [SyncVar] private float inGameHP;

        //captured bools
        [SyncVar] protected bool capturedByBlue;
        [SyncVar] protected bool capturedByRed;

        //logging number to check if base/tower is under capture or not
        private int numOfCapturers;
        private bool giveScoreToCapturers;
        [SerializeField] private bool isBase;

        public event Action<float> OnHealthPctChangedTower = delegate { };

        #region Properties

        protected GameObject P_scoreGameObject
        {
            get{return scoreGameObject;}
            set{scoreGameObject = value;}
        }

        protected float P_hp
        {
            get{return hp;}
            set{hp = value;}
        }
        
        protected virtual float P_inGameHP
        {
            get{return inGameHP;}
            set{inGameHP = value;}
        }

        protected float P_captureRate
        {
            get{return captureRate;}
            set{captureRate = value;}
        }

        protected float P_regenRate
        {
            get{return regenRate;}
            set{regenRate = value;}
        }

        protected virtual bool P_capturedByBlue
        {
            get{return capturedByBlue;}
            set{capturedByRed = value;}
        }

        protected virtual bool P_capturedByRed
        {
            get{return capturedByRed;}
            set{capturedByRed = value;}
        }

        protected int P_numOfCapturers
        {
            get{return numOfCapturers;}
            set{numOfCapturers = value;}
        }

        protected bool P_giveScoreToCapturers
        {
            get{return giveScoreToCapturers;}
            set{giveScoreToCapturers = value;}
        }

        protected bool P_isBase
        {
            get{return isBase;}
            set{isBase = value;}
        }

        #endregion

        private void Start()
        {
            InitObjective();
        }

        protected abstract void InitObjective();

        // would preferably set to 'protected' but PlayerProjectileSettings calls this method
        public void ModifyingHealth(float amount)
        {
            P_inGameHP += amount;

            float currenthealthPct = P_inGameHP / P_hp;
            OnHealthPctChangedTower(currenthealthPct);
        }

        protected void SetTowerColor()
        {
            if (P_capturedByBlue && !P_capturedByRed)
                GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
            else if (P_capturedByRed && !P_capturedByBlue)
                GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        }

        protected virtual void Victory()
        {
            // empty
        }

        protected virtual void Update()
        {
            if (isServer)
            {
                // once HP = 0, notify the scoring and convert the tower
                if (P_inGameHP <= 0 && (!P_capturedByBlue || !P_capturedByRed))
                {
                    //show which point is captured, change point authority and max out towerHP
                    CapturedServer(P_capturedByBlue, P_capturedByRed);
                    RpcUpdateClients(true, false, P_isBase);
                }

                // regen hp if tower is not under capture
                if ((P_numOfCapturers == 0) && (P_inGameHP < P_hp))
                {
                    ModifyingHealth(P_regenRate * Time.deltaTime);
                    RpcUpdateClients(false, true, P_isBase);
                }  
            }

            if(P_isBase)
              //  Debug.Log(this.name + " base ingamehp? "+P_inGameHP);

            if(P_isBase && P_inGameHP <= 10)
                Victory();
        }

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

            if (!isBase)
            {
                P_giveScoreToCapturers = true;
                P_scoreGameObject.GetComponent<GameScore>().ScoreUp(P_capturedByBlue, P_capturedByRed);
            }

            ModifyingHealth(P_hp);
        }

        [ClientRpc]
        protected void RpcUpdateClients(bool _isCapture, bool _isChangeHp, bool _isBase)
        {
            if(_isCapture)
                // deals with syncvar delay
                StartCoroutine(WaitForUpdate(P_capturedByBlue, P_capturedByRed, _isBase));
            else if(_isChangeHp)
                ModifyingHealth(0); // 0 because value from server will sync
        }

        private IEnumerator WaitForUpdate(bool _oldBlue, bool _oldRed, bool _isBase)
        {
            while (P_capturedByBlue == _oldBlue && P_capturedByRed == _oldRed)
                yield return null;
            
            SetTowerColor();
            ModifyingHealth(0); // 0 because value from server will sync
        }

        // check for player enter
        protected virtual void OnTriggerEnter(Collider _collider)
        {
            if (_collider.tag == "Player")
            {
                //get player's team ID
                float tID = _collider.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
                bool isDed = _collider.gameObject.GetComponent<PlayerHealth>().isPlayerDead();
                if (((P_capturedByRed && tID == 1) || (P_capturedByBlue && tID == 2)) && !isDed)
                    P_numOfCapturers += 1;
            }
        }

        protected virtual void OnTriggerStay(Collider _collider)
        {
            if(isServer)
            {
                if (_collider.CompareTag("Player"))
                {
                    // get player teamID
                    float tID = _collider.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
                    // get info of is player dead or alive
                    bool isDed = _collider.gameObject.GetComponent<PlayerHealth>().isPlayerDead();

                    OnStay(_collider, tID);

                    if (((P_capturedByRed && tID == 1) || (P_capturedByBlue && tID == 2)) && !isDed)
                    {
                        ModifyingHealth(-(P_captureRate * Time.deltaTime));
                        RpcUpdateClients(false, true, P_isBase);
                    }
                }
            }
        }

        protected virtual void OnStay(Collider _collider, float _tID)
        {
            // empty
        }

        // check for player exit
        protected virtual void OnTriggerExit(Collider _collider)
        {
            if(isServer)
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
        }

        public override void OnStartServer()
        {
            SetTowerColor();
        }

        public override void OnStartClient()
        {
            SetTowerColor();
        }
    }
}

#region archive

//under update()

/*
// regen hp if base/tower is not under capture
if ((P_numOfCapturers == 0) && (P_inGameHP < P_hp))
{
    ModifyingHealth(P_regenRate * Time.deltaTime);
}

// change color when captured by blue
if (P_capturedByBlue)
{
    Debug.Log("CAPTURED BY BLUE");
    var captureRenderer = P_captureGameObject.GetComponent<Renderer>();
    captureRenderer.material.SetColor("_Color", Color.blue);
}
// else its red
else if (P_capturedByRed)
{
    Debug.Log("CAPTURED BY RED");
    var captureRenderer = P_captureGameObject.GetComponent<Renderer>();
    captureRenderer.material.SetColor("_Color", Color.red);
}
*/

//under onenter()

/*
if (_collider.tag == "Player")
{
    // get player's team ID
    float tID = _collider.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
    //bool isDed = _collider.gameObject.GetComponent<PlayerHealth>().isPlayerDead();

    // if point belongs to red, it can be captured by blue players
    if (P_capturedByRed && (tID == 1))
    {
        numOfCapturers += 1;
    }

    // if point belongs to blue, it can be captured by red players
    if (P_capturedByBlue && (tID == 2))
    {
        numOfCapturers += 1;
    }
}
*/

//under onstay()

/*
if (_collider.CompareTag("Player"))
{
    float tID = _collider.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
    //get info of is player dead or alive
    bool isDed = _collider.gameObject.GetComponent<PlayerHealth>().isPlayerDead();

    OnStay(_collider, tID);

    // if point belongs to red, it can be captured by blue
    if (P_capturedByRed && (tID == 1) && !isDed)
    {
        ModifyingHealth(-(P_captureRate * Time.deltaTime));
    }

    // if point belongs to blue, it can be captured by red
    if (P_capturedByBlue && (tID == 2) && !isDed)
    {
        ModifyingHealth(-(P_captureRate * Time.deltaTime));
    }
}
*/

//under onexit()

/*
if (_collider.CompareTag("Player"))
{
    // get player's team ID
    float tID = _collider.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();

    // if point belongs to red, it can be captured by blue players
    if (P_capturedByRed && (tID == 1))
    {
        P_numOfCapturers -= 1;
    }

    // if point belongs to blue, it can be captured by red players
    if (P_capturedByBlue && (tID == 2))
    {
        P_numOfCapturers -= 1;
    }
}
*/
#endregion