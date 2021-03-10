using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{
    public class MainMenu : NetworkBehaviour
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
            if(isClient)
                playerName.text = Player.player.GetUsername(); // from network behaviour
        }

        // When clicked host button
        public void Host()
        {
            Player.player.HostGame();
        }

        public void HostSuccess(bool success)
        {
            if(success)
            {
                // load scene
                Debug.Log("Congrats!! Host success");
            }
            else
            {
                Debug.Log("Host game failed");
            }
        }

        // When clicked Join button...may change to room listing
        public void Join()
        {
            Player.player.JoinGame(matchID.text.ToUpper());
        }

        public void JoinSuccess(bool success)
        {
            if(success)
            {
                Debug.Log("Congrats!! Join success");
            }
            else
            {
                Debug.Log("Join game failed");
            }
        }
    }
}