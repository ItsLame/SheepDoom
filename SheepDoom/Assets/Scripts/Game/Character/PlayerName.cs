using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SheepDoom
{
    public class PlayerName : MonoBehaviour
    {
        [SerializeField]
        private Text playerName;

        void Start()
        {
            StartCoroutine(SetPlayerName());
        }

        private IEnumerator SetPlayerName()
        {
            while (GetComponent<PlayerObj>().GetPlayerName() == null)
                yield return null;
            playerName.text = GetComponent<PlayerObj>().GetPlayerName();
        }
    }
}
