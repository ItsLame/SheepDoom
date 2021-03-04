using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MirrorBasics {
    public class UI_LobbyScript : MonoBehaviour
    {
        //create instance for singleton
        public static UI_LobbyScript instance;

        [Header("Host Join")]
        //getting the buttons / fields for controlling
        [SerializeField] InputField joinMatchInput;
        [SerializeField] Button joinButton;
        [SerializeField] Button hostButton;

        //canvas for overlay
        [SerializeField] Canvas lobbyCanvas;

        [Header("Lobby")]
        [SerializeField] Transform UIPlayerParent;
        [SerializeField] GameObject UIPlayerPrefab;
        [SerializeField] Text matchIDText;

        [Header("Game Lobby (Hero Selection)")]
        //canvas
        [SerializeField] public Canvas gameLobbyCanvas;
        //text
        [SerializeField] public Text selectionTimerText;
        //lock-in button
        [SerializeField] public Button lockinButton;

        [Header("For Debugging Purposes")]
        //for debugging purposes
        [SerializeField] public GameObject forceStartDebug;

        //singleton UI instance on start
        void Start()
        {
            instance = this;
        }

        //Hosting 
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

        public void HostSuccess (bool success, string matchID)
        {
            if (success)
            {
                lobbyCanvas.enabled = true;
                SpawnPlayerPrefab(Lobby_Player.localPlayer);
                matchIDText.text = matchID;
                //so that only hosts have access to force start button
                forceStartDebug.gameObject.SetActive(true);
            }

            //if host / join fail, re-enable the buttons
            else
            {
                joinMatchInput.interactable = true;
                joinButton.interactable = true;
                hostButton.interactable = true;
            }
        }

        //Joining
        public void Join()
        {
            Debug.Log("Join Room Button Clicked");

            //disable join/host when either is clicked
            joinMatchInput.interactable = false;
            joinButton.interactable = false;
            hostButton.interactable = false;

            Lobby_Player.localPlayer.JoinGame(joinMatchInput.text.ToUpper());
        }

        public void JoinSuccess (bool success, string matchID)
        {
            if (success)
            {
                lobbyCanvas.enabled = true;
                SpawnPlayerPrefab(Lobby_Player.localPlayer);
                matchIDText.text = matchID;
            }
            //if host / join fail, re-enable the buttons
            else
            {
                joinMatchInput.interactable = true;
                joinButton.interactable = true;
                hostButton.interactable = true;
            }
        }

        //function for force start button
        public void ForceStartDebug ()
        {
            Lobby_Player.localPlayer.StartGame();
        }

        public void SpawnPlayerPrefab(Lobby_Player player) 
        {
            GameObject newUIPlayer = Instantiate(UIPlayerPrefab, UIPlayerParent);
            newUIPlayer.GetComponent<UIPlayer>().SetPlayer(player);
            newUIPlayer.transform.SetSiblingIndex(player.playerIndex - 1);
        }
    }
}
