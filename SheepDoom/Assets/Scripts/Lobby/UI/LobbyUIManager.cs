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
    //set matchid and players in match
    public class LobbyUIManager : NetworkBehaviour
    {
        public static LobbyUIManager instance;

        [Header("Lobby UI Manager Setup")]
        [SerializeField] private GameObject matchIDText;
        [SerializeField] private GameObject team1GameObject;
        [SerializeField] private GameObject team2GameObject;

        [Header("Inputs For Client")]
        [SerializeField] private GameObject toTeam1Button;
        [SerializeField] private GameObject toTeam2Button;
        [SerializeField] private GameObject startButton;
        [SerializeField] private GameObject readyButton;

        [Header("Warning Messages")]
        [SerializeField] private GameObject startStatusText;
        
        //room matchID and matchIndex
        [SyncVar] private string matchID = string.Empty;
        [SyncVar] private bool startMatch = false;
        
        #region Properties
        
        public GameObject P_matchIDText
        {
            get {return matchIDText;}
            set {matchIDText = value;}
        }

        public GameObject P_team1GameObject
        {
            get {return team1GameObject;}
            set {team1GameObject = value;}       
        }

        public GameObject P_team2GameObject
        {
            get {return team2GameObject;}
            set {team2GameObject = value;}
        }

        public string P_matchID
        {
            get {return matchID;}
            set {matchID = value;}
        }

        public GameObject P_toTeam1Button
        {
            get{return toTeam1Button;}
            set{toTeam1Button = value;}
        }

        public GameObject P_toTeam2Button
        {
            get{return toTeam2Button;}
            set{toTeam2Button = value;}
        }

        public GameObject P_startButton
        {
            get{return startButton;}
            set{startButton = value;}
        }

        public GameObject P_readyButton
        {
            get{return readyButton;}
            set{readyButton = value;}
        }

        public GameObject P_startStatusText
        {
            get{return startStatusText;}
            set{startStatusText = value;}
        }

        #endregion

        private void Start()
        {
            if(isServer)
                ServerStartSetting(SDSceneManager.instance.P_matchID);
            if(isClient)
                ClientStartSetting();
        }

        #region Server Functions

        public void ServerStartSetting(string _matchID)
        {
            //begin setting matchID
            SetUI_MatchID(_matchID);
        }

        private void SetUI_MatchID(string _matchID)
        {
            if(P_matchIDText.GetComponent<Text>().text == string.Empty || P_matchIDText.GetComponent<Text>().text != _matchID)
            {
                P_matchID = _matchID;
                P_matchIDText.GetComponent<Text>().text = P_matchID;
            }

            StartCoroutine(SetUI_Lobby(P_matchID));
        }

        #endregion

        #region Client Functions

        public void ClientStartSetting()
        {
            StartCoroutine(SetUI_Lobby(P_matchID));
        }

        private IEnumerator RequestLobbyUpdate(string _matchID, GameObject _player, bool _startMatch)
        {
            CmdRequestLobbyUpdate(_matchID, _player, _startMatch);

            yield return null;
        }

        [Command(ignoreAuthority = true)]
        private void CmdRequestLobbyUpdate(string _matchID, GameObject _player, bool _startMatch)
        {
            Debug.Log("Count in networkid-conn list: " + SDNetworkManager.LocalPlayersNetId.Count);
            if(SDNetworkManager.LocalPlayersNetId.TryGetValue(_player.GetComponent<PlayerObj>().ci.gameObject.GetComponent<NetworkIdentity>(), out NetworkConnection conn))
            {
                Debug.Log("Did i find the correct connection?");
                foreach(var player in MatchMaker.instance.GetMatches()[_matchID].GetPlayerObjList())
                {
                        TargetUpdateJoiner(conn, player, _startMatch, MatchMaker.instance.GetMatches()[_matchID].GetSDSceneManager());
                }

                RpcUpdateExisting(_player, _startMatch, MatchMaker.instance.GetMatches()[_matchID].GetSDSceneManager());

                if(_startMatch == true)
                {
                    MatchMaker.instance.GetMatches()[P_matchID].GetSDSceneManager().StartCharacterSelectScene();
                }
            }
            else
            {
                Debug.Log("Connection with this netID does not exist");
            }
        }

        [TargetRpc]
        private void TargetUpdateJoiner(NetworkConnection conn, GameObject _player, bool _startMatch, SDSceneManager _sceneManager)
        {
            //Debug.Log("Did i run?");
            if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 1)
                _player.transform.SetParent(team1GameObject.transform, false);
            else if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 2)
                _player.transform.SetParent(team2GameObject.transform, false);

            // range is 0-255
            byte r = 0;
            byte g = 0;
            byte b = 0;

            if(_player.GetComponent<PlayerObj>().GetIsReady() == false)
            {
                r = 255;
                g = 183;
                b = 136;

                _player.GetComponent<PlayerObj>().GetComponent<PlayerLobbyUI>().P_playerReady.text = "Not Ready";
                _player.GetComponent<PlayerObj>().GetComponent<PlayerLobbyUI>().P_playerReady.color = new Color32(r, g, b, 255);
            }
            else if(_player.GetComponent<PlayerObj>().GetIsReady() == true)
            {
                r = 211;
                g = 255;
                b = 136;

                _player.GetComponent<PlayerObj>().GetComponent<PlayerLobbyUI>().P_playerReady.text = "Ready!";
                _player.GetComponent<PlayerObj>().GetComponent<PlayerLobbyUI>().P_playerReady.color = new Color32(r, g, b, 255);
            }

            if(_startMatch == true)
            {
                //_sceneManager.StartCharacterSelectScene(conn);
                //P_startMatch = _startMatch;
                //P_SDSceneM = MatchMaker.instance.GetMatches()[P_matchID].GetSDSceneManager();
                /*foreach(var player in MatchMaker.instance.GetMatches()[_matchID].GetPlayerObjList())
                {
                    Debug.Log("startmatch "+_startMatch+", playerobj netid: "+ player.GetComponent<PlayerObj>().ci.gameObject.GetComponent<NetworkIdentity>());
                    SDNetworkManager.LocalPlayersNetId.TryGetValue(player.GetComponent<PlayerObj>().ci.gameObject.GetComponent<NetworkIdentity>(), out NetworkConnection _conn);
                    TargetPlayersInMatch(_conn, MatchMaker.instance.GetMatches()[_matchID].GetSDSceneManager());
                }*/
            }
        }

        [ClientRpc]
        private void RpcUpdateExisting(GameObject _player, bool _startMatch, SDSceneManager _sceneManager)
        {
            if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 1)
                _player.transform.SetParent(team1GameObject.transform, false);
            else if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 2)
                _player.transform.SetParent(team2GameObject.transform, false);

            // range is 0-255
            byte r = 0;
            byte g = 0;
            byte b = 0;

            if(_player.GetComponent<PlayerObj>().GetIsReady() == false)
            {
                r = 255;
                g = 183;
                b = 136;

                _player.GetComponent<PlayerObj>().GetComponent<PlayerLobbyUI>().P_playerReady.text = "Not Ready";
                _player.GetComponent<PlayerObj>().GetComponent<PlayerLobbyUI>().P_playerReady.color = new Color32(r, g, b, 255);
            }
            else if(_player.GetComponent<PlayerObj>().GetIsReady() == true)
            {
                r = 211;
                g = 255;
                b = 136;

                _player.GetComponent<PlayerObj>().GetComponent<PlayerLobbyUI>().P_playerReady.text = "Ready!";
                _player.GetComponent<PlayerObj>().GetComponent<PlayerLobbyUI>().P_playerReady.color = new Color32(r, g, b, 255);
            }

            if(_startMatch == true)
            {
                _sceneManager.StartCharacterSelectScene();
                //P_startMatch = _startMatch;
                //P_SDSceneM = MatchMaker.instance.GetMatches()[P_matchID].GetSDSceneManager();
                /*foreach(var player in MatchMaker.instance.GetMatches()[_matchID].GetPlayerObjList())
                {
                    Debug.Log("startmatch "+_startMatch+", playerobj netid: "+ player.GetComponent<PlayerObj>().ci.gameObject.GetComponent<NetworkIdentity>());
                    SDNetworkManager.LocalPlayersNetId.TryGetValue(player.GetComponent<PlayerObj>().ci.gameObject.GetComponent<NetworkIdentity>(), out NetworkConnection _conn);
                    TargetPlayersInMatch(_conn, MatchMaker.instance.GetMatches()[_matchID].GetSDSceneManager());
                }*/
            }
        }

        private IEnumerator SetUI_StartReadyButton(GameObject _player)
        {
            while(startButton == null && readyButton == null)
                yield return "start/ready button missing!";

            //set start/ready buttons
            if(_player.GetComponent<PlayerObj>().GetIsHost() == true)
            {
                P_startButton.SetActive(true);
                P_readyButton.SetActive(false);
            }
            else if(_player.GetComponent<PlayerObj>().GetIsHost() == false)
            {
                if(!P_readyButton.activeSelf)
                {
                    P_startButton.SetActive(false);
                    P_readyButton.SetActive(true);
                }

                if(_player.GetComponent<PlayerObj>().GetIsReady() == true)
                    readyButton.transform.GetChild(0).GetComponent<Text>().text = "Cancel";
                else if(_player.GetComponent<PlayerObj>().GetIsReady() == false)
                    readyButton.transform.GetChild(0).GetComponent<Text>().text = "Ready";
            }
        }

        private IEnumerator SetUI_SwapButton(GameObject _player)
        {
            while(P_toTeam1Button == null && P_toTeam2Button == null)
                yield return "swap buttons button missing!";

            //set swap buttons
            if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 1)
            {
                P_toTeam1Button.SetActive(false);
                P_toTeam2Button.SetActive(true);
            }
            else if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 2)
            {
                P_toTeam1Button.SetActive(true);
                P_toTeam2Button.SetActive(false);
            }
        }

        #region Swap

        public void GoTeam1()
        {  
            if(isClient)
                StartCoroutine(SetUI_TeamSwap(1, PlayerObj.instance.gameObject));
        }

        public void GoTeam2()
        {
            if(isClient)
                StartCoroutine(SetUI_TeamSwap(2, PlayerObj.instance.gameObject));
        }

        private IEnumerator SetUI_TeamSwap(int GoTeam, GameObject _player)
        {
            while(GoTeam < 1 || GoTeam  > 2)
                yield return "invalid team number!";

            yield return StartCoroutine(RequestSwapUpdate(GoTeam, _player));
            StartCoroutine(SetUI_SwapButton(_player));
            StartCoroutine(RequestLobbyUpdate(_player.GetComponent<PlayerObj>().GetMatchID(), _player, false));
        }

        private IEnumerator RequestSwapUpdate(int GoTeam, GameObject _player)
        {
            CmdRequestSwapUpdate(GoTeam, _player);

            while(_player.GetComponent<PlayerObj>().GetTeamIndex() != GoTeam)
                yield return null;
        }

        [Command(ignoreAuthority = true)]
        private void CmdRequestSwapUpdate(int GoTeam, GameObject _player)
        {
            if(GoTeam == 1)
            {
                while(_player.GetComponent<PlayerObj>().GetTeamIndex() != 1)
                     _player.GetComponent<PlayerObj>().SetTeamIndex(1);

                MatchMaker.instance.GetMatches()[_player.GetComponent<PlayerObj>().GetMatchID()].AddTeam1Count();
                MatchMaker.instance.GetMatches()[_player.GetComponent<PlayerObj>().GetMatchID()].MinusTeam2Count();
            }
            else if(GoTeam == 2)
            {
                while(_player.GetComponent<PlayerObj>().GetTeamIndex() != 2)
                    _player.GetComponent<PlayerObj>().SetTeamIndex(2);
                
                MatchMaker.instance.GetMatches()[_player.GetComponent<PlayerObj>().GetMatchID()].AddTeam2Count();
                MatchMaker.instance.GetMatches()[_player.GetComponent<PlayerObj>().GetMatchID()].MinusTeam1Count();
            }

            Debug.Log("TEAM1: " + MatchMaker.instance.GetMatches()[_player.GetComponent<PlayerObj>().GetMatchID()].GetTeam1Count());
            Debug.Log("TEAM2: " + MatchMaker.instance.GetMatches()[_player.GetComponent<PlayerObj>().GetMatchID()].GetTeam2Count());
        }

        #endregion

        #region Start

        public void GoStart()
        {
            if(isClient)
                StartCoroutine(CheckStart(PlayerObj.instance.gameObject));
        }

        private IEnumerator CheckStart(GameObject _player)
        {
            if(_player.GetComponent<PlayerObj>().GetIsHost() == true)
            {
                yield return StartCoroutine(RequestCheckStart(_player));
            }   
        }

        private IEnumerator RequestCheckStart(GameObject _player)
        {
            CmdRequestCheckStart(_player);

            yield return null;
        }

        [Command(ignoreAuthority = true)]
        private void CmdRequestCheckStart(GameObject _player)
        {
            Debug.Log("--- BEGIN START CHECK " + _player.GetComponent<PlayerObj>().GetMatchID() + " ---");
            //Debug.Log("CHECK READY COUNT: " + MatchMaker.instance.GetMatches()[_player.GetComponent<PlayerObj>().GetMatchID()].GetReadyCount());
            //Debug.Log("CHECK PLAYERS COUNT: " + MatchMaker.instance.GetMatches()[_player.GetComponent<PlayerObj>().GetMatchID()].GetPlayerObjList().Count);
            //Debug.Log("CHECK TEAM1 COUNT:" + MatchMaker.instance.GetMatches()[_player.GetComponent<PlayerObj>().GetMatchID()].GetTeam1Count());
            //Debug.Log("CHECK TEAM2 COUNT:" + MatchMaker.instance.GetMatches()[_player.GetComponent<PlayerObj>().GetMatchID()].GetTeam2Count());

            string startStatusMsg = string.Empty;
            //bool startMatch = false;

            // if all players are ready
            if(MatchMaker.instance.GetMatches()[_player.GetComponent<PlayerObj>().GetMatchID()].GetReadyCount() ==
                MatchMaker.instance.GetMatches()[_player.GetComponent<PlayerObj>().GetMatchID()].GetPlayerObjList().Count)
            {
                Debug.Log("START PASS CHECK 1");
                startStatusMsg = "";
                startMatch = false;

                // if there's at least 1 player in each team
                if(MatchMaker.instance.GetMatches()[_player.GetComponent<PlayerObj>().GetMatchID()].GetTeam1Count() > 0 &&
                    MatchMaker.instance.GetMatches()[_player.GetComponent<PlayerObj>().GetMatchID()].GetTeam2Count() > 0)
                {
                        _player.GetComponent<PlayerObj>().SetIsReady(true);
                        startStatusMsg = "SUCCESS";
                        Debug.Log("START PASS CHECK 2, SUCCESS");

                        startMatch = true;
                }
                else
                {
                    Debug.Log("START CHECK 2 FAIL");

                    _player.GetComponent<PlayerObj>().SetIsReady(false);
                    if(MatchMaker.instance.GetMatches()[_player.GetComponent<PlayerObj>().GetMatchID()].GetTeam1Count() <= 0)
                        startStatusMsg  = "Team 1 is empty!";
                    else if(MatchMaker.instance.GetMatches()[_player.GetComponent<PlayerObj>().GetMatchID()].GetTeam2Count() <= 0)
                        startStatusMsg  = "Team 2 is empty!";
                    
                    startMatch = false;
                }
            }
            else
            {
                Debug.Log("START CHECK 1 FAIL");

                _player.GetComponent<PlayerObj>().SetIsReady(false);
                startStatusMsg  = "Some players not ready!";
                startMatch = false;
            }

            if(SDNetworkManager.LocalPlayersNetId.TryGetValue(_player.GetComponent<PlayerObj>().ci.gameObject.GetComponent<NetworkIdentity>(), out NetworkConnection conn))
                TargetRequestCheckStart(conn, startStatusMsg, _player, startMatch);

            Debug.Log("--- END START CHECK " + _player.GetComponent<PlayerObj>().GetMatchID() + " ---");
        }

        [TargetRpc]
        private void TargetRequestCheckStart(NetworkConnection conn, string _startStatusMsg, GameObject _player, bool _startMatch)
        {
            Debug.Log("startmatch true/false? "+_startMatch);
            P_startStatusText.GetComponent<Text>().text = _startStatusMsg;
            StartCoroutine(RequestLobbyUpdate(_player.GetComponent<PlayerObj>().GetMatchID(), _player, _startMatch));
        }

        #endregion

        #region Ready
        public void GoReady()
        {
            if(isClient)
                ChangeReady(PlayerObj.instance.gameObject);
        }

        private void ChangeReady(GameObject _player)
        {
            if(_player.GetComponent<PlayerObj>().GetIsReady() == true)
                StartCoroutine(SetUI_Ready(false, _player));
            else if(_player.GetComponent<PlayerObj>().GetIsReady() == false)
                StartCoroutine(SetUI_Ready(true, _player));
        }

        private IEnumerator SetUI_Ready(bool ChangeReady, GameObject _player)
        {
            yield return StartCoroutine(RequestReadyUpdate(ChangeReady, _player));
            StartCoroutine(SetUI_StartReadyButton(_player));
            StartCoroutine(RequestLobbyUpdate(_player.GetComponent<PlayerObj>().GetMatchID(), _player, false));
        }

        private IEnumerator RequestReadyUpdate(bool ChangeReady, GameObject _player)
        {
            CmdRequestReadyUpdate(ChangeReady, _player);

            while(ChangeReady != _player.GetComponent<PlayerObj>().GetIsReady())
                yield return null;     
        }

        [Command(ignoreAuthority = true)]
        private void CmdRequestReadyUpdate(bool ChangeReady, GameObject _player)
        {
            _player.GetComponent<PlayerObj>().SetIsReady(ChangeReady);
            
            if(_player.GetComponent<PlayerObj>().GetIsReady() == true)
                MatchMaker.instance.GetMatches()[_player.GetComponent<PlayerObj>().GetMatchID()].AddCountReady();
            else if(_player.GetComponent<PlayerObj>().GetIsReady() == false)
                MatchMaker.instance.GetMatches()[_player.GetComponent<PlayerObj>().GetMatchID()].MinusCountReady();

            Debug.Log(_player.GetComponent<PlayerObj>().GetMatchID() + " READY COUNT: " + MatchMaker.instance.GetMatches()[_player.GetComponent<PlayerObj>().GetMatchID()].GetReadyCount());
        }
        
        #endregion

        #endregion

        #region Server & Client Functions

        private IEnumerator SetUI_Lobby(string _matchID)
        {
            if(isClient)
            {
                while(_matchID == string.Empty)
                    yield return null;

                P_matchIDText.GetComponent<Text>().text = _matchID;
                Debug.Log("Player object's matchID: " + PlayerObj.instance.GetMatchID() + " = " + "lobbyUI's matchID: " + _matchID);

                StartCoroutine(SetUI_StartReadyButton(PlayerObj.instance.gameObject));
                StartCoroutine(SetUI_SwapButton(PlayerObj.instance.gameObject));
                StartCoroutine(RequestLobbyUpdate(_matchID, PlayerObj.instance.gameObject, false));
            }
            if(isServer)
            {
                while(!MatchMaker.instance.GetMatches()[_matchID].GetSDSceneManager().P_lobbySceneLoaded)
                    yield return null;

                foreach(var _player in MatchMaker.instance.GetMatches()[_matchID].GetPlayerObjList())
                {
                    Debug.Log("Player matchID: " + _player.GetComponent<PlayerObj>().GetMatchID());
                    Debug.Log("Player index: " + _player.GetComponent<PlayerObj>().GetTeamIndex());

                    if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 1)
                        _player.GetComponent<PlayerObj>().gameObject.transform.SetParent(MatchMaker.instance.GetMatches()[_matchID].GetLobbyUIManager().P_team1GameObject.transform, false);
                    else if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 2)
                        _player.GetComponent<PlayerObj>().gameObject.transform.SetParent(MatchMaker.instance.GetMatches()[_matchID].GetLobbyUIManager().P_team2GameObject.transform, false);
                }
            }
        }

        #endregion

        #region Start & Stop Callbacks

        /// <summary>
        /// This is invoked for NetworkBehaviour objects when they become active on the server.
        /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
        /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
        /// </summary>
        public override void OnStartServer()
        {
            instance = this;
            MatchMaker.instance.GetMatches()[SDSceneManager.instance.P_matchID].SetLobbyUIManager(instance);
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