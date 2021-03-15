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
        public static PlayerObj instance;
        [Header("Player profile")]
        [SyncVar(hook = nameof(OnNameUpdate))] private string syncName; 
        [SyncVar] private string matchID;
        [SyncVar] private int teamIndex = 0;
        [SyncVar] private int playerSortIndex = 0;

        [Client]
        public void SetPlayerName(string name)
        {
            CmdSetName(name);
        }

        [Command]
        private void CmdSetName(string name)
        {
            syncName = name;
        }

        private void OnNameUpdate(string prev, string next)
        {
            if (hasAuthority)
                MainMenu.instance.SetPlayerName(next);
        }

        // playerobj match settings

        public void SetTeamIndex(int _teamIndex)
        {
            teamIndex = _teamIndex;
        }

        public void SetPlayerSortIndex(int _playerSortIndex)
        {
            playerSortIndex = _playerSortIndex;
        }

       /* [Client]
        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        [Client]
        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        [Client]
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.MoveGameObjectToScene(Client.client.gameObject, scene);
            SceneManager.MoveGameObjectToScene(gameObject, scene);
        }*/

        [Client]
        public void HostGame()
        {
            CmdHostGame();
        }

        [Command]
        void CmdHostGame()
        {
            if (Client.client != null)
            {
                matchID = MatchMaker.GetRandomMatchID(); // syncvared
                if (MatchMaker.instance.HostGame(matchID, gameObject))
                {
                    StartCoroutine(WaitForSyncList(MatchMaker.instance.GetLobbyScenes().Count, Client.client));
                }
                else
                {
                    matchID = string.Empty;
                    Debug.Log("Failed to command server to host game");
                }
            }
            else
                Debug.Log("Client instance on server is null in playerobj script");
        }

        IEnumerator WaitForSyncList(int oldCount, Client client)
        {
            while(MatchMaker.instance.GetLobbyScenes().Count == oldCount)
                yield return null;
            SceneManager.MoveGameObjectToScene(client.gameObject, MatchMaker.instance.GetLobbyScenes()[matchID]);
            SceneManager.MoveGameObjectToScene(gameObject, MatchMaker.instance.GetLobbyScenes()[matchID]);
            SceneMessage msg = new SceneMessage
            {
                sceneName = MatchMaker.instance.GetLobbyScenes()[matchID].name,
                sceneOperation = SceneOperation.LoadAdditive
            };
            connectionToClient.Send(msg);
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
            if(hasAuthority)
            {
                if (Client.client != null)
                {
                    Debug.Log("Retrieved client instance for client on playerobj script");
                    instance = Client.client.GetPlayerObj().GetComponent<PlayerObj>();
                    if (instance != null)
                        Debug.Log("PlayerObj initialized on client");
                }
            }
            else
            {
                Debug.Log("no authority");
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