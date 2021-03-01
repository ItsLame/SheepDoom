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

        NetworkMatchChecker networkMatchChecker;

        void Start()
        {
            //adding the current player
            if (isLocalPlayer)
            {
                localPlayer = this;
            }

            networkMatchChecker = GetComponent<NetworkMatchChecker>();
        }

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
            if(MatchMaker.instance.HostGame(_matchID, gameObject))
            {
                Debug.Log($"<color = green>Game hosted successfully");
                //convert the 5 digit string to a default mirror method GUID
                networkMatchChecker.matchId = _matchID.ToGuid();

                Debug.Log("test");
                //generate match
                TargetHostGame(true, _matchID);

                //notify the UI for success
                
                //if successful, spawn the UIPlayer prefab
            }

            //if host fail
            else
            {
                Debug.Log($"<color = red>Game hosting failed");
                TargetHostGame(false, _matchID);
            }
        }

        [TargetRpc]
        void TargetHostGame(bool success, string _matchID)
        {
            Debug.Log($"MatchID: {matchID} == {_matchID}");
            UI_LobbyScript.instance.HostSuccess(success);
        }

        //function for player to Join the game
        public void JoinGame()
        {
            Debug.Log("Joining Game");
            string matchID = MatchMaker.GetRandomMatchID();

            //give command to server once a matchID is generated
            CmdJoinGame(matchID);
        }

        //sending the server the Join game ID, to a list prolly
        [Command]
        void CmdJoinGame(string _matchID)
        {
            //setting MatchID
            matchID = _matchID;
            //from client, calling player function, if manage to Join a game
            //if Join success
            if (MatchMaker.instance.JoinGame(_matchID, gameObject))
            {
                Debug.Log($"<color = green>Game Joined successfully");
                //convert the 5 digit string to a default mirror method GUID
                networkMatchChecker.matchId = _matchID.ToGuid();

                Debug.Log("test");
                //generate match
                TargetJoinGame(true, _matchID);

                //notify the UI for success

                //if successful, spawn the UIPlayer prefab
            }

            //if Join fail
            else
            {
                Debug.Log($"<color = red>Game Joining failed");
                TargetJoinGame(false, _matchID);
            }
        }

        [TargetRpc]
        void TargetJoinGame(bool success, string _matchID)
        {
            Debug.Log($"MatchID: {matchID} == {_matchID}");
            UI_LobbyScript.instance.JoinSuccess(success);
        }
    }
}