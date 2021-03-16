using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using System;

namespace SheepDoom
{
    public class SpawnManager : NetworkBehaviour
    {
        [Header("Setting up player")]
        [SerializeField]
        private NetworkIdentity playerPrefab = null;
        private GameObject currentPlayerObj = null;
        private ClientName _cn;

        // dynamically store and call functions and dispatched on the player object spawned by the client
        // note that client prefab/object and player prefab/object are 2 different things but are connected
        public static event Action<GameObject> OnClientPlayerSpawned;

        void Awake()
        {
            _cn = GetComponent<ClientName>();
        }

        // This function will be called when player object is spawned for a client, make sure to pass the player obj
        // Once this function is called, it will retrieve the relevant function within OnClientPlayerSpawned Actions and call it for the player obj
        public void InvokePlayerObjectSpawned(GameObject _player)
        {
            currentPlayerObj = _player;
            _cn.SetClientName();
            _cn.SetPlayerName(_cn.GetClientName());
            Debug.Log("Player object spawned");
            OnClientPlayerSpawned?.Invoke(_player);
        }

        // only works on client
        public GameObject GetPlayerObj()
        {
            GameObject _currentPlayerObj = null;
            if (currentPlayerObj != null)
            {
                Debug.Log("Retrieved current player object on client");
                _currentPlayerObj = currentPlayerObj;
            }
            else
                Debug.Log("Failed to retrieve current player object on client, it is empty");
            return _currentPlayerObj;
        }

        #region Start & Stop Callbacks

        public override void OnStartLocalPlayer()
        {
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
