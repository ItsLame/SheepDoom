using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using System;

namespace SheepDoom
{
    public class Client : NetworkBehaviour
    {
        [Header("Setting up player")]
        [SerializeField]
        private NetworkIdentity playerPrefab = null;
        public static Client client;

        // dynamically store and call functions and dispatched on the player object spawned by the client
        // note that client prefab/object and player prefab/object are 2 different things but are connected
        public static event Action<GameObject> OnClientPlayerSpawned;
        private static string userInput;
        private string clientName;
        private GameObject currentPlayerObj = null;
        
        // setup client
        public static bool ClientLogin(string user)
        {
            // validate with database here..
            userInput = user;
            return true; // for now la..
        }

        // this is for other scripts to be able to retrieve the client instance with their corresponding connection and network identity
        // To retrieve for other scripts do this => Player player = Player.ReturnPlayerInstance(connectionToClient);
        // If connection is null, your intention was to retrieve the client instance on the client
        // If connection is not null, your intention was to retrieve the client instance on the server
        public static Client ReturnClientInstance(NetworkConnection conn = null) 
        {
            if (NetworkServer.active && conn != null)
            {
                NetworkIdentity localPlayer;
                if (SDNetworkManager.LocalPlayers.TryGetValue(conn, out localPlayer))
                {
                    Debug.Log("Retrieved client instance for server");
                    return localPlayer.GetComponent<Client>(); // if this is returned, this returns server client with the corresponding network identity and has authority
                }
                else
                {
                    Debug.Log("No such client on server");
                    return null;
                }   
            }
            else
            {
                Debug.Log("Retrieved client instance for client");
                return client; // if this is returned, server client is not connected to the player object, so no authority
            }
        }

        public override void OnStartServer()
        {

        }

        public override void OnStartLocalPlayer() 
        {
            client = this;
            //DontDestroyOnLoad(client.gameObject);
            CmdRequestPlayerObjSpawn();
        }

        [Command] // Request player object to be spawned for client
        void CmdRequestPlayerObjSpawn()
        {
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
            currentPlayerObj = _player;
            clientName = userInput;
            SetPlayerName(clientName);
            Debug.Log("Player object spawned");
            OnClientPlayerSpawned?.Invoke(_player);
        }

        public GameObject GetPlayerObj()
        {
            GameObject _currentPlayerObj = null;
            if (currentPlayerObj != null)
            {
                Debug.Log("Retrieved current player object");
                _currentPlayerObj = currentPlayerObj;
            }
            else
                Debug.Log("Failed to retrieve current player object, it is empty");
            return _currentPlayerObj;
        }

        // set client name
        public void SetPlayerName(string name)
        {
            clientName = name;
            if (currentPlayerObj != null)
            {
                PlayerObj playerName = currentPlayerObj.GetComponent<PlayerObj>();
                playerName.SetPlayerName(name);
            }
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
