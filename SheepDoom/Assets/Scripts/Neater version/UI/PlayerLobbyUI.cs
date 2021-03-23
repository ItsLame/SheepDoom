using UnityEngine;
using UnityEngine.UI;
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
        [SerializeField] private GameObject myPlayerReadyText = null;
        [SyncVar] private string myPlayerReadyString = null;
        [SyncVar] private Transform myParentObj = null;
        [SyncVar] private PlayerObj myPlayerObj = null;
        [SyncVar] private bool myPlayerReady = false;
        private float nextActionTime = 0.0f;
        private float period = 0.5f;    //change this value to change update speed

        #region Properties

        public string P_myPlayerReadyString
        {
            get{return myPlayerReadyString;}
            set{myPlayerReadyString = value;}
        }

        #endregion

        //existing player will call this
        void Update()
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

        public void InitReady(PlayerObj _playerObj, bool _isReady)
        {
            if(hasAuthority)
                CmdInitReady(_playerObj, _isReady);
        }

        [Command]
        private void CmdInitReady(PlayerObj _playerObj, bool _isReady)
        {
            myPlayerReady = _isReady;
        }

        //function to retrieve players
        public void RetrievePlayers()
        {
            StartCoroutine(StartTransform(myPlayerObj, myParentObj, myPlayerReady));
        }

        private void PlayerSetReady(PlayerObj _playerObj, bool _isReady)
        {
            //range is 0-255
            byte r = 0;
            byte g = 0;
            byte b = 0;
            
            if(_isReady == true)
            {
                P_myPlayerReadyString = "Ready!";
                r = 211;
                g = 255;
                b = 136;
            }
            else if(_isReady == false)
            {
                P_myPlayerReadyString = "Not Ready";
                r = 255;
                g = 183;
                b = 136;
            }

            myPlayerReadyText.GetComponent<Text>().text = P_myPlayerReadyString;
            myPlayerReadyText.GetComponent<Text>().color = new Color32(r, g, b, 255);
        }

        //start _playerObj set parent
        private IEnumerator StartTransform(PlayerObj _playerObj, Transform _parentObj, bool _isReady)
        {
            //transform for client (you) first
            _playerObj.transform.SetParent(_parentObj, false);

            while(_playerObj.transform.parent != _parentObj)
                yield return _playerObj + " not in parent!";

            //ready status
            PlayerSetReady(_playerObj, _isReady);

            //transform for server
            if(hasAuthority)
                CmdPlayerSetParent(_playerObj, _parentObj, _isReady);
        }

        //(to server) set player obj's parent
        [Command]
        private void CmdPlayerSetParent(PlayerObj _playerObj, Transform _parentObj, bool _isReady)
        {
            //can comment out; just visual when vieweing in unity as server (won't matter in server build i think)
            _playerObj.transform.SetParent(_parentObj, false);
            PlayerSetReady(_playerObj, _isReady);

            RpcPlayerSetParent(_playerObj, _parentObj, _isReady);
        }

        //(to all client) set player obj's parent (doesn't apply to late comers)
        [ClientRpc]
        private void RpcPlayerSetParent(PlayerObj _playerObj, Transform _parentObj, bool _isReady)
        {
            //set parent
            _playerObj.transform.SetParent(_parentObj, false);
            
            //ready status
            PlayerSetReady(_playerObj, _isReady);
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
