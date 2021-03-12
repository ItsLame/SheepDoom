using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;


namespace SheepDoom
{
    public class MainMenu : MonoBehaviour
    {
        public static MainMenu instance;

        [Header("Player profile")]
        [SerializeField] Text playerName;

        [Header("Miscellaneous")]
        [SerializeField] Button optionButton;
        [SerializeField] Button creditsButton;

        [Header("Setup match")]
        [SerializeField] Button joinButton;
        [SerializeField] Button hostButton;
        [SerializeField] InputField matchID;

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
        }

        public void SetPlayerName(string name)
        {
            playerName.text = name;
        }
         
        // When clicked host button
        public void Host()
        {
            Player.player.HostGame();
        }

        // When clicked Join button...may change to room listing
        public void Join()
        {
            Player.player.JoinGame(matchID.text.ToUpper());
        }
    }
}