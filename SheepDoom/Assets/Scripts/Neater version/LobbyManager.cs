using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEngine.UI;
// The gameobject this script is attached to should only be spawned as a prefab on the server
public class LobbyManager : NetworkBehaviour
{
<<<<<<< HEAD
    // The gameobject this script is attached to should only be spawned as a prefab on the server
    public class LobbyManager : NetworkBehaviour
    {
        public static LobbyManager instance;
        // these will activate on host/join game, not on starting of application
        [Header("MultiScene Setup")]
        [Scene]
        public string lobbyScene;
        private string matchID = string.Empty;
<<<<<<< Updated upstream
        //[Scene]
        //public string gameScene;
        bool lobbySubSceneLoaded = false;
        //readonly List<Scene> subGameScenes = new List<Scene>();
        //bool gameSubScenesLoaded;
        [SerializeField] Transform UIPlayerParentTeam1;
        [SerializeField] Transform UIPlayerParentTeam2;
=======
        private GameObject UIPlayerParentTeam1;
        private GameObject UIPlayerParentTeam2;
>>>>>>> Stashed changes

        void Start()
        {
            instance = this;
        }

        public void SetMatchID(string _matchID)
        {
            matchID = _matchID;
        }

        public void StartLobbyScene()
        {
            StartCoroutine(ServerLoadSubScenes());
            //ServerAssignTeam();
        }

        IEnumerator ServerLoadSubScenes()
        {
            // load lobby scenes
            for(int i = 1; i <= 1; i++)
            {
                yield return SceneManager.LoadSceneAsync(lobbyScene, LoadSceneMode.Additive);
                Scene newLobbyScene = SceneManager.GetSceneAt(i);
                Debug.Log("Loaded scene name: " + newLobbyScene.name);
                MatchMaker.instance.GetLobbyScenes().Add(matchID, newLobbyScene);
            }
<<<<<<< Updated upstream
            lobbySubSceneLoaded = true;


            /*// load game scenes
            for (int i = 1; i <= matchSceneInstances; i++)
            {
                yield return SceneManager.LoadSceneAsync(lobbyScene, new LoadSceneParameters {loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics3D});
                Scene newGameScene = SceneManager.GetSceneAt(i);
                subGameScenes.Add(newGameScene);
            }*/
=======
            //SceneManager.MoveGameObjectToScene(gameObject, MatchMaker.instance.GetLobbyScenes()[matchID]);
        }

        public void ServerAssignTeam()
        {
            // (server only) to move the playerprefab into other gameobjects in lobby scene
            // Search for team1 & team2 gameobject
            UIPlayerParentTeam1 = GameObject.Find("Players (Team 1)");
            //Debug.Log("lobby manager -> client spawn: " + Client.client.spawn);
            //Client.client.spawn.transform.parent = UIPlayerParentTeam1.transform;
            
>>>>>>> Stashed changes
        }

        public void StartGame()
        {

        }
        public void SwitchTeam()
        {

        }
        #region Start & Stop Callbacks
=======
    [SerializeField] Transform UIPlayerParentTeam1;
    [SerializeField] Transform UIPlayerParentTeam2;
>>>>>>> parent of d3aed25 (Spawned lobby scene from server)

    public void StartGame()
    {

    }
    public void SwitchTeam()
    {

    }
    #region Start & Stop Callbacks

    /// <summary>
    /// This is invoked for NetworkBehaviour objects when they become active on the server.
    /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
    /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
    /// </summary>
    public override void OnStartServer() { }

    /// <summary>
    /// Invoked on the server when the object is unspawned
    /// <para>Useful for saving object data in persistent storage</para>
    /// </summary>
    public override void OnStopServer() { }

    /// <summary>
    /// Called on every NetworkBehaviour when it is activated on a client.  
    /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
    /// </summary>
    public override void OnStartClient() { }

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
