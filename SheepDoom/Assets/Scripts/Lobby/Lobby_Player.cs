using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        [SyncVar] public bool selectionTimerReset = true;

        [SyncVar] public bool isReady = false;
        [SyncVar] private bool isReadyUI = false;

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
            if(MatchMaker.instance.HostGame(_matchID, gameObject, out playerIndex, out teamIndex))
            {
                Debug.Log("Game hosted successfully");
                //convert the 5 digit string to a default mirror method GUID
                networkMatchChecker.matchId = _matchID.ToGuid();
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
            teamIndex = _teamIndex;
            matchID = _matchID;
            //teamIndex = _teamIndex;
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
                TargetJoinGame(true, _matchID, playerIndex, teamIndex);
                EnterRoomInit();
            }

            //if Join fail
            else
            {
                Debug.Log("Game Joining failed");
                TargetJoinGame(false, _matchID, playerIndex, teamIndex);
            }
        }

        [TargetRpc]
        void TargetJoinGame(bool success, string _matchID, int _playerIndex, int _teamIndex)
        {
            playerIndex = _playerIndex;
            teamIndex = _teamIndex;
            matchID = _matchID;
            Debug.Log($"MatchID: {matchID} == {_matchID}");
            UI_LobbyScript.instance.JoinSuccess(success, _matchID);

            //local player is not room owner
            isRoomOwner = false;
        }

        public void EnterRoomInit()
        {
            //isReady = false;
            //isReadyUI = true;
            
            //isReady = false;

            MatchMaker.instance.EnterRoomInit(matchID, gameObject);

            TargetEnterRoomInit();
            RpcEnterRoomInit();
        }

        //server to a client
        [TargetRpc]
        void TargetEnterRoomInit()
        {
            string readyBtnText = "Ready";
            Color readyBtnColor = Color.green;
            
            Debug.Log("[INITIALIZE] Changing " + playerIndex + " status to " + readyBtnText + " in " + readyBtnColor);
            UI_LobbyScript.instance.readyButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = readyBtnText;
            UI_LobbyScript.instance.readyButton.transform.GetChild(0).gameObject.GetComponent<Text>().color = readyBtnColor;

            //MatchMaker.instance.EnterRoomInit(matchID, gameObject);
        }

        //server to all clients
        [ClientRpc]
        void RpcEnterRoomInit()
        {
            string readyStatus = "Waiting";
            Color readyStatusColor = Color.red;

            Debug.Log("[INITIALIZE] Changing " + playerIndex + " status to " + readyStatus + " in " + readyStatusColor);
            gameObject.transform.GetChild(0).gameObject.transform.GetChild(2).GetComponent<Text>().text = readyStatus;
            gameObject.transform.GetChild(0).gameObject.transform.GetChild(2).GetComponent<Text>().color = readyStatusColor;
        }

        // switching team
        public void SwitchTeam(Transform teamParentGroup, string team)
        {
            Debug.Log("Send to server to switch team");
            CmdSwitchTeam(teamParentGroup, team);
        }
        
        [Command]
        void CmdSwitchTeam(Transform teamParentGroup, string team)
        {
            Debug.Log("In cmdswitchteam");
            if(MatchMaker.instance.SwitchTeam(matchID, out teamIndex, team))
            {
                networkMatchChecker.matchId = matchID.ToGuid();
                RpcSwitchTeam(teamParentGroup, teamIndex);
            }
            else
            {
                Debug.Log("Team switch failed");
            }
        }

        [ClientRpc]
        void RpcSwitchTeam(Transform teamParentGroup, int _teamIndex)
        {
            teamIndex = _teamIndex;
            Debug.Log("Switching team..");
            if (teamIndex == 1)
            {
                Debug.Log(playerIndex + " belongs to team 1!");
                this.gameObject.transform.SetParent(teamParentGroup);
            }
            else if (teamIndex == 2)
            {
                Debug.Log(playerIndex + " belongs to team 2!");
                this.gameObject.transform.SetParent(teamParentGroup);
            }
        }

        //start match
        //available to hosts only
        public void StartGame(string StartState)
        {
            Debug.Log("Starting Game");
            CmdStartGame(StartState);
        }

        [Command]
        void CmdStartGame(string StartState)
        {
            Debug.Log("Attempting to start " + matchID + " game...");
            MatchMaker.instance.StartGame(StartState, matchID);
        }

        public void BeginGame(bool StartSuccess)
        {
            if(!StartSuccess)
            {
                Debug.Log($"MatchID: {matchID} | all players must be ready!");
                TargetBeginGame(StartSuccess);
                isGameStart = false;
            }
            else if(StartSuccess)
            {
                Debug.Log($"MatchID: {matchID} | starting");
                TargetBeginGame(StartSuccess);
                isGameStart = true; // change syncvar hook to display gamelobbycanvas and timer
            }
        }

        [TargetRpc]
        void TargetBeginGame(bool StartSuccess)
        {
            if(!StartSuccess)
            {
                UI_LobbyScript.instance.debugText.text = "all players must be ready!";
            }
            else if(StartSuccess)
            {
                UI_LobbyScript.instance.debugText.text = "starting match...";
            }
        }

        public void ReadyGame()
        {
            Debug.Log("Player " + playerIndex + ": pressed ready!");
            CmdReadyGame();
        }

        [Command]
        void CmdReadyGame()
        {
            Debug.Log("Attempt to change Player " + playerIndex + "'s ready status...");

            MatchMaker.instance.ReadyGame(matchID, gameObject);
        }

        public void UpdateReadyCountUI()
        {
            if(isReady)
            {
                isReadyUI = true;
            }
            else if(!isReady)
            {
                isReadyUI = false;
            }

            //change ready button viewable only to the client who pressed it
            TargetReadyGame();
            //change ready status viewable to everyone
            RpcReadyGame();

            if(isReady)
            {
                isReady = false;
            }
            else if(!isReady)
            {
                isReady = true;
            }

            if(isReady)
            {
                isReadyUI = true;
            }
            else if(!isReady)
            {
                isReadyUI = false;
            }
        }

        [TargetRpc]
        void TargetReadyGame()
        {
            string readyBtnText = "";
            Color readyBtnColor = Color.black;

            if(isReadyUI == true)
            {
                readyBtnText = "Cancel";
                readyBtnColor = Color.red;
            }
            else if(isReadyUI == false)
            {
                readyBtnText  = "Ready";
                readyBtnColor = Color.green;
            }
            
            Debug.Log("Changing " + playerIndex + " status to " + readyBtnText + " in " + readyBtnColor);
            UI_LobbyScript.instance.readyButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = readyBtnText;
            UI_LobbyScript.instance.readyButton.transform.GetChild(0).gameObject.GetComponent<Text>().color = readyBtnColor;
        }

        [ClientRpc]
        public void RpcReadyGame()
        {
            string readyStatus = "";
            Color readyStatusColor = Color.black;

            if(isReadyUI == true)
            {
                readyStatus = "Ready!";
                readyStatusColor = Color.green;
            }
            else if(isReadyUI == false)
            {
                readyStatus = "Waiting";
                readyStatusColor = Color.red;
            }

            Debug.Log("Changing " + playerIndex + " status to " + readyStatus + " in " + readyStatusColor);
            gameObject.transform.GetChild(0).gameObject.transform.GetChild(2).GetComponent<Text>().text = readyStatus;
            gameObject.transform.GetChild(0).gameObject.transform.GetChild(2).GetComponent<Text>().color = readyStatusColor;
        }

        //function to start count down and get ready start actual game
        private void StartCountDown(bool oldIsGameStart, bool newIsGameStart)
        {
            //timer countdown and sync
            Debug.Log("check isGameStart state");
            
            string readyStatus = "Ready!";
            Color readyStatusColor = Color.green;
            Debug.Log("Changing " + playerIndex + " status to " + readyStatus + " in " + readyStatusColor);
            gameObject.transform.GetChild(0).gameObject.transform.GetChild(2).GetComponent<Text>().text = readyStatus;
            gameObject.transform.GetChild(0).gameObject.transform.GetChild(2).GetComponent<Text>().color = readyStatusColor;

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

// ------------- ARCHIVE ------------- //

/*[TargetRpc]
        void TargetSwitchTeam(Transform teamParentGroup, int _teamIndex)
        {
            teamIndex = _teamIndex;
            Debug.Log("Switching team..");
            if (teamIndex == 1)
            {
                Debug.Log(playerIndex + " belongs to team 1!");
                this.gameObject.transform.SetParent(teamParentGroup);
            }
            else if (teamIndex == 2)
            {
                Debug.Log(playerIndex + " belongs to team 2!");
                this.gameObject.transform.SetParent(teamParentGroup);
            }
        }

        [Command]
        void CmdRefreshTeam(Transform teamParentGroup, int _teamIndex)
        {
            teamIndex = _teamIndex;
            Debug.Log("Switch team success..switching");
            RpcRefreshTeam(teamParentGroup, teamIndex);
        }*/

//function to auto refresh team list
/*public void RefreshTeam(Transform teamParentGroup)
{
    Debug.Log("Refreshing team list...");
    CmdRefreshTeam(teamParentGroup);
}

//client to server
[Command]
void CmdRefreshTeam(Transform teamParentGroup)
{
    Debug.Log("Commencing team refresh...");
    RpcRefreshTeam(teamParentGroup);
}

//refresh view for all players
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

//client to server
/*[Command]
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
}*/