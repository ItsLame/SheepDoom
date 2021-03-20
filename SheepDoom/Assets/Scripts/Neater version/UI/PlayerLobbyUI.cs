using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

namespace SheepDoom
{
    //to set player transform (in lobby)
    public class PlayerLobbyUI : NetworkBehaviour
    {
        public static PlayerLobbyUI instance;

        //retrieve early birds
        /*public void AddPlayerToList(LobbyUIManager _instance, GameObject _team1GO, GameObject _team2GO)
        {
            CmdAddPlayerToList(_instance, _team1GO, _team2GO);
        }

        [Command]
        private void CmdAddPlayerToList(LobbyUIManager _instance, GameObject _team1GO, GameObject _team2GO)
        {
            _instance.playersInLobby.Add(PlayerObj.instance);
            RpcAddPlayerToList(_instance, _team1GO, _team2GO);
        }

        [ClientRpc]
        private void RpcAddPlayerToList(LobbyUIManager _instance, GameObject _team1GO, GameObject _team2GO)
        {
            if(_instance.playersInLobby == null)
            {
                Debug.Log(PlayerObj.instance + "is first player!");
            }
            else if(_instance.playersInLobby != null)
            {
                for(int i = 0 ; i != _instance.playersInLobby.Count ; i++)
                {
                    if(_instance.playersInLobby[i].GetTeamIndex() == 1)
                    {
                        Debug.Log("PREVIOUS updating team1 UI");
                        _instance.playersInLobby[i].GetComponent<PlayerLobbyUI>().PlayerSetParent(_team1GO.transform);
                    }
                    if(_instance.playersInLobby[i].GetTeamIndex() == 2)
                    {
                        Debug.Log("PREVIOUS updating team2 UI");
                        _instance.playersInLobby[i].GetComponent<PlayerLobbyUI>().PlayerSetParent(_team2GO.transform);
                    }
                }
            }
        }*/

        public void PlayerSetParent(Transform _parentObj)
        {
            StartCoroutine(StartTransform(_parentObj));
        }

        IEnumerator StartTransform(Transform _parentObj)
        {
            //transform for client
            PlayerObj.instance.transform.SetParent(_parentObj);

            yield return "client ok, now the server";

            //transform for server
            CmdPlayerSetParent(PlayerObj.instance, _parentObj);
        }

        [Command]
        private void CmdPlayerSetParent(PlayerObj _playerObj, Transform _parentObj)
        {
            _playerObj.transform.SetParent(_parentObj);
            RpcPlayerSetParent(_playerObj, _parentObj);
        }

        //to all client (doesn't apply to late comers)
        [ClientRpc]
        private void RpcPlayerSetParent(PlayerObj _playerObj, Transform _parentObj)
        {
            _playerObj.transform.SetParent(_parentObj);
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
