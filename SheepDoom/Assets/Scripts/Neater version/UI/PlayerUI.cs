using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SheepDoom
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private Text playerName;
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(SetPlayerName());
        }

        IEnumerator SetPlayerName()
        {
            while(this.GetComponent<PlayerObj>().GetPlayerName() == null)
                yield return null;

            playerName.text = this.GetComponent<PlayerObj>().GetPlayerName();
        }
    }
}

