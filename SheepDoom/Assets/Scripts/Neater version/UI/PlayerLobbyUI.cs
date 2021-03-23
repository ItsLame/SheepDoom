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
        [SyncVar] private Transform myParentObj = null;
        [SyncVar] public PlayerObj myPlayerObj = null;
        private float nextActionTime = 0.0f;
        private float period = 1.0f;    //change this value to change update speed
        
        //existing player will call this
        void Update ()
        {
            //if myPlayerObj & myParentObj not null, rpc every 1 second
            if (Time.time > nextActionTime && myPlayerObj != null && myParentObj != null)
            {
                //Debug.Log(myPlayerObj + " RPC!");
                nextActionTime += period;

                //execute block of code here
                RetrievePlayers();
            }
        }

        //start initializing process
        public void InitPlayer(PlayerObj _playerObj, Transform _parentObj)
        {
            if(hasAuthority)
                CmdInitPlayer(_playerObj, _parentObj);
        }

        //(to server) initialize _playerObj & _parentObj
        [Command]
        private void CmdInitPlayer(PlayerObj _playerObj, Transform _parentObj)
        {
            myParentObj = _parentObj;
            myPlayerObj = _playerObj;
        }

        //function to retrieve players
        public void RetrievePlayers()
        {
            StartCoroutine(StartTransform(myPlayerObj, myParentObj));
        }

        //start _playerObj set parent
        IEnumerator StartTransform(PlayerObj _playerObj, Transform _parentObj)
        {
            //transform for client (you) first
            _playerObj.transform.SetParent(_parentObj, false);

            while(_playerObj.transform.parent != _parentObj)
                yield return _playerObj + " not in parent!";

            //transform for server
            if(hasAuthority)
                CmdPlayerSetParent(_playerObj, _parentObj);
        }

        //(to server) set player obj's parent
        [Command]
        private void CmdPlayerSetParent(PlayerObj _playerObj, Transform _parentObj)
        {
            //can comment out; just visual when vieweing in unity as server (won't matter in server build i think)
            _playerObj.transform.SetParent(_parentObj, false);

            RpcPlayerSetParent(_playerObj, _parentObj);
        }

        //(to all client) set player obj's parent (doesn't apply to late comers)
        [ClientRpc]
        private void RpcPlayerSetParent(PlayerObj _playerObj, Transform _parentObj)
        {
            _playerObj.transform.SetParent(_parentObj, false);
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
