using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public abstract class Objective : NetworkBehaviour
    {
        public AudioClip Sound1;
        public AudioClip Sound2;
        public AudioClip Sound3;
        public AudioSource ObjectiveSound;
        /*
        public bool capturing = false;
        public bool hasPlayedSound = false;
        public float SoundInterval;
        [SyncVar] public float SoundTimer;*/

        [Header("--- Reference Objects ---")]
        [SerializeField] private GameObject scoreGameObject;
        [SerializeField] protected GameObject gameStatus;
        [SerializeField] private bool isBase;

        [Header("--- Capture Point/Base Stats ---")]
        [SerializeField] private float hp;
        [SerializeField] private float captureRate;
        [SerializeField] private float regenRate;

        //logging number to check if base/tower is under capture or not
        [Header("--- Capture Point/Base Tracker ---")]
        [SerializeField] [SyncVar] private float inGameHP;
        [SerializeField] private int numOfCapturers;
        //captured bools
        [SyncVar] protected bool capturedByBlue;
        [SyncVar] protected bool capturedByRed;

        // hp: base/tower hp
        // inGameHP: to be used in game, gonna be the one fluctuating basically
        // captureRate: rate of capture
        // regenRate: regeneration rate if not under capture
        private bool giveScoreToCapturers;

        public event Action<float> OnHealthPctChangedTower = delegate { };

        #region Properties

        protected GameObject P_scoreGameObject
        {
            get { return scoreGameObject; }
            set { scoreGameObject = value; }
        }

        protected float P_hp
        {
            get { return hp; }
            set { hp = value; }
        }

        protected virtual float P_inGameHP
        {
            get { return inGameHP; }
            set { inGameHP = value; }
        }

        protected float P_captureRate
        {
            get { return captureRate; }
            set { captureRate = value; }
        }

        protected float P_regenRate
        {
            get { return regenRate; }
            set { regenRate = value; }
        }

        protected virtual bool P_capturedByBlue
        {
            get { return capturedByBlue; }
            set { capturedByRed = value; }
        }

        protected virtual bool P_capturedByRed
        {
            get { return capturedByRed; }
            set { capturedByRed = value; }
        }

        public int P_numOfCapturers
        {
            get { return numOfCapturers; }
            set { numOfCapturers = value; }
        }

        protected bool P_giveScoreToCapturers
        {
            get { return giveScoreToCapturers; }
            set { giveScoreToCapturers = value; }
        }

        protected bool P_isBase
        {
            get { return isBase; }
            set { isBase = value; }
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
            {
                GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.blue * 1.5f);
            }

            else if (P_capturedByRed && !P_capturedByBlue)
            {
                GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red * 1.5f);
            }
        }

        protected virtual void Victory()
        {
            // empty
        }

        [ClientRpc]
        public void playCapturedSound()
        {
            ObjectiveSound.clip = Sound2;
            ObjectiveSound.PlayOneShot(ObjectiveSound.clip, ObjectiveSound.volume);
        }
        /*
        [ClientRpc]
        public void playCapturingSound()
        {
            ObjectiveSound.clip = Sound1;
            ObjectiveSound.PlayOneShot(ObjectiveSound.clip, ObjectiveSound.volume);
            Invoke("SetPlayFalse", 3f);
        }

        public void setPlayFalse()
        {
            hasPlayedSound = false;
        }*/

        protected virtual void Update()
        {
            if (isServer)
            {
                /*
                if (capturing && hasPlayedSound == false)
                {
                    playCapturingSound();
                    hasPlayedSound = true;
                }*/

                // once HP = 0, notify the scoring and convert the tower
                if (P_inGameHP <= 0 && (!P_capturedByBlue || !P_capturedByRed) && !P_isBase)
                {
                    //show which point is captured, change point authority and max out towerHP
                    CapturedServer(P_capturedByBlue, P_capturedByRed);
                    RpcUpdateClients(true, false, false);

                    playCapturedSound();
                }

                // regen hp if tower is not under capture
                if (P_numOfCapturers == 0 && P_inGameHP < P_hp)
                {
                    if (P_isBase)
                        OpenBaseAnim();

                  //  capturing = false;
                    ModifyingHealth(P_regenRate * Time.deltaTime);
                    RpcUpdateClients(false, true, false);
                }

                if (P_isBase && P_inGameHP <= 0 && gameStatus != null)
                {
                    if (!gameStatus.GetComponent<GameStatus>().P_gameEnded)
                    {
                        playCapturedSound();
                        Victory();
                        return;
                    }
                }
            }
        }

        protected virtual void OpenBaseAnim()
        {
            // empty
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

            P_numOfCapturers = 0;

            if (!P_isBase)
            {
                P_giveScoreToCapturers = true;
                P_scoreGameObject.GetComponent<GameScore>().ScoreUp(P_capturedByBlue, P_capturedByRed);
            }

            ModifyingHealth(P_hp);
        }

        [ClientRpc]
        protected void RpcUpdateClients(bool _isCapture, bool _isChangeHp, bool _isEndGame)
        {
            if (_isCapture)
                StartCoroutine(WaitForUpdate(P_capturedByBlue, P_capturedByRed)); // deals with syncvar delay
            else if (_isChangeHp)
                ModifyingHealth(0); // 0 because value from server will sync
        }

        private IEnumerator WaitForUpdate(bool _oldBlue, bool _oldRed)
        {
            while (P_capturedByBlue == _oldBlue && P_capturedByRed == _oldRed)
                yield return null;

            SetTowerColor();
            ModifyingHealth(0); // 0 because value from server will sync
        }

        // check for player enter
        [ServerCallback]
        protected virtual void OnTriggerEnter(Collider _collider)
        {
            if (_collider.tag == "Player")
            {
                //get player's team ID
                float tID = _collider.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
                bool isDed = _collider.gameObject.GetComponent<PlayerHealth>().isPlayerDead();
                if (((P_capturedByRed && tID == 1) || (P_capturedByBlue && tID == 2)) && !isDed)
                    P_numOfCapturers = P_numOfCapturers + 1;
            }
        }

        [ServerCallback]
        protected virtual void OnTriggerStay(Collider _collider)
        {
            if (_collider.CompareTag("Player") && !P_isBase)
            {
                // get player teamID
                float tID = _collider.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
                // get info of is player dead or alive
                bool isDed = _collider.gameObject.GetComponent<PlayerHealth>().isPlayerDead();

                OnStay(_collider, tID);

                if (((P_capturedByRed && tID == 1) || (P_capturedByBlue && tID == 2)) && !isDed)
                {
                    //decrease point normally if not base
                    ModifyingHealth(-(P_captureRate * Time.deltaTime));
                    RpcUpdateClients(false, true, false);

                  //  capturing = true;
                }
            }
        }

        protected virtual void OnStay(Collider _collider, float _tID)
        {
            // empty
        }

        [ServerCallback]
        // check for player exit
        protected virtual void OnTriggerExit(Collider _collider)
        {
            if (_collider.CompareTag("Player"))
            {
                //get player's team ID
                float tID = _collider.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
                bool isDed = _collider.gameObject.GetComponent<PlayerHealth>().isPlayerDead();
                if (((P_capturedByRed && tID == 1) || (P_capturedByBlue && tID == 2)) && !isDed)
                    P_numOfCapturers = P_numOfCapturers - 1;
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
