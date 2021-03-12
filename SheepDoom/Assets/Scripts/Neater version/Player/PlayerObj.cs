﻿using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEngine.UI;

namespace SheepDoom
{
    public class PlayerObj : NetworkBehaviour
    {
        NetworkMatchChecker networkMatchChecker;
        public static PlayerObj instance;
        [Header("Player profile")]
        [SyncVar(hook = nameof(OnNameUpdate))] private string syncName; 
        [SyncVar] private string matchID;
        [SyncVar] private int teamIndex;
        [SyncVar] private int playerSortIndex;

        // setup playerobj
        void Start()
        {
            instance = this;
        }

        void Update()
        {
            if(isClient)
            {
                Debug.Log("Set matchId: " + matchID);
                Debug.Log("Matchmaker set teamindex: " + teamIndex);
                Debug.Log("Matchmaker set playersortindex: " + playerSortIndex);
            }
        }

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

        [Client]
        public void HostGame()
        {
            Debug.Log("Callable");
            CmdHostGame();
        }

        [Command]
        void CmdHostGame()
        {
            Debug.Log("Before serveronly");
            matchID = MatchMaker.GetRandomMatchID(); // syncvared
            if (MatchMaker.instance.HostGame(matchID, gameObject))
            {
                Debug.Log("CmdHostGameSuccess");
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
            networkMatchChecker = GetComponent<NetworkMatchChecker>();
            networkMatchChecker.matchId = string.Empty.ToGuid();
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