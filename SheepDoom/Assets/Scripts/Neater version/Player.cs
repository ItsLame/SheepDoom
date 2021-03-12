using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

namespace SheepDoom
{
    public class Player : NetworkBehaviour
    {
        NetworkMatchChecker networkMatchChecker; // change this to playerprefab script attached to player prefab?
        [Header("Setting up player")]
        [SerializeField]
        private NetworkIdentity playerPrefab = null;
        public static Player player;
        // dynamically store and call functions and dispatched on the player object spawned by the client
        // note that client prefab/object and player prefab/object are 2 different things but are connected
        public static event Action<GameObject> OnClientPlayerSpawned;
        private static string userInput;
        [SyncVar(hook = nameof(SetClientName))] private string username;

        // match settings
        [SyncVar] private string matchID;
        [SyncVar] private int teamIndex;
        [SyncVar] private int playerSortIndex;
        
        //setup 
        void Awake()
        {
            networkMatchChecker = GetComponent<NetworkMatchChecker>();
            networkMatchChecker.matchId = string.Empty.ToGuid();
        }

        public static bool ClientLogin(string user)
        {
            // validate with database here..
            userInput = user;
            return true; // for now la..
        }

        // this is for other scripts to be able to retrieve the client instance with their corresponding connection and network identity
        // To retrieve for other scripts do this => Player player = Player.ReturnPlayerInstance(connectionToClient);
        public static Player ReturnPlayerInstance(NetworkConnection conn = null) // optional parameter
        {
            if (NetworkServer.active && conn != null)
            {
                NetworkIdentity localPlayer;
                if (SDNetworkManager.LocalPlayers.TryGetValue(conn, out localPlayer))
                    return localPlayer.GetComponent<Player>(); // if this is returned, this is owned by a client with the corresponding network identity and has authority
                else
                    return null;
            }
            else
                return player; // if this is returned, no client is connected to the player object, so no authority
            
        }

        public override void OnStartLocalPlayer() 
        {
            player = this;
            CmdRequestPlayerObjSpawn(userInput);
        }


        [Command] // Request player object to be spawned for client
        void CmdRequestPlayerObjSpawn(string userInput)
        {
            username = userInput; // update client name on server
            NetworkSpawnPlayer();
        }

        [Server]
        private void NetworkSpawnPlayer()
        {
            GameObject spawn = Instantiate(playerPrefab.gameObject);
            NetworkServer.Spawn(spawn, connectionToClient); // pass the client's connection to spawn the player obj prefab for the correct client into any point in the game
        }

        // This function will be called when player object is spawned for a client, make sure to pass the player obj
        // Once this function is called, it will retrieve the relevant function within OnClientPlayerSpawned Actions and call it for the player obj
        public void InvokePlayerObjectSpawned(GameObject _player)
        {
            OnClientPlayerSpawned?.Invoke(_player);
        }

        public string GetClientName()
        {
            return username;
        }

        // SyncVar hook
        private void SetClientName(string prev, string next)
        {
            if(hasAuthority)
                MainMenu.instance.SetPlayerName(next);
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
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
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
        /// This is invoked on behaviours when authority is removed.
        /// <para>When NetworkIdentity.RemoveClientAuthority is called on the server, this will be called on the client that owns the object.</para>
        /// </summary>
        public override void OnStopAuthority() { }

        #endregion
    }
}
