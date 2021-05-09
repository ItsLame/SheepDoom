using UnityEngine;
using Mirror;
using System.Collections;
using UnityEngine.SceneManagement;

namespace SheepDoom
{
    // The gameobject this script is attached to should only be spawned as a prefab on the server
    public class SDSceneManager : NetworkBehaviour
    {
        public static SDSceneManager instance;
        // these will activate on host/join game, not on starting of application
        [Header("MultiScene Setup")]
        [Scene] public string lobbyScene;
        [Scene] public string characterSelectScene;
        [Scene] public string gameScene;

        // used to initialize in match
        private Scene newLobbyScene;
        private Scene newCharSelectScene;
        private Scene newGameScene;

        [SerializeField]
        [SyncVar] private string matchID = string.Empty;
        private bool scenesLoaded = false;
        private bool gameSceneLoaded = false;

        #region Properties

        public string P_matchID
        {
            get {return matchID;}
            set {matchID = value;}
        }

        public string P_lobbyScene
        {
            get {return lobbyScene;}
            set {lobbyScene = value;}
        }

        public string P_characterSelectScene
        {
            get {return characterSelectScene;}
            set {characterSelectScene = value;}
        }

        public bool P_scenesLoaded
        {
            get {return scenesLoaded;}
            set {scenesLoaded = value;}
        }

        public string P_gameScene
        {
            get {return gameScene;}
            set {gameScene = value;}
        }

        public bool P_gameSceneLoaded
        {
            get { return gameSceneLoaded; }
            set { gameSceneLoaded = value; }
        }

        #endregion
        
        [Server]
        public void StartScenes(NetworkConnection conn = null)
        {
            StartCoroutine(LoadScene(P_lobbyScene, P_characterSelectScene, P_gameScene, conn));
        }

        [Server] 
        public void MoveToNewScene(GameObject _objectToMove, Scene _scene)
        {
            SceneManager.MoveGameObjectToScene(_objectToMove, _scene);
        }

        [Server]
        public void UnloadScenes(NetworkConnection conn, string _matchID, bool _unloadLobby, bool _unloadCharSelect)
        {
            StartCoroutine(UnloadScene(conn, _matchID, _unloadLobby, _unloadCharSelect));
        }

        [Server]
        public void JoinMatch(NetworkConnection conn, string _matchID) // for non-hosts
        {
            ClientSceneMsg(conn, MatchMaker.instance.GetMatches()[_matchID].GetScenes()[1].name, true); // load char select
            ClientSceneMsg(conn, MatchMaker.instance.GetMatches()[_matchID].GetScenes()[0].name, true); // load lobby
        }

        [Server]
        public void JoinGame(NetworkConnection conn, string _matchID)
        {
            ClientSceneMsg(conn, MatchMaker.instance.GetMatches()[_matchID].GetScenes()[2].name, true);
        }

        private IEnumerator LoadScene(string _lobbyScene, string _charSelectScene, string _gameScene, NetworkConnection conn)
        {
            if (!P_scenesLoaded)
            {
                // latest loaded scene on client will be the active scene i think
                // load lobby scene
                yield return SceneManager.LoadSceneAsync(_lobbyScene, LoadSceneMode.Additive);
                newLobbyScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
                MatchMaker.instance.GetMatches()[P_matchID].SetLobbyUIManager(LobbyUIManager.instance);
                MatchMaker.instance.GetMatches()[P_matchID].GetLobbyUIManager().P_matchID = P_matchID;

                // load character select scene
                yield return SceneManager.LoadSceneAsync(_charSelectScene, LoadSceneMode.Additive);
                newCharSelectScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
                MatchMaker.instance.GetMatches()[P_matchID].SetCharacterSelectUIManager(CharacterSelectUIManager.instance);
                MatchMaker.instance.GetMatches()[P_matchID].GetCharacterSelectUIManager().P_matchID = P_matchID;
                
                // set scene in matches
                MatchMaker.instance.GetMatches()[P_matchID].SetScene(newLobbyScene);
                MatchMaker.instance.GetMatches()[P_matchID].SetScene(newCharSelectScene);

                // send scene load message to clients, latest loaded scene will be the active scene on client for hosts
                ClientSceneMsg(conn, MatchMaker.instance.GetMatches()[P_matchID].GetScenes()[1].name, true); // load char select
                ClientSceneMsg(conn, MatchMaker.instance.GetMatches()[P_matchID].GetScenes()[0].name, true); // load lobby

                MoveToNewScene(gameObject, MatchMaker.instance.GetMatches()[P_matchID].GetScenes()[0]);
                P_scenesLoaded = true;
            }
            else if(!P_gameSceneLoaded)
            {
                yield return SceneManager.LoadSceneAsync(_gameScene, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics3D });
                newGameScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
                MatchMaker.instance.GetMatches()[P_matchID].SetGameStatusManager(GameStatus.instance);
                MatchMaker.instance.GetMatches()[P_matchID].GetGameStatusManager().P_matchID = P_matchID;
                MatchMaker.instance.GetMatches()[P_matchID].SetScene(newGameScene);
                MatchMaker.instance.GetMatches()[P_matchID].GetGameStatusManager().MoveNavMesh(P_matchID);
                P_gameSceneLoaded = true;
            }
        }

        [Server]
        private IEnumerator UnloadScene(NetworkConnection conn, string _matchID, bool _unloadLobby, bool _unloadCharSelect)
        {
            if(P_scenesLoaded && (_unloadLobby || _unloadCharSelect))
            {
                int sceneIndex = 0;

                if (_unloadLobby)
                    sceneIndex = 0;
                else if (_unloadCharSelect)
                    sceneIndex = 1;

                if(conn != null)
                    ClientSceneMsg(conn, MatchMaker.instance.GetMatches()[_matchID].GetScenes()[sceneIndex].name, false); 
                else
                {
                    yield return SceneManager.UnloadSceneAsync(MatchMaker.instance.GetMatches()[_matchID].GetScenes()[sceneIndex]);
                    yield return Resources.UnloadUnusedAssets();
                }
            }
            else if (P_gameSceneLoaded)
            {
                if(MatchMaker.instance.GetMatches()[_matchID].GetPlayerObjList().Count != 0 && MatchMaker.instance.GetMatches()[_matchID].GetHeroesList().Count != 0 && conn != null)
                    ClientSceneMsg(conn, MatchMaker.instance.GetMatches()[_matchID].GetScenes()[2].name, false); // unload game scene on client
                else
                {
                    yield return SceneManager.UnloadSceneAsync(MatchMaker.instance.GetMatches()[_matchID].GetScenes()[2]); // unload game scene on server only when players all left
                    yield return Resources.UnloadUnusedAssets();
                    P_gameSceneLoaded = false;
                }
            }
        }

        [Server]
        private void ClientSceneMsg (NetworkConnection conn, string _sceneName, bool _load)
        {
            if (_load)
            {
                SceneMessage msg = new SceneMessage
                {
                    sceneName = _sceneName,
                    sceneOperation = SceneOperation.LoadAdditive
                };
                conn.Send(msg);
            }
            else
            {
                SceneMessage msg = new SceneMessage
                {
                    sceneName = _sceneName,
                    sceneOperation = SceneOperation.UnloadAdditive
                };
                conn.Send(msg);
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
}