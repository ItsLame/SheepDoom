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
        //    [SyncVar]
        public float PlayerKills;
        public Text PlayerKillsText;
        //    [SyncVar]
        public float PlayerDeaths;
        public Text PlayerDeathsText;
        //    [SyncVar]
        public float TowerCaptures;
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
        [TargetRpc]
        public void increasePlayerKillCount()
        {
            PlayerKills += 1;
            Debug.Log("Player Kills increased to " + PlayerKills);
            PlayerKillsText.text = PlayerKills.ToString();
        }

        [TargetRpc]
        public void increasePlayerDeathCount()
        {
            PlayerDeaths += 1;
            Debug.Log("Player Deaths increased to " + PlayerDeaths);
            PlayerDeathsText.text = PlayerDeaths.ToString();
        }

        [TargetRpc]
        public void increaseCaptureCount()
        {
            TowerCaptures += 1;
            Debug.Log("Captures increased to " + TowerCaptures);
            TowerCapturesText.text = TowerCaptures.ToString();
        }

        //start
        private void Start()
        {
            if (!hasAuthority) return;
            //set scores
            PlayerKills = 0;
            PlayerDeaths = 0;
            TowerCaptures = 0;

            //get the UI
            PlayerKillsText = GameObject.Find("KText").GetComponent<Text>();
            //       Debug.Log("playerkillstext found");
            PlayerDeathsText = GameObject.Find("DText").GetComponent<Text>();
            //       Debug.Log("playerdeathtext found");
            TowerCapturesText = GameObject.Find("CText").GetComponent<Text>();
            //       Debug.Log("towercapturetext found");

            PlayerKillsText.text = PlayerKills.ToString();
            PlayerDeathsText.text = PlayerDeaths.ToString();
            TowerCapturesText.text = TowerCaptures.ToString();

            //assign player to shop
            float playerTeam = getTeamIndex();
            if (playerTeam == 1)
            {
                Debug.Log("Assigning player to blue shop");
                GameObject shop = GameObject.Find("BlueShop");
                shop.gameObject.GetComponent<Shop>().Player = this.gameObject;
                Debug.Log("Assigned player to blue shop");
            }

            else
            {
                Debug.Log("Assigning player to red shop");
                GameObject shop = GameObject.Find("RedShop");
                shop.gameObject.GetComponent<Shop>().Player = this.gameObject;
                Debug.Log("Assigned player to red shop");
            }

        }
    }
}
