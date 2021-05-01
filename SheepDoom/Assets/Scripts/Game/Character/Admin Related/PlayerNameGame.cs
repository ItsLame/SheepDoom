using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SheepDoom
{
    public class PlayerNameGame : MonoBehaviour
    {
        [SerializeField] private Text playerName;

        void Update()
        {
            StartCoroutine(SetPlayerName());
        }

        IEnumerator SetPlayerName()
        {
            while (this.GetComponent<PlayerAdmin>().P_playerName == null)
                yield return null;

            playerName.text = this.GetComponent<PlayerAdmin>().P_playerName;
        }

        public Text getPlayerName()
        {
            return playerName;
        }
    }
}
