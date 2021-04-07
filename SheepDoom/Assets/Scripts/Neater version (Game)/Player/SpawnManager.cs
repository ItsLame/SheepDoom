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
        /*[Header("UI Attack Buttons")]
        public Button _NormalButton;
        public Button _SpecialButton;
        public Button _UltiButton;

        [Space(15)]
        public static SpawnManager instance;*/
        
        [Header("Setting up player")]
        [SerializeField]
        private NetworkIdentity playerPrefab = null;
        [SerializeField]
        private NetworkIdentity gameplayPlayerPrefab = null;
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
            if(_player.CompareTag("Player"))
            {
                OnClientPlayerSpawned?.Invoke(_player);
                Debug.Log("Player object spawned");
            }
            else if (_player.CompareTag("lobbyPlayer"))
            {
                _cn.SetClientName();
                _cn.SetPlayerName(_cn.GetClientName());
                OnClientPlayerSpawned?.Invoke(_player);
                Debug.Log("Player object spawned");
            }    
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
            //CmdRequestPlayerObjSpawn(playerPrefab.gameObject);
            //Debug.Log("does player prefab exist here? " + gameplayPlayerPrefab.gameObject);
            //SpawnPlayer("lobby");
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

        /*public void assignButtons(GameObject player)
        {
            _NormalButton = GameObject.Find("AttackButton").GetComponent<Button>();
            _SpecialButton = GameObject.Find("SpecialButton").GetComponent<Button>();
            _UltiButton = GameObject.Find("UltimateButton").GetComponent<Button>();

            //get the action
            UnityAction normalAttack = new UnityAction(player.GetComponent<PlayerAttack>().AttackClick);
            UnityAction specialAttack = new UnityAction(player.GetComponent<PlayerAttack>().SpecialSkillClick);
            UnityAction ultiAttack = new UnityAction(player.GetComponent<PlayerAttack>().UltiClick);

            _NormalButton.onClick.AddListener(normalAttack);
            Debug.Log("Are buttons assigned 2?");
        }*/

        [Server]
        private void NetworkSpawnPlayer(string playerType)
        {
            GameObject spawn = null;
            if (playerType == "lobby")
                spawn = Instantiate(playerPrefab.gameObject);
            else if (playerType == "game")
                spawn = Instantiate(gameplayPlayerPrefab.gameObject, transform.position, Quaternion.identity);
            SetPlayerObj(spawn);

            //assign player attack functions to buttons
            //Debug.Log("Are buttons assigned?");
            //assignButtons(spawn);
            //Debug.Log("Are buttons assigned 3?");
            NetworkServer.Spawn(spawn, connectionToClient); // pass the client's connection to spawn the player obj prefab for the correct client into any point in the game
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
