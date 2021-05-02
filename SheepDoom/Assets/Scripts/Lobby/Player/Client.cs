using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using System;

namespace SheepDoom
{
    public class Client : NetworkBehaviour
    {
        [Header("Setting up player")]
        public static Client client; // only use for scripts outside of gameobject

        // this is for other scripts to be able to retrieve the client instance with their corresponding connection and network identity
        // To retrieve for other scripts do this => Player player = Player.ReturnPlayerInstance(connectionToClient);
        // If connection is null, your intention was to retrieve the client instance on the client
        // If connection is not null, your intention was to retrieve the client instance on the server
        public static Client ReturnClientInstance(NetworkConnection conn = null) 
        {
            if (NetworkServer.active && conn != null)
            {
                NetworkIdentity localPlayer;
                if (SDNetworkManager.LocalPlayers.TryGetValue(conn, out localPlayer))
                {
                    Debug.Log("Retrieved client instance for server");
                    return localPlayer.GetComponent<Client>(); // if this is returned, this returns server client with the corresponding network identity and has authority
                }
                else
                {
                    Debug.Log("No such client on server");
                    return null;
                }   
            }
            else
                return client; // if this is returned, server client is not connected to the player object, so no authority
        }

        public override void OnStartServer()
        {
            client = ReturnClientInstance(connectionToClient);
        }

        public override void OnStartLocalPlayer() 
        {
            client = this;
        }

        #region Start & Stop Callbacks

        /// <summary>
        /// Invoked on the server when the object is unspawned
        /// <para>Useful for saving object data in persistent storage</para>
        /// </summary>
        public override void OnStopServer() { }

        /// <summary>
        /// This is invoked on clients when the server has caused this object to be destroyed.
        /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
        /// </summary>
        public override void OnStopClient() { }

        /// <summary>
        /// This is invoked on behaviours when authority is removed.
        /// <para>When NetworkIdentity.RemoveClientAuthority is called on the server, this will be called on the client that owns the object.</para>
        /// </summary>
        public override void OnStopAuthority() { }

        #endregion
    }
}
