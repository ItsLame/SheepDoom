using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections.Generic;
using System.Collections;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

/*namespace SheepDoom
{
    public delegate void UIEvent();

    public class NetworkLobbyUI : NetworkBehaviour
    {
        public event UIEvent uiEvent;

        [Header("Lobby UI Setup")]
        public static NetworkLobbyUI instance;
        [SerializeField] private Text MatchIDText;
        [SerializeField] private GameObject Team1GameObject;
        [SerializeField] private GameObject Team2GameObject;
        [SyncVar] private string _matchID = string.Empty;
        [SyncVar] private int _matchIndex = 0;
        [SyncVar] private int team1Count = 0;
        [SyncVar] private int team2Count = 0;

        private GameObject _myPlayer = null;
        private GameObject _player = null;
        [SyncVar] private SyncList<PlayerObj> _playerList = new SyncList<PlayerObj>();
        [SyncVar] private LobbyManager _myLobby;

        #region Properties
        
        public int myTeam1Count
        {
            get
            {
                return team1Count;
            }
            set
            {
                team1Count = value;
                if(uiEvent != null)
                    uiEvent();
            }
        }

        public int myTeam2Count
        {
            get
            {
                return team2Count;
            }
            set
            {
                team2Count = value;
                if(uiEvent != null)
                    uiEvent();
            }
        }

        #endregion

        /*public void Start()
        {
            StartSetting();
            
            if(LobbyManager.instance)
            {
                //_myLobby = MatchMaker.instance.GetLobby().GetComponent<LobbyManager>();
                //_myLobby.lobbyEvent += UpdateUI_Team;
                //uiEvent += UpdateUI_Team;

                //LobbyManager.instance.lobbyEvent += UpdateUI_Team; //subscribe to event
            }
            
            //Debug.Log("mylobby is: " + _myLobby);
            //Debug.Log("mylobby myteam1count: " + _myLobby.myTeam1Count);
        }*/
/*
        private void StartSetting()
        {
            this.StartCoroutine(SetUI_MatchID());
        }

        private void Update()
        {
            StartSetting();
        }

        IEnumerator SetUI_MatchID()
        {
            //get matchid from lobby manager first (from server)
            if(LobbyManager.instance)
            {
                if(_matchID == string.Empty)
                {
                    _matchID = LobbyManager.instance.GetMatchID();
                    _matchIndex = LobbyManager.instance.GetMatchIndex();

                    while(_matchID == null)
                    {
                        Debug.Log("matchID still null!");
                        yield return null;
                    }

                    Debug.Log("matchID not null anymore!");
                }
            }

            //set matchID to UI (server & client)
            MatchIDText.text = _matchID;

            //if already set, start coroutine set team UI
            if(MatchIDText.text != string.Empty)
                StartCoroutine(SetUI_Team());
        }
        
        IEnumerator SetUI_Team()
        {
            //set player object transform parent
            if(LobbyManager.instance)
            {
                while(LobbyManager.instance.myTeam1Count != MatchMaker.instance.GetTeam1Count() &&
                    LobbyManager.instance.myTeam2Count != MatchMaker.instance.GetTeam2Count())
                {
                    Debug.Log("both team1count doesn't match!");

                    yield return null;
                }
            }

             Debug.Log("detected changes to teamcount! updating...");
             //UpdateUI_Team();

             //TEMPORARY
            if(PlayerObj.instance.GetTeamIndex() == 1)
            {
                Debug.Log("updating team1 UI");
                PlayerObj.instance.transform.parent = Team1GameObject.transform;
            }
            if(PlayerObj.instance.GetTeamIndex() == 2)
            {
                Debug.Log("updating team2 UI");
                PlayerObj.instance.transform.parent = Team1GameObject.transform; 
            }
        }
        
        #region Spawn Player (Very Broken)

        /*private void UpdateUI_Team()
        {
            if(LobbyManager.instance)
            {
                /*foreach(GameObject getPlayer in MatchMaker.instance.GetPlayerObjList(_matchIndex))
                {
                    if(!_playerList.Contains(getPlayer.GetComponent<PlayerObj>()))
                        _playerList.Add(getPlayer.GetComponent<PlayerObj>());
                }

                foreach(PlayerObj getPlayerObj in _playerList)
                {
                    if(getPlayerObj.GetTeamIndex() == 1)
                    {
                        Debug.Log("updating team1 UI");
                        getPlayerObj.transform.parent = Team1GameObject.transform;
                    }
                    if(getPlayerObj.GetTeamIndex() == 2)
                    {
                        Debug.Log("updating team2 UI");
                        getPlayerObj.transform.parent = Team2GameObject.transform; 
                    }
                }*/

                //Debug.Log("myteam1count: " + LobbyManager.instance.myTeam1Count);
                //Debug.Log("myteam2count: " + LobbyManager.instance.myTeam2Count);

                /*if(MatchMaker.instance.GetPlayerObjList(_matchIndex).Count == 1)
                {
                    _player = MatchMaker.instance.GetPlayerObjList(_matchIndex)[0];

                    if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 1)
                    {
                        Debug.Log("updating team1 UI");
                        _player.transform.parent = Team1GameObject.transform;
                    }
                    if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 2)
                    {
                        Debug.Log("updating team2 UI");
                        _player.transform.parent = Team2GameObject.transform; 
                    }
                }
                if(MatchMaker.instance.GetPlayerObjList(_matchIndex).Count > 1)
                {*/
                   /* for(int i = 0 ; i != MatchMaker.instance.GetPlayerObjList(_matchIndex).Count-1 ; i++)
                    {
                        _player = MatchMaker.instance.GetPlayerObjList(_matchIndex)[i];

                        if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 1)
                        {
                            Debug.Log("updating team1 UI");
                            _player.transform.parent = Team1GameObject.transform;
                        }
                        if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 2)
                        {
                            Debug.Log("updating team2 UI");
                            _player.transform.parent = Team2GameObject.transform; 
                        }
                    }*/
                //}
            //}
            //if(!LobbyManager.instance)
            //{
                //PlayerObj.instance.transform.parent = Team1GameObject.transform;
                /*
                foreach(PlayerObj getPlayerObj in _playerList)
                {
                    if(_playerList.Contains(_myPlayer.GetComponent<PlayerObj>()))
                    {
                        int index = _playerList.IndexOf(_myPlayer.GetComponent<PlayerObj>());
                        PlayerObj pobj = _playerList[index];
                        pobj.transform.parent = Team1GameObject.transform;
                    }
                }*/
            //}
        //}
/*
        #endregion

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
}*/
