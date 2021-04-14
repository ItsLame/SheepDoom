using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*namespace SheepDoom
{
    // The gameobject this script is attached to should only be spawned as a prefab on the server
    public class LobbyManager : NetworkBehaviour
    {
        public static LobbyManager instance;
        // these will activate on host/join game, not on starting of application
        [Header("MultiScene Setup")]
        [Scene] public string lobbyScene;
        [SyncVar] private string matchID = string.Empty;
        private bool lobbySceneLoaded = false;

        #region Properties

        public string P_lobbyScene
        {
            get {return lobbyScene;}
            set {lobbyScene = value;}
        }

        public string P_matchID
        {
            get {return matchID;}
            set {matchID = value;}
        }

        public bool P_lobbySceneLoaded
        {
            get {return lobbySceneLoaded;}
            set {lobbySceneLoaded = value;}
        }

        #endregion

        public void StartLobbyScene(NetworkConnection conn)
        {
            if(isServer)
            {
                StartCoroutine(LoadLobbyScene());
                TargetClientLoadLobbyScene(conn);
            }
        }

        [TargetRpc]
        private void TargetClientLoadLobbyScene(NetworkConnection conn)
        {
            StartCoroutine(LoadLobbyScene());
        }

        public void JoinLobby(string _matchID)
        {
            if(isServer)
                MatchMaker.instance.GetMatches()[_matchID].GetLobbyUIManager().ServerStartSetting(_matchID);
        }

        private IEnumerator LoadLobbyScene()
        {
            if(isServer && !P_lobbySceneLoaded)
            {
                // load lobby scenes
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(P_lobbyScene, LoadSceneMode.Additive);
                
                while (!asyncLoad.isDone)
                    yield return null;

                Scene newLobbyScene = SceneManager.GetSceneAt(MatchMaker.instance.GetMatches().Count); 

                MatchMaker.instance.GetMatches()[matchID].SetLobbyScene(newLobbyScene);                
                SceneManager.MoveGameObjectToScene(gameObject, newLobbyScene);

                P_lobbySceneLoaded = true;
            }
            
            if(isClient)
            {
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(P_lobbyScene, LoadSceneMode.Additive);
                
                while(!asyncLoad.isDone)
                    yield return null;
            }
        }
        
        #region Start & Stop Callbacks

        /// <summary>
        /// This is invoked for NetworkBehaviour objects when they become active on the server.
        /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
        /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
        /// </summary>
        public override void OnStartServer() 
        {
            instance = this;
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
        public override void OnStartClient()
        {
            instance = this;
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
        public override void OnStartAuthority() { }

        /// <summary>
        /// This is invoked on behaviours when authority is removed.
        /// <para>When NetworkIdentity.RemoveClientAuthority is called on the server, this will be called on the client that owns the object.</para>
        /// </summary>
        public override void OnStopAuthority() { }

        #endregion
    }
}*/