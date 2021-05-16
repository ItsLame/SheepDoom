using UnityEngine;
using Mirror;
using System.Collections.Generic;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/
namespace SheepDoom
{
    public class GameStatus : NetworkBehaviour
    {
        public static GameStatus instance;

        [SerializeField] private GameObject navMeshObject;
        private GameObject navMesh;

        [SyncVar] private string matchID;
        [SyncVar (hook = nameof(OnGameEnd))] private bool gameEnded;

        public string P_matchID
        {
            get { return matchID; }
            set { matchID = value; }
        }

        public bool P_gameEnded
        {
            get { return gameEnded; }
            set { gameEnded = value; }
        }

        // when game end
        private void OnGameEnd(bool oldBool, bool newBool)
        {
            // disable player game canvas
            if(newBool == true)
                gameObject.GetComponent<GetGameStatus>().Disable_GameCanvas();
        }

        [Server]
        public void MoveNavMesh(string _matchID)
        {
            MatchMaker.instance.GetMatches()[_matchID].GetSDSceneManager().MoveToNewScene(navMesh, MatchMaker.instance.GetMatches()[_matchID].GetScenes()[2]);
        }

        [Client]
        public void ExitGame()
        {
            AudioManager.instance.sendToLobby();

            GameObject _player = PlayerObj.instance.gameObject;
            if (_player.GetComponent<PlayerObj>().hasAuthority)
                CmdServerExitGame(_player);
        }

        [Command(ignoreAuthority = true)]
        void CmdServerExitGame(GameObject _player)
        {
            _player.GetComponent<LeaveGame>().Exit(P_matchID, false, false, true, true);
            Client ci = _player.GetComponent<PlayerObj>().ci;
            ci.GetComponent<SpawnManager>().ResetPlayer(ci.GetComponent<NetworkIdentity>().connectionToClient, _player);
            if (MatchMaker.instance.GetMatches()[P_matchID].GetPlayerObjList().Count == 0 && MatchMaker.instance.GetMatches()[P_matchID].GetHeroesList().Count == 0)
                MatchMaker.instance.ClearMatch(P_matchID, false, false, true);
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
            navMesh = Instantiate(navMeshObject, transform.position, transform.rotation);
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