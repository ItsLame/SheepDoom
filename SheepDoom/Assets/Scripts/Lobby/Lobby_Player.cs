using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace MirrorBasics
{
    public class Lobby_Player : NetworkBehaviour
    {
        //setting the localplayer to this script
        public static Lobby_Player localPlayer;

        //matchID if player host room, syncedvared
        [SyncVar] public string matchID;
        [SyncVar] public int playerIndex;
        [SyncVar] public int teamIndex;
        
        //syncvar timer
        [SyncVar] public float selectionTimer;
        //[SyncVar] public bool selectionTimerZero = false;
        [SyncVar] public bool selectionTimerReset = true;

        //bool to check if room owner & if game is started
        [SyncVar] public bool isRoomOwner = false;
        [SyncVar(hook = nameof(StartCountDown))] public bool isGameStart = false;

        NetworkMatchChecker networkMatchChecker;

        void Start()
        {
            networkMatchChecker = GetComponent<NetworkMatchChecker>();
            //adding the current player
            if (isLocalPlayer)
            {
                localPlayer = this;
            }
            else
            {
                UI_LobbyScript.instance.SpawnPlayerPrefab(this);    //this could be the source of the server extra player in room problem
            }
        }

        // Hosting match

        //function for player to host the game
        public void HostGame()
        {
            Debug.Log("Hosting Game");
            string matchID = MatchMaker.GetRandomMatchID();

            //give command to server once a matchID is generated
            CmdHostGame(matchID);
        }

        //sending the server the host game ID, to a list prolly
        [Command]
        void CmdHostGame(string _matchID)
        {
            //setting MatchID
            matchID = _matchID;
            //from client, calling player function, if manage to host a game
            //if host success
            if(MatchMaker.instance.HostGame(_matchID, gameObject, out playerIndex, out teamIndex))
            {
                Debug.Log("Game hosted successfully");
                //convert the 5 digit string to a default mirror method GUID
                networkMatchChecker.matchId = _matchID.ToGuid();

                Debug.Log("test");
                //generate match
                TargetHostGame(true, _matchID, playerIndex, teamIndex);

                //notify the UI for success
                
                //if successful, spawn the UIPlayer prefab
            }

            //if host fail
            else
            {
                Debug.Log("Game hosting failed"); 
                TargetHostGame(false, _matchID, playerIndex, teamIndex);
            }
        }

        [TargetRpc]
        void TargetHostGame(bool success, string _matchID, int _playerIndex, int _teamIndex)
        {
            playerIndex = _playerIndex;
            matchID = _matchID;
            teamIndex = _teamIndex;
            Debug.Log($"MatchID: {matchID} == {_matchID}");
            UI_LobbyScript.instance.HostSuccess(success, _matchID);

            //local player is room owner
            isRoomOwner = true;
        }

        // Joining match

        //function for player to Join the game
        public void JoinGame(string _inputID)
        {
            Debug.Log("Joining Game");
            
            //pass in _inputID from typed input to join game
            CmdJoinGame(_inputID);
        }

        //sending the server the Join game ID, to a list prolly
        [Command]
        void CmdJoinGame(string _matchID)
        {
            //setting MatchID
            matchID = _matchID;
            //from client, calling player function, if manage to Join a game
            //if Join success
            if(MatchMaker.instance.JoinGame(_matchID, gameObject, out playerIndex, out teamIndex))
            {
                Debug.Log("Game Joined successfully");
                //convert the 5 digit string to a default mirror method GUID
                networkMatchChecker.matchId = _matchID.ToGuid();

                //generate match
                TargetJoinGame(true, _matchID, playerIndex);
            }

            //if Join fail
            else
            {
                Debug.Log("Game Joining failed");
                TargetJoinGame(false, _matchID, playerIndex);
            }
        }

        [TargetRpc]
        void TargetJoinGame(bool success, string _matchID, int _playerIndex)
        {
            playerIndex = _playerIndex;
            matchID = _matchID;
            Debug.Log($"MatchID: {matchID} == {_matchID}");
            UI_LobbyScript.instance.JoinSuccess(success, _matchID);

            //local player is not room owner
            isRoomOwner = false;
        }

        //start match
        //available to hosts only
        public void StartGame()
        {
            Debug.Log("Starting Game");
            CmdStartGame();
        }

        [Command]
        void CmdStartGame()
        {
            MatchMaker.instance.StartGame(matchID);
            Debug.Log("Game started successfully");
            //generate match
        }

        public void BeginGame()
        {
            Debug.Log($"MatchID: {matchID} | starting");
            isGameStart = true; // change syncvar hook to display gamelobbycanvas and timer
        }

        //function to start count down and get ready start actual game
        private void StartCountDown(bool oldIsGameStart, bool newIsGameStart)
        {
            //timer countdown and sync
            Debug.Log("check isGameStart state");
            if(isGameStart == true)
            {
                //turn on gamelobbyui canvas
                UI_LobbyScript.instance.gameLobbyCanvas.enabled = true;
                
                //reset timer
                if(selectionTimerReset == true)
                {
                    Debug.Log("selection timer resetted");
                    selectionTimer = 5.0f;
                    UI_LobbyScript.instance.selectionTimerText.text = selectionTimer.ToString("f0");
                    selectionTimerReset = false;
                }
                
                //after reset start countdown
                if(selectionTimerReset == false)
                {
                    Debug.Log("start selection timer countdown");
                    StartCoroutine(Countdown());
                }
            }
            else
            {
                Debug.Log("Starting actual game");
                //when timer reaches 0, proceed to the actual game screen
            }
        }

        //ienumerator for counting down (so timer won't freeze)
        private IEnumerator Countdown()
        {
            //count down will last for this duration
            float duration = selectionTimer + 2.0f;
            float normalizedTime = 0;
            
            while(normalizedTime <= 1.0f)
            {
                normalizedTime += Time.deltaTime / duration;

                selectionTimer -= Time.deltaTime;
                UI_LobbyScript.instance.selectionTimerText.text = selectionTimer.ToString("f0");

                if(selectionTimer <= 0)
                {
                    UI_LobbyScript.instance.selectionTimerText.text = "Starting Game...";
                    UI_LobbyScript.instance.lockinButton.interactable = false;
                    isGameStart = false;
                }

                yield return null;
            }
        }

        //function to auto refresh team list
        public void RefreshTeam(Transform teamParentGroup)
        {
            Debug.Log("Refreshing team list...");
            CmdRefreshTeam(teamParentGroup);
        }

        [Command]
        void CmdRefreshTeam(Transform teamParentGroup)
        {
            Debug.Log("Commencing team refresh...");
            RpcRefreshTeam(teamParentGroup);
        }

        [ClientRpc]
        void RpcRefreshTeam(Transform teamParentGroup)
        {
            if(teamIndex == 1)
            {
                Debug.Log(playerIndex + " belongs to team 1!");
                this.gameObject.transform.SetParent(teamParentGroup);
            }
            else if(teamIndex == 2)
            {
                Debug.Log(playerIndex + " belongs to team 2!");
                this.gameObject.transform.SetParent(teamParentGroup);
            }
        }

        //function for players to switch team
        public void SwitchTeam(Transform teamParentGroup)
        {
            Debug.Log("Attempting to switch team...");
            CmdSwitchTeam(teamParentGroup);
        }

        [Command]
        void CmdSwitchTeam(Transform teamParentGroup)
        {
                Debug.Log("Commencing team switch...");
                RpcSwitchTeam(teamParentGroup);
                //MatchMaker.instance.SwitchTeam(teamParentGroup, out teamIndex);
                TargetSwitchTeam();
        }

        //when team switched, change view for all players
        [ClientRpc]
        void RpcSwitchTeam(Transform teamParentGroup)
        {
            bool isSwitch = true;
            
            if(isSwitch == true)
            {
                if(teamIndex == 1)
                {
                    Debug.Log(playerIndex + " switches to team 2!");
                    teamIndex = 2;
                    this.gameObject.transform.SetParent(teamParentGroup);
                    UI_LobbyScript.instance.SwitchToTeam2();
                }
                else if(teamIndex == 2)
                {
                    Debug.Log(playerIndex + " switches to team 1!");
                    teamIndex = 1;
                    this.gameObject.transform.SetParent(teamParentGroup);
                    UI_LobbyScript.instance.SwitchToTeam1();
                }

                isSwitch = false;
            }
        }

        //change button only for the person who clicked
        [TargetRpc]
        void TargetSwitchTeam()
        {
            if(teamIndex == 1)
            {
                UI_LobbyScript.instance.MoveToTeam1Btn.SetActive(false);
                UI_LobbyScript.instance.MoveToTeam2Btn.SetActive(true);
            }
            else if(teamIndex == 2)
            {
                UI_LobbyScript.instance.MoveToTeam1Btn.SetActive(true);
                UI_LobbyScript.instance.MoveToTeam2Btn.SetActive(false);
            }
        }
    }
}