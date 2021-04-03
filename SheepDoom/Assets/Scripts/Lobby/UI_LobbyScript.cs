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
        [SerializeField] Transform UIPlayerParentTeam1;
        [SerializeField] Transform UIPlayerParentTeam2;
        [SerializeField] public GameObject UIPlayerPrefab; // should be in Lobby_Player
        [SerializeField] public GameObject MoveToTeam1Btn;
        [SerializeField] public GameObject MoveToTeam2Btn;
        [SerializeField] Text matchIDText;
        [SerializeField] public Button startButton;
        [SerializeField] public Button readyButton;

        [Header("Game Lobby (Hero Selection)")]
        //canvas
        [SerializeField] public Canvas gameLobbyCanvas;
        //text
        [SerializeField] public Text selectionTimerText;
        //lock-in button
        [SerializeField] public Button lockinButton;

        [Header("For Debugging Purposes")]
        //for debugging purposes
        [SerializeField] bool isForceStartDebug = false;
        [SerializeField] public GameObject forceStartDebug;
        [SerializeField] public Text debugText;

        //[SerializeField] public GameObject newUIPlayer;
        //private int countPlayers = 0;

        //singleton UI instance on start
        void Start()
        {
            instance = this;
            
            //toggle isforcestart bool on start to true (for debug)
            isForceStartDebug = true;
        }

        void Update()
        {
            //so that only hosts have access to force start button
            if(Lobby_Player.localPlayer?.isRoomOwner == true)
            {
                if(isForceStartDebug == true)
                {
                    forceStartDebug.gameObject.SetActive(true);
                }
                else if(isForceStartDebug == false)
                {
                    forceStartDebug.gameObject.SetActive(false);
                }
            }
            /*else if(Lobby_Player.localPlayer?.isRoomOwner == false)
            {
                forceStartDebug.gameObject.SetActive(false);
                startButton.gameObject.SetActive(false);
                readyButton.gameObject.SetActive(true);
            }*/

            if(Lobby_Player.localPlayer?.teamIndex == 1)
            {
                MoveToTeam2Btn.SetActive(true);
                MoveToTeam1Btn.SetActive(false);
            }
            else if (Lobby_Player.localPlayer?.teamIndex == 2)
            {
                MoveToTeam1Btn.SetActive(true);
                MoveToTeam2Btn.SetActive(false);
            }
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

                startButton.gameObject.SetActive(true);
                readyButton.gameObject.SetActive(false);
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
                
                startButton.gameObject.SetActive(false);
                readyButton.gameObject.SetActive(true);
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
        public void ForceStartDebug()
        {
            Debug.Log("Force Start Button Pressed");
            Lobby_Player.localPlayer.StartGame("Force Start");
        }

        //function for start button
        public void StartGamePressed()
        {
            Debug.Log("Start Button Pressed");
            Lobby_Player.localPlayer.StartGame("Normal Start");
        }

        public void ReadyGamePressed()
        {
            Debug.Log("Ready Button Pressed");
            Lobby_Player.localPlayer.ReadyGame();
        }

        // switching team
        public void SwitchTeam()
        {
            if (MoveToTeam2Btn.activeSelf)
            {
                string toTeam2 = "team2";
                Lobby_Player.localPlayer.SwitchTeam(UIPlayerParentTeam2, toTeam2);
            }
            else if (MoveToTeam1Btn.activeSelf)
            {
                string toTeam1 = "team1";
                Lobby_Player.localPlayer.SwitchTeam(UIPlayerParentTeam1, toTeam1);
            }
        }

        //to spawn players in lobby
        public void SpawnPlayerPrefab(Lobby_Player player) 
        {
            if(player.teamIndex == 1)
            {
                GameObject newUIPlayer = player.gameObject;
                GameObject playerUIGameObject = newUIPlayer.transform.GetChild(0).gameObject;
                newUIPlayer.transform.SetSiblingIndex(player.playerIndex - 1);
                playerUIGameObject.SetActive(true);
                player.gameObject.transform.SetParent(UIPlayerParentTeam1);
                player.GetComponent<UIPlayer>().SetPlayer(player);
            }
            else if (player.teamIndex == 2)
            {
                GameObject newUIPlayer = player.gameObject;
                GameObject playerUIGameObject = newUIPlayer.transform.GetChild(0).gameObject;
                newUIPlayer.transform.SetSiblingIndex(player.playerIndex - 1);
                playerUIGameObject.SetActive(true);
                player.gameObject.transform.SetParent(UIPlayerParentTeam2);
                player.GetComponent<UIPlayer>().SetPlayer(player);
            }
        }
    }
}

// ------------- ARCHIVE ------------- //

//GameObject newUIPlayer = Instantiate(UIPlayerPrefab, UIPlayerParentTeam2);
                //newUIPlayer.GetComponent<UIPlayer>().SetPlayer(player);
                //newUIPlayer.transform.SetSiblingIndex(player.playerIndex - 1); 

/* //when switch button is pressed
        public void SwitchTeam()
        {
            if(MoveToTeam2Btn.activeSelf)
            {
                Debug.Log("MoveToTeam2Btn pressed!");
                Lobby_Player.localPlayer.SwitchTeam(UIPlayerParentTeam2);
            }
            else if(MoveToTeam1Btn.activeSelf)
            {
                Debug.Log("MoveToTeam1Btn pressed!");
                Lobby_Player.localPlayer.SwitchTeam(UIPlayerParentTeam1);
            }
        }

        //to know which team new client belong to
        /*public void SwitchToTeam2()
        {
            countPlayers -= 1;
        }
        public void SwitchToTeam1()
        {
            countPlayers += 1;
        }*/

/*//timer countdown and sync <--- previously in Update() function
            if(Lobby_Player.localPlayer.isGameStart == true)
            {
                //Debug.Log("gameLobbyCanvas enabled for: " + Lobby_Player.localPlayer);
                gameLobbyCanvas.enabled = true;
                if(Lobby_Player.localPlayer.selectionTimerReset == true)
                {
                    Lobby_Player.localPlayer.selectionTimer = 5.0f;
                    selectionTimerText.text = Lobby_Player.localPlayer.selectionTimer.ToString("f0");
                    Lobby_Player.localPlayer.selectionTimerReset = false;
                }
                else
                {
                    Lobby_Player.localPlayer.selectionTimer -= Time.deltaTime;
                    selectionTimerText.text = Lobby_Player.localPlayer.selectionTimer.ToString("f0");

                    if(Lobby_Player.localPlayer.selectionTimer <= 0)
                    {
                        selectionTimerText.text = "Starting Game...";
                        lockinButton.interactable = false;
                        Lobby_Player.localPlayer.isGameStart = false;
                        //Lobby_Player.localPlayer.selectionTimerZero = true;
                    }
                }
            }*/