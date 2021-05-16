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
        [SyncVar] private string startStatusMsg = string.Empty;
        
        //room matchID
        [SerializeField]
        [SyncVar] private string matchID = string.Empty;
        private bool matchIDSet = false;
        
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

        public string P_startStatusMsg
        {
            get{return startStatusMsg;}
            set{startStatusMsg = value;}
        }

        #endregion

        private void Start()
        {
            if(isClient)
                ClientStartSetting();
        }

        void Update()
        {
            if(isServer && P_matchID != string.Empty && !matchIDSet)
            {
                ServerStartSetting(P_matchID);
                matchIDSet = true;
                return;
            }
        }

        #region Server Functions

        private void ServerStartSetting(string _matchID)
        {
            //begin setting matchID
            SetUI_MatchID(_matchID);
        }

        [Server]
        private void SetUI_MatchID(string _matchID)
        {
            if(P_matchIDText.GetComponent<Text>().text == string.Empty || P_matchIDText.GetComponent<Text>().text != _matchID)
            {
                P_matchID = _matchID;
                P_matchIDText.GetComponent<Text>().text = P_matchID;
            }

            StartCoroutine(SetUI_Lobby(P_matchID));
        }

        [Command(ignoreAuthority = true)]
        private void CmdRequestLobbyUpdate(string _matchID, GameObject _player, bool _swap, bool _ready, bool _start, bool _exit)
        {
            NetworkConnection conn = _player.GetComponent<NetworkIdentity>().connectionToClient;
            if (conn != null)
            {
                if (!_swap && !_ready && !_start && !_exit)
                {
                    StartCoroutine(SetUI_Lobby(_matchID));
                    foreach (var player in MatchMaker.instance.GetMatches()[_matchID].GetPlayerObjList())
                        TargetUpdateOwner(conn, player, _swap, _ready, _start, false);

                    RpcUpdateOthers(_player, _swap, _ready, false);
                }
                else if (_swap)
                {
                    RequestSwapUpdate(_matchID, _player); // update swap on the server, causes a syncvar delay
                    TargetUpdateOwner(conn, _player, _swap, _ready, _start, false);
                    RpcUpdateOthers(_player, _swap, _ready, false);
                }
                else if (_ready)
                {
                    RequestReadyUpdate(_matchID, _player); // update ready on the server, causes a syncvar delay
                    TargetUpdateOwner(conn, _player, _swap, _ready, _start, false);
                    RpcUpdateOthers(_player, _swap, _ready, false);
                }
                else if (_start)
                {
                    RequestCheckStart(_matchID, _player);
                    TargetUpdateOwner(conn, _player, _swap, _ready, _start, false); // only need to show host player start status text
                }
                else if (_exit)
                    _player.GetComponent<LeaveGame>().Exit(_matchID, true, false, false, true);

            }
            else
            {
                Debug.Log("Connection with this netID does not exist");
            }
        }

        [Server]
        private void RequestSwapUpdate(string _matchID, GameObject _player)
        {
            if (_player.GetComponent<PlayerObj>().GetTeamIndex() == 1) // go to team 2
            {
                _player.GetComponent<PlayerObj>().SetTeamIndex(2); // syncvar delay here
                MatchMaker.instance.GetMatches()[_matchID].AddTeam2Count();
                MatchMaker.instance.GetMatches()[_matchID].MinusTeam1Count();
            }
            else if (_player.GetComponent<PlayerObj>().GetTeamIndex() == 2) // go to team 1
            {
                _player.GetComponent<PlayerObj>().SetTeamIndex(1); // syncvar delay here
                MatchMaker.instance.GetMatches()[_matchID].AddTeam1Count();
                MatchMaker.instance.GetMatches()[_matchID].MinusTeam2Count();
            }
            StartCoroutine(SetUI_Lobby(_matchID));
        }

        [Server]
        private void RequestReadyUpdate(string _matchID, GameObject _player)
        {
            if (_player.GetComponent<PlayerObj>().GetIsReady())
            {
                _player.GetComponent<PlayerObj>().SetIsReady(false);
                MatchMaker.instance.GetMatches()[_matchID].MinusCountReady();
            }
            else if (!_player.GetComponent<PlayerObj>().GetIsReady())
            {
                _player.GetComponent<PlayerObj>().SetIsReady(true);
                MatchMaker.instance.GetMatches()[_matchID].AddCountReady();
            }
            SetUI_StartReadyUI(_player, false);
        }

        [Server]
        private void RequestCheckStart(string _matchID, GameObject _player)
        {
            if(MatchMaker.instance.GetMatches()[_matchID].GetTeam1Count() == MatchMaker.instance.GetMatches()[_matchID].GetTeam2Count()) // equal number of players on both sides
            {
                if (MatchMaker.instance.GetMatches()[_matchID].GetPlayerObjList().Count == MatchMaker.instance.GetMatches()[_matchID].GetReadyCount()) // all ready
                {
                    MatchMaker.instance.GetMatches()[_matchID].GetLobbyUIManager().P_startStatusMsg = "Game starting..";
                    _player.GetComponent<StartGame>().StartNewScene(_matchID);
                }
                else
                    MatchMaker.instance.GetMatches()[_matchID].GetLobbyUIManager().P_startStatusMsg = "Some players not ready!";
            }
            else
                MatchMaker.instance.GetMatches()[_matchID].GetLobbyUIManager().P_startStatusMsg = "Not enough players!";

            MatchMaker.instance.GetMatches()[_matchID].GetLobbyUIManager().P_startStatusText.GetComponent<Text>().text = MatchMaker.instance.GetMatches()[_matchID].GetLobbyUIManager().P_startStatusMsg;
        }

        [Server]
        public void HostLeftLobby(GameObject _player)
        {
            SetUI_StartReadyUI(_player, false); // change on server
            TargetUpdateOwner(_player.GetComponent<NetworkIdentity>().connectionToClient, _player, false, false, false, true);
            RpcUpdateOthers(_player, false, false, true); // change on client
        }

        #endregion

        #region Client Functions

        public void ClientStartSetting()
        {
            StartCoroutine(SetUI_Lobby(P_matchID));
        }

        [TargetRpc]
        private void TargetUpdateOwner(NetworkConnection conn, GameObject _player, bool _swap, bool _ready, bool _start, bool _hostChange)
        {
            bool isOwner = true;
            if (!_swap && !_ready && !_start && !_hostChange) // for host/join
            {
                if (_player.GetComponent<PlayerObj>().GetTeamIndex() == 1)
                    _player.transform.SetParent(P_team1GameObject.transform, false);
                else if (_player.GetComponent<PlayerObj>().GetTeamIndex() == 2)
                    _player.transform.SetParent(P_team2GameObject.transform, false);
                SetUI_SwapButton(_player.GetComponent<PlayerObj>().GetTeamIndex());
                SetUI_StartReadyUI(_player, isOwner);
            }
            else if (_swap) // for swap
            {
                // deal with syncvar delay
                if (_player.GetComponent<PlayerObj>().GetTeamIndex() == 1)
                    StartCoroutine(WaitForTeamUpdate(_player, 1, isOwner));
                else if (_player.GetComponent<PlayerObj>().GetTeamIndex() == 2)
                    StartCoroutine(WaitForTeamUpdate(_player, 2, isOwner));
            }
            else if (_ready) // for ready
                StartCoroutine(WaitForReadyUpdate(_player, _player.GetComponent<PlayerObj>().GetIsReady(), isOwner)); // deal with syncvar delay
            else if (_start) // for start
                StartCoroutine(WaitForStartStatusUpdate(P_startStatusMsg)); // deal with syncvar delay
            else if (_hostChange)
                StartCoroutine(WaitForHostChangeUpdate(_player, _player.GetComponent<PlayerObj>().GetIsHost(), isOwner));
        }

        [ClientRpc]
        private void RpcUpdateOthers(GameObject _player, bool _swap, bool _ready, bool _hostChange)
        {
            bool isOwner = false;
            if (!_swap && !_ready && !_hostChange) // for host/join
            {
                if (_player.GetComponent<PlayerObj>().GetTeamIndex() == 1)
                    _player.transform.SetParent(P_team1GameObject.transform, false);
                else if (_player.GetComponent<PlayerObj>().GetTeamIndex() == 2)
                    _player.transform.SetParent(P_team2GameObject.transform, false);
                SetUI_StartReadyUI(_player, isOwner);
            }
            else if (_swap) // for swap
            {
                // deal with syncvar delay
                if (_player.GetComponent<PlayerObj>().GetTeamIndex() == 1)
                    StartCoroutine(WaitForTeamUpdate(_player, 1, isOwner));
                else if (_player.GetComponent<PlayerObj>().GetTeamIndex() == 2)
                    StartCoroutine(WaitForTeamUpdate(_player, 2, isOwner));
            }
            else if (_ready) // for ready
                StartCoroutine(WaitForReadyUpdate(_player, _player.GetComponent<PlayerObj>().GetIsReady(), isOwner)); // deal with syncvar delay
            else if (_hostChange)
                StartCoroutine(WaitForHostChangeUpdate(_player, _player.GetComponent<PlayerObj>().GetIsHost(), isOwner));
        }

        private IEnumerator WaitForTeamUpdate(GameObject _player, int oldTeamIndex, bool isOwner)
        {
            while (_player.GetComponent<PlayerObj>().GetTeamIndex() == oldTeamIndex)
                yield return null;
            if (_player.GetComponent<PlayerObj>().GetTeamIndex() == 1)
                _player.transform.SetParent(P_team1GameObject.transform, false);
            else if (_player.GetComponent<PlayerObj>().GetTeamIndex() == 2)
                _player.transform.SetParent(P_team2GameObject.transform, false);

            if(isOwner)
                SetUI_SwapButton(_player.GetComponent<PlayerObj>().GetTeamIndex()); // only need to set button on owner, other clients cant see ur button
        }

        private IEnumerator WaitForReadyUpdate(GameObject _player, bool oldReadyState, bool isOwner)
        {
            while (_player.GetComponent<PlayerObj>().GetIsReady() == oldReadyState)
                yield return null;
            SetUI_StartReadyUI(_player, isOwner);
        }

        private IEnumerator WaitForStartStatusUpdate(string oldStatus)
        {
            while (P_startStatusMsg == oldStatus)
                yield return null;
            P_startStatusText.GetComponent<Text>().text = P_startStatusMsg;
        }

        private IEnumerator WaitForHostChangeUpdate(GameObject _player, bool oldHostStatus, bool _isOwner)
        {
            while (_player.GetComponent<PlayerObj>().GetIsHost() == oldHostStatus)
                yield return null;
            SetUI_StartReadyUI(_player, _isOwner);
        }
        
        [Client] // show for owner only since these buttons shouldn't be available to other clients, don't need to show on server because...there is only 1 server
        private void SetUI_SwapButton(int teamIndex)
        {
            if(teamIndex == 1)
            {
                P_toTeam1Button.SetActive(false);
                P_toTeam2Button.SetActive(true);
            }
            else if(teamIndex == 2)
            {
                P_toTeam1Button.SetActive(true);
                P_toTeam2Button.SetActive(false);
            }
        }

        #region Swap

        public void GoTeam1()
        {
            GameObject _player = PlayerObj.instance.gameObject;
            if (_player.GetComponent<NetworkIdentity>().hasAuthority)
                CmdRequestLobbyUpdate(_player.GetComponent<PlayerObj>().GetMatchID(), _player, true, false, false, false);
        }

        public void GoTeam2()
        {
            GameObject _player = PlayerObj.instance.gameObject;
            if (_player.GetComponent<NetworkIdentity>().hasAuthority)
                CmdRequestLobbyUpdate(_player.GetComponent<PlayerObj>().GetMatchID(), _player, true, false, false, false);
        }

        #endregion

        #region Start
        public void GoStart()
        {
            GameObject _player = PlayerObj.instance.gameObject;
            if(_player.GetComponent<NetworkIdentity>().hasAuthority && _player.GetComponent<PlayerObj>().GetIsHost())
                CmdRequestLobbyUpdate(_player.GetComponent<PlayerObj>().GetMatchID(), _player, false, false, true, false);
        }
        #endregion

        #region Ready
        public void GoReady()
        {
            GameObject _player = PlayerObj.instance.gameObject;
            if (_player.GetComponent<NetworkIdentity>().hasAuthority)
                CmdRequestLobbyUpdate(_player.GetComponent<PlayerObj>().GetMatchID(), _player, false, true, false, false);
        }
        #endregion

        #region Leave
        public void GoLeave()
        {
            GameObject _player = PlayerObj.instance.gameObject;
            if(_player.GetComponent<NetworkIdentity>().hasAuthority)
                CmdRequestLobbyUpdate(_player.GetComponent<PlayerObj>().GetMatchID(), _player, false, false, false, true);
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
                GameObject _player = PlayerObj.instance.gameObject;
                if (_player.GetComponent<NetworkIdentity>().hasAuthority)
                {
                    P_matchIDText.GetComponent<Text>().text = _matchID;
                    CmdRequestLobbyUpdate(_matchID, _player, false, false, false, false);
                }
            }

            if(isServer)
            {
                while(!MatchMaker.instance.GetMatches()[_matchID].GetSDSceneManager().P_scenesLoaded)
                    yield return null;

                foreach(var _player in MatchMaker.instance.GetMatches()[_matchID].GetPlayerObjList())
                {
                    if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 1)
                        _player.GetComponent<PlayerObj>().gameObject.transform.SetParent(MatchMaker.instance.GetMatches()[_matchID].GetLobbyUIManager().P_team1GameObject.transform, false);
                    else if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 2)
                        _player.GetComponent<PlayerObj>().gameObject.transform.SetParent(MatchMaker.instance.GetMatches()[_matchID].GetLobbyUIManager().P_team2GameObject.transform, false);
                    SetUI_StartReadyUI(_player, false);
                }
            }
        }

        // has authority checked from SetUILobby, so should always run on the correct client
        // for host/join
        private void SetUI_StartReadyUI(GameObject _player, bool isOwner)
        {
            if (isClient && isOwner)
            {
                if (_player.GetComponent<PlayerObj>().GetIsHost())
                {
                    P_startButton.SetActive(true);
                    P_readyButton.SetActive(false);
                }
                else if (!_player.GetComponent<PlayerObj>().GetIsHost())
                {
                    if (!P_readyButton.activeSelf)
                    {
                        P_startButton.SetActive(false);
                        P_readyButton.SetActive(true);
                    }

                    if (_player.GetComponent<PlayerObj>().GetIsReady())
                        readyButton.transform.GetChild(0).GetComponent<Text>().text = "Cancel";
                    else if (!_player.GetComponent<PlayerObj>().GetIsReady())
                        readyButton.transform.GetChild(0).GetComponent<Text>().text = "Ready";
                }
            }

            // show on everyone
            if (isServer || isClient)
            {
                byte r = 0;
                byte g = 0;
                byte b = 0;

                if (!_player.GetComponent<PlayerObj>().GetIsReady())
                {
                    r = 255;
                    g = 183;
                    b = 136;
                    if (_player.GetComponent<PlayerObj>().GetIsHost())
                        _player.GetComponent<PlayerLobbyUI>().P_playerReady.text = "Host";
                    else
                        _player.GetComponent<PlayerLobbyUI>().P_playerReady.text = "Not Ready";
                    _player.GetComponent<PlayerLobbyUI>().P_playerReady.color = new Color32(r, g, b, 255);
                }
                else if (_player.GetComponent<PlayerObj>().GetIsReady())
                {
                    r = 211;
                    g = 255;
                    b = 136;
                    _player.GetComponent<PlayerLobbyUI>().P_playerReady.text = "Ready!";
                    _player.GetComponent<PlayerLobbyUI>().P_playerReady.color = new Color32(r, g, b, 255);
                }
            }
        }

        #endregion

        #region Debug

        public void ForceStart()
        {
            CmdForceStart(PlayerObj.instance.gameObject, PlayerObj.instance.GetMatchID());
        }

        [Command(ignoreAuthority = true)]
        private void CmdForceStart(GameObject _player, string _matchID)
        {
            _player.GetComponent<StartGame>().StartNewScene(_matchID);
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