using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

namespace SheepDoom
{
    public class Player : NetworkBehaviour
    {
        NetworkMatchChecker networkMatchChecker;
        // profile details
        public static Player player;
        private static string userInput;
        [SyncVar] private string username;

        // match settings
        [SyncVar] private string matchID;
        [SyncVar] private int teamIndex;
        [SyncVar] private int playerSortIndex;
        
        //setup profile
        void Awake()
        {
            networkMatchChecker = GetComponent<NetworkMatchChecker>();
        }

        public static bool ClientLogin(string user)
        {
            // validate with database here..
            userInput = user;
            return true; // for now la..
        }

        public override void OnStartServer()
        {
            player = this; // initialize player on server
        }

        public override void OnStartClient()
        {
            player = this; // initialize player on client
        }

        public override void OnStartLocalPlayer() 
        {
            CmdSetPlayerName(userInput);
        }

        [Command] // update player name on the server
        void CmdSetPlayerName(string _username)
        {
            username = _username; 
        }

        public string GetUsername()
        {
            return username;
        }
       

        // Hosting

        public void HostGame()
        {
            CmdHostGame();
        }

        [Command]
        void CmdHostGame()
        {
            matchID = MatchMaker.GetRandomMatchID(); 
            if (MatchMaker.instance.HostGame(matchID, gameObject)) 
            {
                networkMatchChecker.matchId = matchID.ToGuid();
                Debug.Log("Match id is: " + matchID);
                TargetHostGame(true);
            }
            else
            {
                matchID = "";
                TargetHostGame(false);
            }  
        }

        [TargetRpc]
        void TargetHostGame(bool success)
        {
            //matchID = _matchID;
            //teamIndex = _teamIndex;
            //playerSortIndex = _playerSortIndex;
            if(success)
            {
                StartCoroutine(LoadLobbyAsyncScene());
                // spawn player
            }
            else
            {
                Debug.Log("Host failed");
            }
        }

        public void JoinGame(string _matchID)
        {
            CmdJoinGame(_matchID);
        }

        [Command]
        void CmdJoinGame(string _matchID)
        {
            matchID = _matchID;
            if(MatchMaker.instance.JoinGame(matchID, gameObject))
            {
                Debug.Log("return true success");
                networkMatchChecker.matchId = matchID.ToGuid();
                TargetJoinGame(true);
            }
            else
            {
                matchID = "";
                TargetJoinGame(false);
            }
        }

        [TargetRpc]
        void TargetJoinGame(bool success)
        {
            if (success)
            {
                StartCoroutine(LoadLobbyAsyncScene());
                // Spawn player
            }
            else
            {
                Debug.Log("Join failed");
            }

        }

        // Waits until scene finishes loading before any stuff happens
        IEnumerator LoadLobbyAsyncScene()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
            while(!asyncLoad.isDone)
            {
                yield return null;
            }    
        }


        public void SetTeamIndex(int _teamIndex)
        {
            teamIndex = _teamIndex;
        }


        public void SetPlayerSortIndex(int _playerSortIndex)
        {
            playerSortIndex = _playerSortIndex;
        }

        #region Start & Stop Callbacks

        

       

        /// <summary>
        /// Invoked on the server when the object is unspawned
        /// <para>Useful for saving object data in persistent storage</para>
        /// </summary>
        public override void OnStopServer() { }

        /// <summary>
        /// This is invoked on clients when the server has caused this object to be destroyed.
        /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
        /// </summary>
        public override void OnStopClient() { }

        

        /// <summary>
        /// This is invoked on behaviours that have authority, based on context and <see cref="NetworkIdentity.hasAuthority">NetworkIdentity.hasAuthority</see>.
        /// <para>This is called after <see cref="OnStartServer">OnStartServer</see> and before <see cref="OnStartClient">OnStartClient.</see></para>
        /// <para>When <see cref="NetworkIdentity.AssignClientAuthority">AssignClientAuthority</see> is called on the server, this will be called on the client that owns the object. When an object is spawned with <see cref="NetworkServer.Spawn">NetworkServer.Spawn</see> with a NetworkConnection parameter included, this will be called on the client that owns the object.</para>
        /// </summary>
        public override void OnStartAuthority() { }

        /// <summary>
        /// This is invoked on behaviours when authority is removed.
        /// <para>When NetworkIdentity.RemoveClientAuthority is called on the server, this will be called on the client that owns the object.</para>
        /// </summary>
        public override void OnStopAuthority() { }

        #endregion
    }
}
