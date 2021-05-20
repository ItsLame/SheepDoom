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
        public void SetPlayerName(string _name, Color _color)
        {
            playerName.text = _name;
            playerName.color = _color;
        }
    }
}
