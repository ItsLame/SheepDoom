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

        [Header("Team affiliation (1 for Blue, 2 for Red")]
        [SerializeField]
        private float TeamIndex;

        [Header("Player scores")]
        [SyncVar(hook = nameof(SyncPlayerKill))] public float PlayerKills;
        public Text PlayerKillsText;
        [SyncVar(hook = nameof(SyncPlayerDeath))] public float PlayerDeaths;
        public Text PlayerDeathsText;
        [SyncVar(hook = nameof(SyncTowerCapture))] public float TowerCaptures;
        public Text TowerCapturesText;

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

        //increasing methods for kills, deaths
        /*public void increasePlayerKillCount()
        {
            PlayerKills += 1;
            //Debug.Log("Player Kills increased to " + PlayerKills);
            PlayerKillsText.text = PlayerKills.ToString();
        }

        public void increasePlayerDeathCount()
        {
            PlayerDeaths += 1;
            //Debug.Log("Player Deaths increased to " + PlayerDeaths);
            PlayerDeathsText.text = PlayerDeaths.ToString();
        }

        public void increaseCaptureCount()
        {
            TowerCaptures += 1;
        }*/

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


        //start
        /*private void Start()
        {
            if (!hasAuthority) return;
            //set scores
            PlayerKills = 0;
            PlayerDeaths = 0;
            TowerCaptures = 0;

            //get the UI
            PlayerKillsText = GameObject.Find("KText").GetComponent<Text>();
 //                 Debug.Log("playerkillstext found");
            PlayerDeathsText = GameObject.Find("DText").GetComponent<Text>();
 //                 Debug.Log("playerdeathtext found");
            TowerCapturesText = GameObject.Find("CText").GetComponent<Text>();
 //                  Debug.Log("towercapturetext found");

            PlayerKillsText.text = PlayerKills.ToString();
            PlayerDeathsText.text = PlayerDeaths.ToString();
            TowerCapturesText.text = TowerCaptures.ToString();

            //assign player to shop
            Debug.Log("Assigning player to blue shop");
            GameObject shop = GameObject.Find("BlueShop");
            shop.gameObject.GetComponent<Shop>().Player = this.gameObject;
            Debug.Log("Assigned player to blue shop");


            Debug.Log("Assigning player to red shop");
            GameObject shop2 = GameObject.Find("RedShop");
            shop2.gameObject.GetComponent<Shop>().Player = this.gameObject;
            Debug.Log("Assigned player to red shop");


        }*/

        public override void OnStartClient()
        {
            if (!hasAuthority) return;

            PlayerKillsText = GameObject.Find("KText").GetComponent<Text>();
            PlayerDeathsText = GameObject.Find("DText").GetComponent<Text>();
            TowerCapturesText = GameObject.Find("CText").GetComponent<Text>();
            PlayerKillsText.text = PlayerKills.ToString();
            PlayerDeathsText.text = PlayerDeaths.ToString();
            TowerCapturesText.text = TowerCaptures.ToString();

            if(TeamIndex == 1)
            {
                GameObject shop = GameObject.Find("BlueShop");
                shop.gameObject.GetComponent<Shop>().Player = this.gameObject;
            }
            else if(TeamIndex == 2)

            {
                GameObject shop2 = GameObject.Find("RedShop");
                shop2.gameObject.GetComponent<Shop>().Player = this.gameObject;
            }
        }

        public override void OnStartServer()
        {
            PlayerKills = 0;
            PlayerDeaths = 0;
            TowerCaptures = 0;
        }
    }
}
