using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SheepDoom
{
    public class PlayerNameGame : MonoBehaviour
    {
        [SerializeField] private Text playerName;
        public void SetPlayerName(string _name)
        {
            playerName.text = _name;
        }
    }
}
