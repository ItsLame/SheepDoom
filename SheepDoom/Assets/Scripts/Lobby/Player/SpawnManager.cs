using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

namespace SheepDoom
{
    public class SpawnManager : NetworkBehaviour
    {
        /*
        [Space(15)]
        public static SpawnManager instance;*/

        [Header("Setting up player")]
        [SerializeField]
        private NetworkIdentity playerPrefab = null;
        [SerializeField]
        private NetworkIdentity gameplayPlayerPrefab = null;

        //second player
        [SerializeField]
        private NetworkIdentity gameplayPlayerPrefab2 = null;
        private GameObject currentPlayerObj = null;
        private ClientName _cn;

        [Space(15)]
        [Header("Player number based on the order they are spawned")]
        private float currentPlayerNumber;
        public float playerTeamID;

        [Header("Spawn position (Team 1)")]
        public GameObject playerSpawnPoint1;
        public GameObject playerSpawnPoint2;
        public GameObject playerSpawnPoint3;


        [Header("Spawn position (Team 2)")]
        [Space(15)]
        public GameObject playerSpawnPoint4;
        public GameObject playerSpawnPoint5;
        public GameObject playerSpawnPoint6;

        [Space(15)]
        //the time we will use
        public float SecondsTimer = 0;
        public float MinutesTimer = 0;
        private TimeSpan timePlaying;

        //TEMPORARY
        [SyncVar] private float playerTeamID_TEMP = 0;

        // dynamically store and call functions and dispatched on the player object spawned by the client
        // note that client prefab/object and player prefab/object are 2 different things but are connected
        public static event Action<GameObject> OnClientPlayerSpawned;

        void Awake()
        {
            _cn = GetComponent<ClientName>();

        }
        private void Start()
        {
            currentPlayerNumber = 0;
        }

        // This function will be called when player object is spawned for a client, make sure to pass the player obj
        // Once this function is called, it will retrieve the relevant function within OnClientPlayerSpawned Actions and call it for the player obj
        public void InvokePlayerObjectSpawned(GameObject _player)
        {
            currentPlayerObj = _player;
            _cn.SetClientName();
            _cn.SetPlayerName(_cn.GetClientName());
            OnClientPlayerSpawned?.Invoke(_player);
            Debug.Log("Player object spawned");
        }

        // only works on client
        public GameObject GetPlayerObj()
        {
           return currentPlayerObj;
        }

        public void SetPlayerObj(GameObject _currentPlayerObj)
        {
            currentPlayerObj = _currentPlayerObj;
        }

        public override void OnStartLocalPlayer()
        {
            SpawnPlayer("game");
        }

        public void SpawnPlayer(string playerType)
        {
            CmdRequestPlayerObjSpawn(playerType);
        }

        [Command] // Request player object to be spawned for client
        void CmdRequestPlayerObjSpawn(string playerType)
        {
            NetworkSpawnPlayer(playerType);
        }

        [Server]
        private void NetworkSpawnPlayer(string playerType)
        {
            GameObject spawn = null;
            if (playerType == "lobby")
            {
                spawn = Instantiate(playerPrefab.gameObject);
            }

            else if (playerType == "game")
            {
                //add 1 to the playercounter in script in networkmanager
                GameObject.Find("NetworkManager").GetComponent<PlayerCounter>().addPlayer();
                currentPlayerNumber = GameObject.Find("NetworkManager").GetComponent<PlayerCounter>().PlayerCount;

                //get the teamID selected in menu
                //GameObject teamD = GameObject.Find("NetworkManager");
                //Debug.Log("GameObject teamD found");
                //playerTeamID = teamD.GetComponent<DebugTeamSelector>().getTeamID();

                //Debug.Log("playerTeamID: " + playerTeamID);

                //Debug.Log("Number of players: " + currentPlayerNumber);

                //if (playerTeamID == 1)
                if (playerTeamID_TEMP == 1)
                {
                    Debug.Log("Spawning in blue team");
                    spawn = Instantiate(gameplayPlayerPrefab.gameObject, playerSpawnPoint1.transform.position, Quaternion.identity);
                }

                if (playerTeamID_TEMP == 2)
                //if (playerTeamID == 2)
                {
                    Debug.Log("Spawning in red team");
                    spawn = Instantiate(gameplayPlayerPrefab2.gameObject, playerSpawnPoint4.transform.position, Quaternion.identity);
                }
            }

            SetPlayerObj(spawn);

            //assign player attack functions to buttons
            //Debug.Log("Are buttons assigned?");
            //assignButtons(spawn);
            //Debug.Log("Are buttons assigned 3?");
            NetworkServer.Spawn(spawn, connectionToClient); // pass the client's connection to spawn the player obj prefab for the correct client into any point in the game
        }

        #region Start & Stop Callbacks

        public override void OnStartClient()
        {
            //get the teamID selected in menu
            GameObject teamD = GameObject.Find("NetworkManager");
            Debug.Log("GameObject teamD found");
            playerTeamID = teamD.GetComponent<DebugTeamSelector>().getTeamID();
            
            CmdTeamID_TEMP(playerTeamID);
        }

        [Command]
        private void CmdTeamID_TEMP(float playerTeamID)
        {
            playerTeamID_TEMP = playerTeamID;
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
