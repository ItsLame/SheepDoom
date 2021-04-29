using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

namespace SheepDoom
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager instance;

        private bool playersInScene = false;
        private bool playersLoaded = false;
        private int playersLoadedCount = 0;
        [SerializeField]
        [SyncVar] private string matchID = string.Empty;
        [SerializeField]
        private Transform team1SpawnPoint, team2SpawnPoint;

        public string P_matchID
        {
            get { return matchID; }
            set { matchID = value; }
        }

        public bool GetPlayerSpawnStatus()
        {
            return playersInScene;
        }

        [Server]
        public void StartGameScene(string _matchID)
        {
            Debug.Log("AA");
            if (P_matchID == _matchID)
                playersInScene = true;
            else
                Debug.Log("mismatch in gamemanager matchID and matchmaker sent matchid");
        }
        
        void Update()
        {
            if(isServer && playersInScene && (playersLoadedCount != MatchMaker.instance.GetMatches()[P_matchID].GetPlayerObjList().Count))
            {
                Debug.Log("BB");
                foreach (GameObject _player in MatchMaker.instance.GetMatches()[P_matchID].GetPlayerObjList())
                {
                    _player.GetComponent<PlayerObj>().ci.GetComponent<SpawnManager>().SpawnForGame("game", _player);
                    playersLoadedCount++;
                }
            }
        }

        /*private void Start()
        {
            Debug.Log("SPAWNING PLAYER CHARACTER");

            if(isClient)
            {
                PlayerObj.instance.ci.GetComponent<SpawnManager>().SpawnPlayer("game", PlayerObj.instance.gameObject);
                Debug.Log("My name " + PlayerObj.instance.getPlayerName() + ", my hero " + PlayerObj.instance.GetHeroName() + ", my team " + PlayerObj.instance.GetTeamIndex());
            }
        }*/

        #region Start & Stop Callbacks

        /// <summary>
        /// This is invoked for NetworkBehaviour objects when they become active on the server.
        /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
        /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
        /// </summary>
        public override void OnStartServer() 
        {
            instance = this;
            //MatchMaker.instance.GetMatches()[SDSceneManager.instance.P_matchID].SetGameManager(instance);
        }

        /// <summary>
        /// Invoked on the server when the object is unspawned
        /// <para>Useful for saving object data in persistent storage</para>
        /// </summary>
        public override void OnStopServer() 
        {
            instance = this;
        }

        /// <summary>
        /// Called on every NetworkBehaviour when it is activated on a client.
        /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
        /// </summary>
        public override void OnStartClient() {}

        /// <summary>
        /// This is invoked on clients when the server has caused this object to be destroyed.
        /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
        /// </summary>
        public override void OnStopClient() { }

        /// <summary>
        /// Called when the local player object has been set up.
        /// <para>This happens after OnStartClient(), as it is triggered by an ownership message from the server. This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.</para>
        /// </summary>
        public override void OnStartLocalPlayer() { }

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
