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
                UI_LobbyScript.instance.SpawnPlayerPrefab(this);   
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
            if(MatchMaker.instance.HostGame(_matchID, gameObject, out playerIndex))
            {
                Debug.Log("Game hosted successfully");
                //convert the 5 digit string to a default mirror method GUID
                networkMatchChecker.matchId = _matchID.ToGuid();

                Debug.Log("test");
                //generate match
                TargetHostGame(true, _matchID, playerIndex);

                //notify the UI for success
                
                //if successful, spawn the UIPlayer prefab
            }

            //if host fail
            else
            {
                Debug.Log("Game hosting failed"); 
                TargetHostGame(false, _matchID, playerIndex);
            }
        }

        [TargetRpc]
        void TargetHostGame(bool success, string _matchID, int _playerIndex)
        {
            playerIndex = _playerIndex;
            matchID = _matchID;
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
            if(MatchMaker.instance.JoinGame(_matchID, gameObject, out playerIndex))
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
            MatchMaker.instance.StartGame(matchID); // pass in the match id so matchmaker will find the correct match on the server to start game
            Debug.Log("Game beginning");
        }

        public void BeginGame()
        {
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
    }
}