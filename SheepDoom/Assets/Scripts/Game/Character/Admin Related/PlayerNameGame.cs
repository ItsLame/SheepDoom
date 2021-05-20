using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SheepDoom
{
    public class PlayerNameGame : MonoBehaviour
    {
        //[SerializeField] private Text playerName;
        [SerializeField] TextMeshProUGUI playerName;
        public void SetPlayerName(string _name)
        {
            playerName.text = _name;
        }

        public void SetPlayerColor(Color _color)
        {
            playerName.color = _color;
        }
    }
}
