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
        [Header("Player Info")]
        [SerializeField] [SyncVar] private float charID;
        [SerializeField] [SyncVar] private string playerName;
        [SerializeField] [SyncVar] private int TeamIndex;
        //[Header("Team affiliation (1 for Blue, 2 for Red")]

        [Header("Player scores")]
        [SyncVar(hook = nameof(SyncPlayerKill))] public float PlayerKills;
        public Text PlayerKillsText;
        [SyncVar(hook = nameof(SyncPlayerDeath))] public float PlayerDeaths;
        public Text PlayerDeathsText;
        [SyncVar(hook = nameof(SyncTowerCapture))] public float TowerCaptures;
        public Text TowerCapturesText;
        private GameObject shop;

        #region Accessors

        //accessor method for team index
        public float getTeamIndex()
        {
            return TeamIndex;
        }

        //set method for teamindex
        public void setTeamIndex(int value)
        {
            TeamIndex = value;
        }

        public float getCharID()
        {
            return charID;
        }

        public string P_playerName
        {
            get{return playerName;}
            set{playerName = value;}
        }

        #endregion

        [Server]
        public void IncreaseCount(bool _isTower, bool _isKill, bool _isDeath)
        {
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

        [Command]
        private void CmdSetInfo(int _teamIndex, string _playerName)
        {
            setTeamIndex(_teamIndex);
            P_playerName = _playerName;
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
            
            int _playerTeamID = PlayerObj.instance.GetTeamIndex();
            string _playerName = PlayerObj.instance.GetPlayerName();

            setTeamIndex(_playerTeamID);
            P_playerName = _playerName;
            
            CmdSetInfo(_playerTeamID, PlayerObj.instance.GetPlayerName());

            if (getTeamIndex() == 1)
                FindMe.instance.P_BlueShop.GetComponent<Shop>().P_shopPlayer = gameObject;
            else if (getTeamIndex() == 2)
                FindMe.instance.P_RedShop.GetComponent<Shop>().P_shopPlayer = gameObject;
        }

        public override void OnStartServer()
        {
            PlayerKills = 0;
            PlayerDeaths = 0;
            TowerCaptures = 0;
        }
    }
}
