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
        /*
        [Space(15)]
        public static SpawnManager instance;*/

        [Header("Setting up player")]
        [SerializeField] private NetworkIdentity playerPrefab = null;

        [Space(15)]
        [Header("Characters")]
        [SerializeField] private NetworkIdentity marioPrefab = null;
        [SerializeField] private NetworkIdentity luigiPrefab = null;
        [SerializeField] private NetworkIdentity peachPrefab = null;
        [SerializeField] private NetworkIdentity yoshiPrefab = null;
        [SerializeField] private NetworkIdentity bowserPrefab = null;
        private GameObject currentPlayerObj = null;
        private ClientName _cn;

        [Space(15)]
        [Header("Player number based on the order they are spawned")]
        //private float currentPlayerNumber;
        [SerializeField] private GameObject playerSpawnPoint;
        [SerializeField] [SyncVar(hook = nameof(OnTeamUpdate))] private int playerTeamID = 0;
        //public float playerTeamID;

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

        public int P_playerTeamID
        {
            get{return playerTeamID;}
            set{playerTeamID = value;}
        }

        #endregion

        // dynamically store and call functions and dispatched on the player object spawned by the client
        // note that client prefab/object and player prefab/object are 2 different things but are connected
        public static event Action<GameObject> OnClientPlayerSpawned;

        private void OnTeamUpdate(int oldTeamID, int newTeamID)
        {
            if(newTeamID == 1)
                P_playerSpawnPoint = team1SpawnPoint;
            else if(newTeamID == 2)
                P_playerSpawnPoint = team2SpawnPoint;
        }

        void Awake()
        {
            _cn = GetComponent<ClientName>();
        }

        private void Start()
        {
            //currentPlayerNumber = 0;
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
            SpawnPlayer("lobby", null);
        }

        public void SpawnPlayer(string playerType, GameObject player)
        {
            Debug.Log("EE");
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
            //else if (playerType == "select")
            //{
                //spawn = Instantiate(playerSelectPrefab.gameObject);
            //}
            else if (playerType == "game")
            {
                Debug.Log("is player null? " + player);
                NetworkIdentity hero = null;
                string heroName = string.Empty;
                string matchID = string.Empty;

                heroName = player.GetComponent<PlayerObj>().GetHeroName();
                matchID = player.GetComponent<PlayerObj>().GetMatchID();

                switch(heroName)
                {
                    case "Mario" : hero = marioPrefab; break;
                    case "Luigi" : hero = luigiPrefab; break;
                    case "Peach" : hero = peachPrefab; break;
                    case "Yoshi" : hero = yoshiPrefab; break;
                    case "Bowser" : hero = bowserPrefab; break;
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

#region archive
//public GameObject playerSpawnPoint2;
//public GameObject playerSpawnPoint3;
//public GameObject playerSpawnPoint5;
//public GameObject playerSpawnPoint6;

//[Space(15)]
//the time we will use
//public float SecondsTimer = 0;
//public float MinutesTimer = 0;
//private TimeSpan timePlaying;

//TEMPORARY
//[SyncVar] private float playerTeamID_TEMP = 0;
/*foreach(var player in MatchMaker.instance.GetMatches()[SDSceneManager.instance.P_matchID].GetPlayerObjList())
{
    if(player.GetComponent<PlayerObj>().GetTeamIndex() == 1)
        spawn = Instantiate(playerGameplayPrefab.gameObject, playerSpawnPoint1.transform.position, Quaternion.identity);
    else if(player.GetComponent<PlayerObj>().GetTeamIndex() == 2)
        spawn = Instantiate(playerGameplayPrefab.gameObject, playerSpawnPoint2.transform.position, Quaternion.identity);
}*/

//add 1 to the playercounter in script in networkmanager
/*
GameObject.Find("NetworkManager").GetComponent<PlayerCounter>().addPlayer();
currentPlayerNumber = GameObject.Find("NetworkManager").GetComponent<PlayerCounter>().PlayerCount;

//get the teamID selected in menu
//GameObject teamD = GameObject.Find("NetworkManager");
//Debug.Log("GameObject teamD found");
//playerTeamID = teamD.GetComponent<DebugTeamSelector>().getTeamID();

if (currentPlayerNumber == 1)
{
    spawn = Instantiate(playerGameplayPrefab.gameObject, playerSpawnPoint1.transform.position, Quaternion.identity);
}
//Debug.Log("playerTeamID: " + playerTeamID);

//Debug.Log("Number of players: " + currentPlayerNumber);

//if (playerTeamID == 1)
if (playerTeamID_TEMP == 1)
{
    spawn = Instantiate(playerGameplayPrefab.gameObject, playerSpawnPoint2.transform.position, Quaternion.identity);
    Debug.Log("Spawning in blue team");
    spawn = Instantiate(gameplayPlayerPrefab.gameObject, playerSpawnPoint1.transform.position, Quaternion.identity);
    spawn.gameObject.GetComponent<PlayerAdmin>().setTeamIndex(1);
}

if (playerTeamID_TEMP == 2)
//if (playerTeamID == 2)
{
    spawn = Instantiate(playerGameplayPrefab.gameObject, playerSpawnPoint3.transform.position, Quaternion.identity);
    Debug.Log("Spawning in red team");
    spawn = Instantiate(gameplayPlayerPrefab2.gameObject, playerSpawnPoint4.transform.position, Quaternion.identity);
    spawn.gameObject.GetComponent<PlayerAdmin>().setTeamIndex(2);
}
*/
#endregion
