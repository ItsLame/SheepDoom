using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using System;

namespace SheepDoom
{
    public class ClientName : NetworkBehaviour
    {
        [Header("Setting up player")]
        private static string userInput;
        private string clientName = string.Empty;
        private SpawnManager _sm;

        void Awake()
        {
            _sm = GetComponent<SpawnManager>();
        }
        // setup client
        public static bool ClientLogin(string user)
        {
            // validate with database here..
            if (string.IsNullOrEmpty(user) || string.IsNullOrWhiteSpace(user))
                return false;
            else
            {
                userInput = user;
                return true; // for now la..
            }
        }

        public void SetClientName()
        {
            clientName = userInput;
        }

        public string GetClientName()
        {
            return clientName;
        }

        // set client name
        public void SetPlayerName(string name)
        {
            clientName = name;
            if (_sm.GetPlayerObj() != null)
            {
                PlayerObj playerName = _sm.GetPlayerObj().GetComponent<PlayerObj>();
                playerName.SetPlayerName(name);
            }
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
