using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/
namespace SheepDoom
{
    public class LeaveGame : NetworkBehaviour
    {
        private PlayerObj pO;

        void Awake()
        {
            pO = GetComponent<PlayerObj>();
        }

        [Server]
        public void Exit(string _matchID, bool _isLobby, bool _isCharSelect, bool _isGame, bool _isClean)
        {
            if (_isLobby)
                RemoveFromLobby(_matchID, _isClean);
            else if (_isCharSelect)
                RemoveFromCharSelect(_matchID);
            else if (_isGame)
                RemoveFromGame(_matchID, _isClean);
        }

        [Server]
        private void RemoveFromLobby(string _matchID, bool _isClean) 
        {
            if (_matchID == pO.GetMatchID())
            {
                Match lobbyMatch = MatchMaker.instance.GetMatches()[_matchID];
                if(_isClean)
                {
                    lobbyMatch.GetSDSceneManager().UnloadScenes(pO.ci.GetComponent<NetworkIdentity>().connectionToClient, _matchID, false, true);
                    lobbyMatch.GetSDSceneManager().UnloadScenes(pO.ci.GetComponent<NetworkIdentity>().connectionToClient, _matchID, true, false); // unload on client
                }
                    
                // adjust team counts
                if (pO.GetTeamIndex() == 1)
                    lobbyMatch.MinusTeam1Count();
                else if (pO.GetTeamIndex() == 2)
                    lobbyMatch.MinusTeam2Count();
                // check statuses
                if (pO.GetIsReady()) // applicable for non-hosts only
                {
                    pO.SetIsReady(false);
                    lobbyMatch.MinusCountReady();
                }

                if (pO.GetIsHost()) // only applicable if player is host
                {
                    lobbyMatch.MinusCountReady(); // need to repeat ready count here, intentional, because host by default already ++ready count
                    pO.SetIsHost(false);
                    // host is always 1st player in the list, so when leave, pass to next player, list re-numbers itself when it changes in size
                    // next player settings
                    GameObject nextHostPlayer = null;
                    if (lobbyMatch.GetPlayerObjList().Count > 1)
                    {
                        nextHostPlayer = lobbyMatch.GetPlayerObjList()[1];

                        if (nextHostPlayer.GetComponent<PlayerObj>().GetIsReady())
                            nextHostPlayer.GetComponent<PlayerObj>().SetIsReady(false);

                        nextHostPlayer.GetComponent<PlayerObj>().SetIsHost(true);
                        lobbyMatch.AddCountReady();
                        lobbyMatch.GetLobbyUIManager().HostLeftLobby(nextHostPlayer);
                    }
                    else
                        MatchMaker.instance.ClearMatch(_matchID, true, false, false);
                }

                if (lobbyMatch.GetPlayerObjList().Contains(gameObject))
                {
                    lobbyMatch.GetPlayerObjList().Remove(gameObject);

                    if(_isClean)
                    {
                        lobbyMatch.GetSDSceneManager().MoveToNewScene(pO.ci.gameObject, SceneManager.GetSceneAt(0));
                        Client ci = pO.ci;
                        ci.GetComponent<SpawnManager>().ResetPlayer(ci.GetComponent<NetworkIdentity>().connectionToClient, gameObject);
                    }
                } 
            }
        }

        [Server]
        private void RemoveFromCharSelect(string _matchID) 
        {
            if (_matchID == pO.GetMatchID())
            {
                Match charSelectMatch = MatchMaker.instance.GetMatches()[_matchID];
                charSelectMatch.GetCharacterSelectUIManager().P_gameStarted = true; // so match wont start

                if (charSelectMatch.GetPlayerObjList().Contains(gameObject))
                    charSelectMatch.GetPlayerObjList().Remove(gameObject);

                foreach (GameObject _player in charSelectMatch.GetPlayerObjList())
                {
                    charSelectMatch.GetSDSceneManager().UnloadScenes(_player.GetComponent<PlayerObj>().ci.GetComponent<NetworkIdentity>().connectionToClient, _matchID, false, true);
                    charSelectMatch.GetSDSceneManager().MoveToNewScene(_player.GetComponent<PlayerObj>().ci.gameObject, SceneManager.GetSceneAt(0));
                    Client ci = _player.GetComponent<PlayerObj>().ci;
                    ci.GetComponent<SpawnManager>().ResetPlayer(ci.GetComponent<NetworkIdentity>().connectionToClient, _player);
                }

                charSelectMatch.GetPlayerObjList().Clear();
                MatchMaker.instance.ClearMatch(_matchID, false, true, false);
            }
        }

        [Server]
        private void RemoveFromGame(string _matchID, bool _isClean) 
        {
            if (_matchID == pO.GetMatchID())
            {
                Match gameMatch = MatchMaker.instance.GetMatches()[_matchID];
                if (_isClean)
                    gameMatch.GetSDSceneManager().UnloadScenes(pO.ci.GetComponent<NetworkIdentity>().connectionToClient, _matchID, false, false); // unload on client

                if (gameMatch.GetPlayerObjList().Contains(gameObject))
                    gameMatch.GetPlayerObjList().Remove(gameObject); // remove from match player list

                if (gameMatch.GetHeroesList().Contains(pO.ci.GetComponent<SpawnManager>().GetPlayerObj()))
                {
                    gameMatch.GetHeroesList().Remove(pO.ci.GetComponent<SpawnManager>().GetPlayerObj()); // remove from match hero list
                    if (_isClean)
                        NetworkServer.Destroy(pO.ci.GetComponent<SpawnManager>().GetPlayerObj()); // destroy it if it exits cleanly, else dont need to destroy
                }

                // move back to main menu scene on server
                if (_isClean) // only need to move if it exits cleanly
                {
                    gameMatch.GetSDSceneManager().MoveToNewScene(pO.ci.gameObject, SceneManager.GetSceneAt(0));
                    gameMatch.GetSDSceneManager().MoveToNewScene(gameObject, SceneManager.GetSceneAt(0));
                }
            }
            else
                Debug.Log("WARNING PLAYER IS IN THE WRONG MATCH!! matchID: " + _matchID + " player matchID: " + pO.GetMatchID());
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
}