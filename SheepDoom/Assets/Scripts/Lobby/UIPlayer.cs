using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MirrorBasics
{
    public class UIPlayer : MonoBehaviour
    {
        [SerializeField] Text text;
        Lobby_Player player;

        public void SetPlayer(Lobby_Player player)
        {
            this.player = player;
            //text.text = "Player " + player.playerIndex.ToString();
            text.text = "You";
        }
        
    }
}
