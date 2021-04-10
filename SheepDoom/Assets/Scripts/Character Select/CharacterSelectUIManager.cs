using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

namespace SheepDoom
{
    public class CharacterSelectUIManager : NetworkBehaviour
    {
        public static CharacterSelectUIManager instance;

        [Header("Character Select UI Manager Setup")]
        [SerializeField] private GameObject team1GameObject;
        [SerializeField] private GameObject team2GameObject;

        [Header("Inputs For Client")]
        [SerializeField] private GameObject lockInButton;
        
        //room matchID
        [SyncVar] private string matchID = string.Empty;

        #region Properties

        public GameObject P_team1GameObject
        {
            get{return team1GameObject;}
            set{team1GameObject = value;}
        }

        public GameObject P_team2GameObject
        {
            get{return team2GameObject;}
            set{team2GameObject = value;}
        }

        #endregion

        private void Start()
        {
            if(isServer)
                ServerStartSetting(SDSceneManager.instance.P_matchID);
            if(isClient)
            {
                //PlayerObj.instance.ci.GetComponent<SpawnManager>().SpawnPlayer("select");
                PlayerObj.instance.GetComponent<PlayerLobbyUI>().P_playerLobbyObject.SetActive(false);
                PlayerObj.instance.GetComponent<PlayerLobbyUI>().P_playerCharacterSelectObject.SetActive(true);
                PlayerObj.instance.gameObject.transform.SetParent(team1GameObject.transform, false);
                //PlayerObj.instance.gameObject.transform.SetParent(team1GameObject, false);
            }
                
                //ClientStartSetting();
        }

        private void ServerStartSetting(string _matchID)
        {
            StartCoroutine(SetUI_CharacterSelect(_matchID));
        }

        private IEnumerator SetUI_CharacterSelect(string _matchID)
        {
            if(isServer)
            {
                while(!MatchMaker.instance.GetMatches()[_matchID].GetSDSceneManager().P_characterSelectSceneLoaded)
                    yield return null;

                foreach(var _player in MatchMaker.instance.GetMatches()[_matchID].GetPlayerObjList())
                {
                    Debug.Log("SETUI: CHARACTER SELECT");
                    //Debug.Log("Player matchID: " + _player.GetComponent<PlayerObj>().GetMatchID());
                    //Debug.Log("Player index: " + _player.GetComponent<PlayerObj>().GetTeamIndex());

                    if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 1)
                    {
                        MatchMaker.instance.GetMatches()[_matchID].GetCharacterSelectUIManager().P_team1GameObject.SetActive(true);
                        MatchMaker.instance.GetMatches()[_matchID].GetCharacterSelectUIManager().P_team2GameObject.SetActive(false);
                        _player.GetComponent<PlayerObj>().gameObject.transform.SetParent(MatchMaker.instance.GetMatches()[_matchID].GetLobbyUIManager().P_team1GameObject.transform, false);
                    }
                        
                    else if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 2)
                    {
                        MatchMaker.instance.GetMatches()[_matchID].GetCharacterSelectUIManager().P_team1GameObject.SetActive(false);
                        MatchMaker.instance.GetMatches()[_matchID].GetCharacterSelectUIManager().P_team2GameObject.SetActive(true);
                        _player.GetComponent<PlayerObj>().gameObject.transform.SetParent(MatchMaker.instance.GetMatches()[_matchID].GetLobbyUIManager().P_team2GameObject.transform, false);
                    }
                }
            }
        }

        /*public void ClientStartSetting()
        {
            //StartCoroutine(SetUI_CharacterSelect(P_matchID));
        }*/

        #region Start & Stop Callbacks

        /// <summary>
        /// This is invoked for NetworkBehaviour objects when they become active on the server.
        /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
        /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
        /// </summary>
        public override void OnStartServer()
        {
            instance = this;
            MatchMaker.instance.GetMatches()[SDSceneManager.instance.P_matchID].SetCharacterSelectUIManager(instance);

            foreach(var player in MatchMaker.instance.GetMatches()[SDSceneManager.instance.P_matchID].GetPlayerObjList())
            {
                SDNetworkManager.LocalPlayersNetId.TryGetValue(player.GetComponent<PlayerObj>().ci.gameObject.GetComponent<NetworkIdentity>(), out NetworkConnection conn);
                SceneManager.MoveGameObjectToScene(Client.ReturnClientInstance(conn).gameObject, SceneManager.GetSceneByName(MatchMaker.instance.GetMatches()[SDSceneManager.instance.P_matchID].GetScene().name));
            }
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
            //PlayerObj.instance.GetComponent<SpawnManager>().SpawnPlayer("character select");
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
