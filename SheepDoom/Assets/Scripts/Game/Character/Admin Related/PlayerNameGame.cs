using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SheepDoom
{
    public class PlayerNameGame : MonoBehaviour
    {
        [SerializeField] private Text playerName;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(SetPlayerName());
        }

        IEnumerator SetPlayerName()
        {
            while (this.GetComponent<PlayerAdmin>().P_playerName == null)
                yield return null;

            playerName.text = this.GetComponent<PlayerAdmin>().P_playerName;
            Debug.Log("Scoreboard: playerName:" + playerName.text);
        }

        public Text getPlayerName()
        {
            return playerName;
        }
    }
}
