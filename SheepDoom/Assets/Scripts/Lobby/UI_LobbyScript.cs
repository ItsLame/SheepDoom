using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MirrorBasics {
    public class UI_LobbyScript : MonoBehaviour
    {
        //create instance for singleton
        public static UI_LobbyScript instance;

        //getting the buttons / fields for controlling
        [SerializeField] InputField joinMatchInput;
        [SerializeField] Button joinButton;
        [SerializeField] Button hostButton;
        [SerializeField] Button lockinButton;
        //canvas for overlay
        [SerializeField] Canvas lobbyCanvas;
        [SerializeField] Canvas gameLobbyCanvas;
        //text
        [SerializeField] Text selectionTimerText;
        //for debugging purposes
        [SerializeField] Button forceStartDebug;
        //singleton UI instance on start
        void Start()
        {
            instance = this;
        }

        void Update()
        {
            if(gameLobbyCanvas.enabled == true && Lobby_Player.localPlayer.selectionTimerZero == false)
            {
                Lobby_Player.localPlayer.selectionTimer -= Time.deltaTime;
                selectionTimerText.text = Lobby_Player.localPlayer.selectionTimer.ToString("f0");
                
                if(Lobby_Player.localPlayer.selectionTimer <= 0)
                {
                    selectionTimerText.text = "Starting Game...";
                    lockinButton.interactable = false;
                    Lobby_Player.localPlayer.selectionTimerZero = true;
                }
            }
            else if(gameLobbyCanvas.enabled == false)
            {
                Lobby_Player.localPlayer.selectionTimer = 5.0f;
                selectionTimerText.text = Lobby_Player.localPlayer.selectionTimer.ToString("f0");
            }
        }

        //UI hosting function to be attached to host button
        public void Host()
        {
            Debug.Log("Host Room Button Clicked");

            //disable join/host when either is clicked
            joinMatchInput.interactable = false;
            joinButton.interactable = false;
            hostButton.interactable = false;

            //connects UI to player
            Lobby_Player.localPlayer.HostGame ();
        }

        public void HostSuccess (bool success)
        {
            if (success)
            {
                lobbyCanvas.enabled = true;
            }

            //if host / join fail, re-enable the buttons
            else
            {
                joinMatchInput.interactable = true;
                joinButton.interactable = true;
                hostButton.interactable = true;
            }
        }

        public void Join()
        {
            Debug.Log("Join Room Button Clicked");

            //disable join/host when either is clicked
            joinMatchInput.interactable = false;
            joinButton.interactable = false;
            hostButton.interactable = false;

            Lobby_Player.localPlayer.JoinGame(joinMatchInput.text);
        }

        public void JoinSuccess (bool success)
        {
            if (success)
            {
                lobbyCanvas.enabled = true;
            }

            //if host / join fail, re-enable the buttons
            else
            {
                joinMatchInput.interactable = true;
                joinButton.interactable = true;
                hostButton.interactable = true;
            }
        }

        public void ForceStartDebug ()
        {
            gameLobbyCanvas.enabled = true;
        }
    }
}
