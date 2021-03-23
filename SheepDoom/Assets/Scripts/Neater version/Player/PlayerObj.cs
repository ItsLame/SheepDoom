using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

namespace SheepDoom
{
    public class PlayerObj : NetworkBehaviour
    {
        public static PlayerObj instance; // only use when outside of gameobject, but currently only works on client
        
        [Header("Player profile")]
        [SyncVar(hook = nameof(OnNameUpdate))] private string syncName; 
        [SyncVar] private string matchID;
        [SyncVar] private int teamIndex = 0;
        [SyncVar] private int playerSortIndex = 0;
        [SyncVar] private bool updateCount = false;
        [SyncVar] private bool isHost = false;
        [SyncVar] private bool isReady = false;

        private void Start()
        {
            
        }

        #region Get
        public string GetPlayerName()
        {
            return syncName;
        }

        public string GetMatchID()
        {
            return matchID;

        }
        public int GetTeamIndex()
        {
            return teamIndex;
        }

        public int GetPlayerSortIndex()
        {
            return playerSortIndex;
        }

        public bool GetIsHost()
        {
            return isHost;
        }

        public bool GetIsReady()
        {
            return isReady;
        }

        public bool GetUpdateCount()
        {
            return updateCount;
        }

        #endregion

        #region Set

        public void SetPlayerName(string name)
        {
            if(hasAuthority)
                CmdSetName(name);
        }

        [Command]
        private void CmdSetName(string name)
        {
            syncName = name;
        }

        public void SetMatchID(string _matchID)
        {
            if(isServer)
                matchID = _matchID;
            else if(hasAuthority)
                CmdSetMatchID(_matchID);
        }

        [Command]
        private void CmdSetMatchID(string _matchID)
        {
            matchID = _matchID;
        }
        
        public void SetTeamIndex(int _teamIndex)
        {
            if(isServer)
                teamIndex = _teamIndex;
            else if(hasAuthority)
                CmdSetTeamIndex(_teamIndex);
        }

        [Command]
        private void CmdSetTeamIndex(int _teamIndex)
        {
            teamIndex = _teamIndex;
        }

        public void SetPlayerSortIndex(int _playerSortIndex)
        {
            playerSortIndex = _playerSortIndex;
        }

        public void SetIsHost(bool _isHost)
        {
            if(isServer)
                isHost = _isHost;
            else if(hasAuthority)
                CmdSetIsHost(_isHost);
        }

        [Command]
        private void CmdSetIsHost(bool _isHost)
        {
            isHost = _isHost;
        }

        public void SetIsReady(bool _isReady)
        {
            if(isServer)
                isReady = _isReady;
            else if(hasAuthority)
                CmdSetIsReady(_isReady);
        }

        [Command]
        private void CmdSetIsReady(bool _isReady)
        {
            isReady = _isReady;
        }

        public void SetUpdateCount(bool _updateCount)
        {
            if(isServer)
                updateCount = _updateCount;
            else if(hasAuthority)
                CmdSetUpdateCount(_updateCount);
        }

        [Command]
        private void CmdSetUpdateCount(bool _updateCount)
        {
                updateCount = _updateCount;
        }

        #endregion

        private void OnNameUpdate(string prev, string next)
        {
            if (hasAuthority)
                MainMenu.instance.SetPlayerName(next);
        }

        public void UpdateTeamCount()
        {
            if (hasAuthority)
            {
                SetUpdateCount(true);
                CmdUpdateTeamCount(this.gameObject);
            }
        }

        [Command]
        private void CmdUpdateTeamCount(GameObject _playerObj)
        {
            MatchMaker.instance.TeamCount(MatchMaker.instance.GetMatchIndex(), _playerObj);
        }

        [Client]
        public void JoinGame(string matchIdInput)
        {
            CmdJoinGame(matchIdInput);
        }

        [Command]
        void CmdJoinGame(string matchIdInput)
        {
            if (Client.client != null)
            {
                matchID = matchIdInput;
                if (MatchMaker.instance.JoinGame(matchID, gameObject))
                {
                    SceneManager.MoveGameObjectToScene(Client.client.gameObject, MatchMaker.instance.GetLobbyScenes()[matchID]);
                    SceneManager.MoveGameObjectToScene(gameObject, MatchMaker.instance.GetLobbyScenes()[matchID]);
                    SceneMessage msg = new SceneMessage
                    {
                        sceneName = MatchMaker.instance.GetLobbyScenes()[matchID].name,
                        sceneOperation = SceneOperation.LoadAdditive
                    };
                    connectionToClient.Send(msg);
                    Debug.Log("Server joined game successfully");
                }
                else
                {
                    matchID = string.Empty;
                    Debug.Log("Failed to command server to join game");
                }
            }
            else
                Debug.Log("Client instance on server is null in playerobj script");
        }

        private void OnMatchJoin(string oldMatchID, string newMatchID)
        {
            
        }

       /* [TargetRpc]
        void TargetJoinGame(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("Loaded scene: " + scene.name);
            
        }*/

        #region Start & Stop Callbacks


        /// <summary>
        /// This is invoked for NetworkBehaviour objects when they become active on the server.
        /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
        /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
        /// </summary>
        public override void OnStartServer() 
        {
            
        }

        /// <summary>
        /// Invoked on the server when the object is unspawned
        /// <para>Useful for saving object data in persistent storage</para>
        /// </summary>
        public override void OnStopServer() { }

        /// <summary>
        /// Called on every NetworkBehaviour when it is activated on a client.
        /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
        /// </summary>
        public override void OnStartClient() // this is running twice for some reason...
        {
            if (Client.client != null)
            {
                Debug.Log("Retrieved client instance for client on playerobj script");
                instance = Client.client.GetComponent<SpawnManager>().GetPlayerObj().GetComponent<PlayerObj>();
                if (instance != null)
                    Debug.Log("PlayerObj initialized on client");
                else
                {
                    Debug.Log("Failed initialization of PlayerObj on client");
                    return;
                }
            }
            else
            {
                Debug.Log("Client is null on client");
                return;
            }
        }

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
        public override void OnStartAuthority() 
        {
            
        }

        /// <summary>
        /// This is invoked on behaviours when authority is removed.
        /// <para>When NetworkIdentity.RemoveClientAuthority is called on the server, this will be called on the client that owns the object.</para>
        /// </summary>
        public override void OnStopAuthority() { }

        #endregion
    }
}