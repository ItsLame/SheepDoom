using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

namespace SheepDoom
{
    public class PlayerAdmin : NetworkBehaviour
    {
        // Mainly player attributes 
        [Header("PlayerID for skills")]
        [SerializeField]
        [SyncVar] private float charID;

        [Header("Team affiliation (1 for Blue, 2 for Red")]
        [SerializeField]
        [SyncVar] private float TeamIndex;

        [Header("Player scores")]
        [SyncVar(hook = nameof(SyncPlayerKill))] public float PlayerKills;
        public Text PlayerKillsText;
        [SyncVar(hook = nameof(SyncPlayerDeath))] public float PlayerDeaths;
        public Text PlayerDeathsText;
        [SyncVar(hook = nameof(SyncTowerCapture))] public float TowerCaptures;
        public Text TowerCapturesText;

        private GameObject shop;

        //accessor method for team index
        public float getTeamIndex()
        {
            return TeamIndex;
        }

        //set method for teamindex
        public void setTeamIndex(float value)
        {
            TeamIndex = value;
        }

        public float getCharID()
        {
            return charID;
        }

        [Server]
        public void IncreaseCount(bool _isTower, bool _isKill, bool _isDeath)
        {
            Debug.Log("IncreaseCount called: " + _isTower + ", " + _isKill + ", " + _isDeath);
            if(_isTower)
                TowerCaptures += 1;
            else if(_isKill)
                PlayerKills += 1;
            else if(_isDeath)
                PlayerDeaths += 1;
        }

        private void SyncTowerCapture(float oldValue, float newValue)
        {
            if (hasAuthority)
                TowerCapturesText.text = newValue.ToString();
        }

        private void SyncPlayerKill(float oldValue, float newValue)
        {
            if (hasAuthority)
                PlayerKillsText.text = newValue.ToString();
        }

        private void SyncPlayerDeath(float oldValue, float newValue)
        {
            if (hasAuthority)
                PlayerDeathsText.text = newValue.ToString();
        }

        public override void OnStartClient()
        {
            if (!hasAuthority) return;
            PlayerKillsText = FindMe.instance.P_K.GetComponent<Text>();
            PlayerDeathsText = FindMe.instance.P_D.GetComponent<Text>();
            TowerCapturesText = FindMe.instance.P_C.GetComponent<Text>();
            PlayerKillsText.text = PlayerKills.ToString();
            PlayerDeathsText.text = PlayerDeaths.ToString();
            TowerCapturesText.text = TowerCaptures.ToString();

            //if (gameObject.layer == 8)
            if (PlayerObj.instance.GetTeamIndex() == 1)
                shop = FindMe.instance.P_BlueShop;
            //else if (gameObject.layer == 9)
            else if (PlayerObj.instance.GetTeamIndex() == 2)
                shop = FindMe.instance.P_RedShop;
            shop.GetComponent<Shop>().SetPlayer(gameObject);
        }

        public override void OnStartServer()
        {
            PlayerKills = 0;
            PlayerDeaths = 0;
            TowerCaptures = 0;
        }
    }
}
