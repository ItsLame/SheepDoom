using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
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
        [SerializeField]
        [SyncVar] private string matchID = string.Empty;
        /*private bool lobbySceneLoaded = false;
        private bool characterSelectSceneLoaded = false;*/
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

        /*public bool P_lobbySceneLoaded
        {
            get {return lobbySceneLoaded;}
            set {lobbySceneLoaded = value;}
        }*/

        public string P_characterSelectScene
        {
            get {return characterSelectScene;}
            set {characterSelectScene = value;}
        }

        /*public bool P_characterSelectSceneLoaded
        {
            get {return characterSelectSceneLoaded;}
            set {characterSelectSceneLoaded = value;}
        }*/

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
            get {return gameSceneLoaded;}
            set {gameSceneLoaded = value;}
        }

        #endregion
        [Server]
        public void StartScenes(NetworkConnection conn)
        {
            //StartCoroutine(LoadScene(P_lobbyScene, P_lobbySceneLoaded, conn));
            //TargetClientLoadScene(conn, P_lobbyScene, P_lobbySceneLoaded);
            StartCoroutine(LoadScene(P_lobbyScene, P_characterSelectScene, conn));
        }

        [Server]
        public void MoveToCharSelect(Scene _scene)
        {
            SceneManager.MoveGameObjectToScene(gameObject, _scene);
        }

        [Server]
        public void UnloadScenes(NetworkConnection conn, string _matchID, bool _unloadLobby, bool _unloadCharSelect)
        {
            StartCoroutine(UnloadScene(conn, _matchID, _unloadLobby, _unloadCharSelect));
        }

        /*public void StartCharacterSelectScene()
        {
            if(isServer)
            {
                StartCoroutine(LoadScene(P_characterSelectScene, P_characterSelectSceneLoaded));
                RpcClientLoadScene(P_characterSelectScene, P_characterSelectSceneLoaded);
            }
        }

        public void StartGameScene()
        {
            if(isServer)
            {
                StartCoroutine(LoadScene(P_gameScene, P_gameSceneLoaded));
                RpcClientLoadScene(P_gameScene, P_gameSceneLoaded);
            }
        }

        [TargetRpc]
        private void TargetClientLoadScene(NetworkConnection conn, string _scene, bool _sceneLoaded)
        {
            StartCoroutine(LoadScene(_scene, _sceneLoaded));
        }

        [ClientRpc]
        private void RpcClientLoadScene(string _scene, bool _sceneLoaded)
        {
            StartCoroutine(LoadScene(_scene, _sceneLoaded));
        }*/

        [Server]
        public void JoinLobby(NetworkConnection conn, string _matchID)
        {
            MatchMaker.instance.GetMatches()[_matchID].GetLobbyUIManager().ServerStartSetting(_matchID);
            ClientSceneMsg(conn, MatchMaker.instance.GetMatches()[_matchID].GetScenes()[1].name, true); // load char select
            ClientSceneMsg(conn, MatchMaker.instance.GetMatches()[_matchID].GetScenes()[0].name, true); // load lobby
        }

        /*private IEnumerator LoadScene(string _scene, bool _sceneLoaded, NetworkConnection conn = null)
        {
            if(isServer && !_sceneLoaded)
            {
                // load lobby scenes
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_scene, LoadSceneMode.Additive);

                //asyncLoad.allowSceneActivation = false;

               /* while (asyncLoad.progress < 0.9f)
                {
                    Debug.Log("LOADING: "+asyncLoad.progress);    
                    yield return new WaitForSecondsRealtime(0.5f);
                }
                while (!asyncLoad.isDone)
                    yield return null;
                
                //asyncLoad.allowSceneActivation = true;
                Scene newLobbyScene = SceneManager.GetSceneAt(MatchMaker.instance.GetMatches().Count);

                MatchMaker.instance.GetMatches()[P_matchID].SetLobbyScene(newLobbyScene);

                SceneMessage msg = new SceneMessage
                {
                    sceneName = MatchMaker.instance.GetMatches()[P_matchID].GetLoadedLobbyScene().name,
                    sceneOperation = SceneOperation.LoadAdditive
                };
                conn.Send(msg);

                SceneManager.MoveGameObjectToScene(gameObject, newLobbyScene);

                _sceneLoaded = true;

                if(_scene == lobbyScene)
                    P_lobbySceneLoaded = _sceneLoaded;
                else if(_scene == P_characterSelectScene)
                    P_characterSelectSceneLoaded = _sceneLoaded;
                else if(_scene == gameScene)
                    P_gameSceneLoaded = _sceneLoaded;
            }
        }*/

        private IEnumerator LoadScene(string _lobbyScene, string _charSelectScene, NetworkConnection conn)
        {
            if (!scenesLoaded)
            {
                // latest loaded scene on client will be the active scene i think
                // load lobby scene
                AsyncOperation asyncLoadLobby = SceneManager.LoadSceneAsync(_lobbyScene, LoadSceneMode.Additive);
                while (!asyncLoadLobby.isDone)
                    yield return null;
                
                // load character select scene
                AsyncOperation asyncLoadCharSelect = SceneManager.LoadSceneAsync(_charSelectScene, LoadSceneMode.Additive);
                while (!asyncLoadCharSelect.isDone)
                    yield return null;

                // will need to change formula once game scene is added, currently is only *2 multiplier to accomodate for lobby and character select scene
                Scene newLobbyScene = SceneManager.GetSceneAt((MatchMaker.instance.GetMatches().Count * 2) - 1);
                Scene newCharSelectScene = SceneManager.GetSceneAt(MatchMaker.instance.GetMatches().Count * 2);
                //Scene newGameScene = SceneManager.GetSceneAt(MatchMaker.instance.GetMatches().Count * 3);

                // set scenes in matches
                MatchMaker.instance.GetMatches()[P_matchID].SetScene(newLobbyScene);
                MatchMaker.instance.GetMatches()[P_matchID].SetScene(newCharSelectScene);

                // send scene load message to clients, latest loaded scene will be the active scene on client
                ClientSceneMsg(conn, MatchMaker.instance.GetMatches()[P_matchID].GetScenes()[1].name, true); // load char select
                ClientSceneMsg(conn, MatchMaker.instance.GetMatches()[P_matchID].GetScenes()[0].name, true); // load lobby

                SceneManager.MoveGameObjectToScene(gameObject, MatchMaker.instance.GetMatches()[P_matchID].GetScenes()[0]);
                P_scenesLoaded = true;
            }
        }

        [Server]
        private IEnumerator UnloadScene(NetworkConnection conn, string _matchID, bool _unloadLobby, bool _unloadCharSelect)
        {
            if(scenesLoaded)
            {
                if (_unloadLobby)
                {
                    ClientSceneMsg(conn, MatchMaker.instance.GetMatches()[_matchID].GetScenes()[0].name, false);
                    yield return SceneManager.UnloadSceneAsync(MatchMaker.instance.GetMatches()[_matchID].GetScenes()[0]);
                }
                // else if (_unloadCharSelect)....
                yield return Resources.UnloadUnusedAssets();
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