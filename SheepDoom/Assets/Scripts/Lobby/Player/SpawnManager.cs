using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace SheepDoom
{
    public class SpawnManager : NetworkBehaviour
    {
        [Header("Setting up player")]
        [SerializeField] private NetworkIdentity playerPrefab = null;

        [Space(15)]
        [Header("Characters")]
        [SerializeField] private NetworkIdentity almaPrefab = null;
        [SerializeField] private NetworkIdentity AstharothPrefab = null;
        [SerializeField] private NetworkIdentity isabellaPrefab = null;
        private GameObject currentPlayerObj = null;
        private ClientName _cn;
        [SyncVar(hook = nameof(OnTeamUpdate))] private int teamIndex;

        [Space(15)]
        [Header("Player number based on the order they are spawned")]
        //private float currentPlayerNumber;
        [SerializeField] private GameObject playerSpawnPoint;

        [Header("Spawn position (Team 1)")]
        [SerializeField] private GameObject team1SpawnPoint;

        [Header("Spawn position (Team 2)")]
        [Space(15)]
        [SerializeField] private GameObject team2SpawnPoint;
        
        #region Properties

        public GameObject P_playerSpawnPoint
        {
            get{return playerSpawnPoint;}
            set{playerSpawnPoint = value;}
        }

        public int P_playerTeamIndex
        {
            get { return teamIndex; }
            set { teamIndex = value; }
        }

        #endregion

        // dynamically store and call functions and dispatched on the player object spawned by the client
        // note that client prefab/object and player prefab/object are 2 different things but are connected
        public static event Action<GameObject> OnClientPlayerSpawned;

        void Awake()
        {
            _cn = GetComponent<ClientName>();
        }

        void OnTeamUpdate(int oldIndex, int newIndex)
        {
            if (!hasAuthority) return;
            if (newIndex == 1)
                P_playerSpawnPoint = team1SpawnPoint;
            else if (newIndex == 2)
                P_playerSpawnPoint = team2SpawnPoint;
        }

        // This function will be called when player object is spawned for a client, make sure to pass the player obj
        // Once this function is called, it will retrieve the relevant function within OnClientPlayerSpawned Actions and call it for the player obj
        public void InvokePlayerObjectSpawned(GameObject _player)
        {
            currentPlayerObj = _player;
            _cn.SetClientName();
            _cn.SetPlayerName(_cn.GetClientName());
            OnClientPlayerSpawned?.Invoke(_player);
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
            SpawnPlayer("lobby", null);
        }

        [Server]
        public void ResetPlayer(NetworkConnection conn, GameObject _player)
        {
            if (conn == connectionToClient)
            {
                NetworkServer.Destroy(_player);
                NetworkSpawnPlayer("lobby", null);
            }
            else
                Debug.Log("WRONG CONNECTION WTF");
        }

        [Client]
        public void SpawnPlayer(string playerType, GameObject player)
        {
            CmdRequestPlayerObjSpawn(playerType, player);
        }

        [Command] // Request player object to be spawned for client
        void CmdRequestPlayerObjSpawn(string playerType, GameObject player)
        {
            NetworkSpawnPlayer(playerType, player);
        }

        [Server]
        public void SpawnForGame(string playerType, GameObject player)
        {
            NetworkSpawnPlayer(playerType, player);
        }

        [Server]
        private void NetworkSpawnPlayer(string playerType, GameObject player)
        {
            GameObject spawn = null;
            
            if (playerType == "lobby")
            {
                spawn = Instantiate(playerPrefab.gameObject);
            }
            else if (playerType == "game")
            {
                NetworkIdentity hero = null;
                string heroName = string.Empty;
                string matchID = string.Empty;

                heroName = player.GetComponent<PlayerObj>().GetHeroName();
                matchID = player.GetComponent<PlayerObj>().GetMatchID();

                switch(heroName)
                {
                    case "Alma Blanc": hero = almaPrefab; break;
                    case "Astharoth Schwarz": hero = AstharothPrefab; break;
                    case "Isabella Licht": hero = isabellaPrefab; break;
                }

                if(hero != null)
                {
                    // spawn under this script's obj
                    spawn = Instantiate(hero.gameObject, transform);
                }

                if(spawn != null)
                {
                    // unparent spawn from the prev obj
                    spawn.transform.SetParent(null);

                    // go to spawn point   
                    if(player.GetComponent<PlayerObj>().GetTeamIndex() == 1)
                        P_playerSpawnPoint = team1SpawnPoint;
                    else if(player.GetComponent<PlayerObj>().GetTeamIndex() == 2)
                        P_playerSpawnPoint = team2SpawnPoint;
                    
                    spawn.transform.SetPositionAndRotation(P_playerSpawnPoint.transform.position, Quaternion.identity);
                }
            }

            SetPlayerObj(spawn);
            NetworkServer.Spawn(spawn, connectionToClient); // pass the client's connection to spawn the player obj prefab for the correct client into any point in the game
        }

        #region Start & Stop Callbacks

        public override void OnStartClient()
        {
            //get the teamID selected in menu
            /*
            GameObject teamD = GameObject.Find("NetworkManager");
            Debug.Log("GameObject teamD found");
            playerTeamID = teamD.GetComponent<DebugTeamSelector>().getTeamID();
            
            CmdTeamID_TEMP(playerTeamID);
            */
        }

        /*
        [Command]
        private void CmdTeamID_TEMP(float playerTeamID)
        {
            playerTeamID_TEMP = playerTeamID;
        }*/

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

