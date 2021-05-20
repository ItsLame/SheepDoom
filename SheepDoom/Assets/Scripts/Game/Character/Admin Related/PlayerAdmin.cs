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
        [SerializeField] [SyncVar(hook = nameof(OnNameUpdate))] private string playerName;
        [SerializeField] [SyncVar(hook = nameof(OnTeamUpdate))] private int TeamIndex;

        [Header("Player scores")]
        [SyncVar(hook = nameof(SyncPlayerKill))] private float PlayerKills;
        public Text PlayerKillsText;
        [SyncVar(hook = nameof(SyncPlayerDeath))] private float PlayerDeaths;
        public Text PlayerDeathsText;
        [SyncVar(hook = nameof(SyncTowerCapture))] private float TowerCaptures;
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
            get { return playerName; }
            set { playerName = value; }
        }

        public float P_playerKills
        {
            get { return PlayerKills; }
            set { PlayerKills = value; }
        }

        public float P_playerDeaths
        {
            get { return PlayerDeaths; }
            set { PlayerDeaths = value; }
        }

        public float P_towerCaptures
        {
            get { return TowerCaptures; }
            set { TowerCaptures = value; }
        }

        #endregion

        [Server]
        public void IncreaseCount(bool _isTower, bool _isKill, bool _isDeath)
        {
            if(_isTower)
                P_towerCaptures += 1;
            else if(_isKill)
                P_playerKills += 1;
            else if(_isDeath)
                P_playerDeaths += 1;
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

        private void OnNameUpdate(string oldValue, string newValue)
        {
            GetComponent<PlayerNameGame>().SetPlayerName(newValue);
        }

        private void OnTeamUpdate(int oldValue, int newValue)
        {
            Color newColor = Color.white;

            if(newValue == 1)
                newColor = Color.blue;
            else if(newValue == 2)
                newColor = Color.red;

            GetComponent<PlayerNameGame>().SetPlayerColor(newColor);
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
            FindMe.instance.P_MyPlayer = this.gameObject;
            
            int _playerTeamID = PlayerObj.instance.GetTeamIndex();
            string _playerName = PlayerObj.instance.GetPlayerName();

            setTeamIndex(_playerTeamID);
            
            CmdSetInfo(_playerTeamID, _playerName);

            if (getTeamIndex() == 1)
                FindMe.instance.P_BlueShop.GetComponent<Shop>().P_shopPlayer = gameObject;
            else if (getTeamIndex() == 2)
                FindMe.instance.P_RedShop.GetComponent<Shop>().P_shopPlayer = gameObject;

            gameObject.GetComponent<AudioListener>().enabled = true;
        }

        public override void OnStartServer()
        {
            PlayerKills = 0;
            PlayerDeaths = 0;
            TowerCaptures = 0;
        }
    }
}
