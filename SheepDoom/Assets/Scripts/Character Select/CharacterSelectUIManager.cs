using System;
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
        [SerializeField] private Text timerText;
        private TimeSpan timePlaying;
        private float secondsTimer = 10;
        private bool playersInScene = false;

        [Header("Inputs For Client")]
        [SerializeField] private Image heroInfoImg;
        [SerializeField] private Text heroInfoText;
        [SerializeField] private GameObject lockInButton;
        [SerializeField] private GameObject statusPanel;

        [Header("Characters")]
        [SerializeField] private GameObject mario;
        [SerializeField] private GameObject luigi;
        [SerializeField] private GameObject peach;
        [SerializeField] private GameObject yoshi;
        [SerializeField] private GameObject bowser;
        private string[] heroesArr = {"Mario", "Luigi", "Peach", "Yoshi", "Bowser"};
        public SyncList<string> team1PickedHeroes = new SyncList<string>();
        public SyncList<string> team2PickedHeroes = new SyncList<string>();
        //public SyncList<GameObject> notLockedInPlayers = new SyncList<GameObject>();
        //room matchID
        [SerializeField]
        [SyncVar] private string matchID = string.Empty;
        [SyncVar] private string timePlayingStr = "60";

        // so that don't share the same random result (but share the same system.random)
        private static readonly System.Random rand = new System.Random();

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

        public string P_matchID
        {
            get{return matchID;}
            set{matchID = value;}
        }

        public Image P_heroInfoImg
        {
            get{return heroInfoImg;}
            set{heroInfoImg = value;}
        }

        public Text P_heroInfoText
        {
            get{return heroInfoText;}
            set{heroInfoText = value;}
        }

        public GameObject P_lockInButton
        {
            get{return lockInButton;}
            set{lockInButton = value;}
        }

        public GameObject P_statusPanel
        {
            get{return statusPanel;}
            set{statusPanel = value;}
        }

        #endregion
        [Server]
        public void StartCharSelect(string _matchID)
        {
            ServerStartSetting(_matchID);
            playersInScene = true;
        }
        private void Start()
        {
            if(isClient)
                ClientStartSetting();
        }

        void Update()
        {
            if (isServer && playersInScene)
            {
                if(secondsTimer > 0)
                {
                    secondsTimer -= Time.deltaTime;
                    timePlaying = TimeSpan.FromSeconds(secondsTimer);
                    timePlayingStr = timePlaying.ToString("%s");
                    timerText.text = timePlayingStr;
                }
                else
                {
                    RequestGameStart();
                    // random lockin for players who havent locked in, then start game
                }
            }

            if (isClient)
                timerText.text = timePlayingStr;
        }

        #region Server Functions

        private void ServerStartSetting(string _matchID)
        {
            P_matchID = _matchID;
            lockInButton.SetActive(false);  // so that server won't have lock in button
        }

        [Command(ignoreAuthority = true)]
        private void CmdRequestCharSelectUpdate(GameObject _player, string _heroName, int _teamIndex, bool _init, bool _lockIn, bool _isGameStart)
        {
            if(SDNetworkManager.LocalPlayersNetId.TryGetValue(_player.GetComponent<PlayerObj>().ci.gameObject.GetComponent<NetworkIdentity>(), out NetworkConnection conn))
            {
                if(_init)
                {
                    Debug.Log("CMD INIT");
                    SetUI_Player(_player);  // server view
                    SetUI_Team(_player, _teamIndex);    // server view
                    SetUI_Hero(_player, _heroName, _lockIn);    // server view
                    TargetUpdateOwner(conn, _player, _heroName, true, _init, _lockIn);  // local client view
                }
                else if(_lockIn)
                {
                    //Debug.Log("CMD LOCK IN");
                    bool _isOwner = false;

                    MatchMaker.instance.GetMatches()[P_matchID].AddCountLockIn();   // lock in true, count lock in ++
                    _player.GetComponent<PlayerObj>().SetIsLockedIn(true);
                    SetUI_Hero(_player, _heroName, _lockIn);
                    foreach (var player in MatchMaker.instance.GetMatches()[P_matchID].GetPlayerObjList())
                    {
                        if(player.GetComponent<PlayerObj>().GetTeamIndex() == _teamIndex)
                        {
                            if(player.GetComponent<PlayerObj>().ci == _player.GetComponent<PlayerObj>().ci)
                                _isOwner = true;
                            else
                                _isOwner = false;

                            if(SDNetworkManager.LocalPlayersNetId.TryGetValue(player.GetComponent<PlayerObj>().ci.GetComponent<NetworkIdentity>(), out NetworkConnection _conn))
                                TargetUpdateOwner(_conn, player, _heroName, _isOwner, _init, _lockIn);  // 4th parameter determines if it's owner
                        }
                    }
                }
                else if(!_lockIn)
                    SetUI_Hero(_player, _heroName, _lockIn);    // server view
                    //TargetUpdateOwner(conn, _player, _heroName, true, _init, _lockIn); // local client view

                RpcUpdateOthers(_player, _heroName, _teamIndex, _init, _lockIn);    // other client view
            }
            else
            {
                Debug.Log("Connection with this netID does not exist");
            }
        }

        [Server] // based on timer
        private void RequestGameStart()
        {
            if(MatchMaker.instance.GetMatches()[P_matchID].GetLockInCount() != MatchMaker.instance.GetMatches()[P_matchID].GetPlayerObjList().Count) // check whether all locked in
            {
                foreach (GameObject _player in MatchMaker.instance.GetMatches()[P_matchID].GetPlayerObjList())
                {
                    // check again after each loop
                    if(MatchMaker.instance.GetMatches()[P_matchID].GetLockInCount() == MatchMaker.instance.GetMatches()[P_matchID].GetPlayerObjList().Count)
                        break;

                    if (_player.GetComponent<PlayerObj>().GetIsLockedIn())
                    {
                        string _heroName = _player.GetComponent<PlayerObj>().GetHeroName();
                        if (_player.GetComponent<PlayerObj>().GetTeamIndex() == 1)
                            team1PickedHeroes.Add(_heroName);
                        else if (_player.GetComponent<PlayerObj>().GetTeamIndex() == 2)
                            team2PickedHeroes.Add(_heroName);
                    }
                    else if(!_player.GetComponent<PlayerObj>().GetIsLockedIn())
                        AutoLockIn(_player);
                }

                // in for loop with notlockedinplayers.count
                // do
                //random a number 1-5
                // if 1 = mario, 2 = bowser, 3 = luigi, 4 = peach, 5 = yoshi
                //
                // while hero in pickedheroes
            }
            //else
                // start game smoothly
        }

        private void AutoLockIn(GameObject _player)
        {
            //System.Random rand = new System.Random();

            int i = rand.Next(0, 5);

            if (_player.GetComponent<PlayerObj>().GetTeamIndex() == 1)
            {
                while(team1PickedHeroes.Contains(heroesArr[i]))
                    i = rand.Next(0, 5);

                team1PickedHeroes.Add(heroesArr[i]);
            }
            else if (_player.GetComponent<PlayerObj>().GetTeamIndex() == 2)
            {
                while(team2PickedHeroes.Contains(heroesArr[i]))
                    i = rand.Next(0, 5);

                team2PickedHeroes.Add(heroesArr[i]);
            }
            
            _player.GetComponent<PlayerObj>().SetHeroName(heroesArr[i]);
            _player.GetComponent<PlayerObj>().SetIsLockedIn(true);
            MatchMaker.instance.GetMatches()[P_matchID].AddCountLockIn();

            if(SDNetworkManager.LocalPlayersNetId.TryGetValue(_player.GetComponent<PlayerObj>().ci.gameObject.GetComponent<NetworkIdentity>(), out NetworkConnection conn))
                TargetUpdateOwner(conn, _player, _player.GetComponent<PlayerObj>().GetHeroName(), true, false, true);
            
            RpcUpdateOthers(_player, _player.GetComponent<PlayerObj>().GetHeroName(), _player.GetComponent<PlayerObj>().GetTeamIndex(), false, true);
        }

        #endregion

        #region Client Functions
        private void ClientStartSetting()
        {
            GameObject _player = PlayerObj.instance.gameObject;
            int _teamIndex = _player.GetComponent<PlayerObj>().GetTeamIndex();

            lockInButton.GetComponent<Button>().interactable = false;   // false since haven't setup hero UI yet (prevent null lock in)

            if(_player.GetComponent<NetworkIdentity>().hasAuthority)
                CmdRequestCharSelectUpdate(_player, string.Empty, _teamIndex, true, false, false);  // 4th parameter set to true to set initial settings
        }

        [Client]
        public void ClientRequestUpdate(string _heroName, bool _lockIn)
        {
            GameObject _player = PlayerObj.instance.gameObject;
            int _teamIndex = _player.GetComponent<PlayerObj>().GetTeamIndex();

            if(_player.GetComponent<NetworkIdentity>().hasAuthority)
                CmdRequestCharSelectUpdate(_player, _heroName, _teamIndex, false, _lockIn, false); // 5th parameter set to true to start lock in request
        }

        public void LockInHero()
        {
            GameObject _player = PlayerObj.instance.gameObject;
            if(_player.GetComponent<NetworkIdentity>().hasAuthority)
            {
                string _heroName = _player.GetComponent<PlayerObj>().GetHeroName();
                Debug.Log("HERONAME: " + _heroName);

                ClientRequestUpdate(_heroName, true);
            }
        }

        [ClientRpc]
        private void RpcUpdateOthers(GameObject _player, string _heroName, int _teamIndex, bool _init, bool _lockIn)
        {
            if(_init)
            {
                SetUI_Player(_player);
                SetUI_Team(_player, _teamIndex);
            }

            //Debug.Log("Player: "+_player.GetComponent<PlayerObj>().GetPlayerName()+"\tSet HeroName: "+_heroName+"\nInit Status: "+_init+"\tLock In Status: "+_lockIn);

            SetUI_Hero(_player, _heroName, _lockIn);
        }

        [TargetRpc]
        private void TargetUpdateOwner(NetworkConnection conn, GameObject _player, string _heroName, bool _isOwner, bool _init, bool _lockIn)
        {
            if(_init)
                SetUI_Parent(_player);
            else if(_lockIn)
            {
                // when other players (from same team) attempt to pick the hero, lock in button will be disabled
                //GameObject.Find(_heroName).SendMessage("SetTaken", _lockIn);
                //GameObject.Find(_heroName).SendMessage("OnClickHero");

                //Hero hero = null;
                GameObject hero = null;
                switch (_heroName)
                {
                    case "Mario":
                        //hero = new Mario(mario.GetComponent<Mario>(), _lockIn, mario.GetComponent<Image>());
                        hero = mario;
                        break;
                    case "Luigi":
                        //hero = new Luigi(luigi.GetComponent<Luigi>(), _lockIn, luigi.GetComponent<Image>());
                        hero = luigi;
                        break;
                    case "Peach":
                        //hero = new Peach(peach.GetComponent<Peach>(), _lockIn, peach.GetComponent<Image>());
                        hero = peach;
                        break;
                    case "Yoshi":
                        //hero = new Yoshi(yoshi.GetComponent<Yoshi>(), _lockIn, yoshi.GetComponent<Image>());
                        hero = yoshi;
                        break;
                    case "Bowser":
                        //hero = new Bowser(bowser.GetComponent<Bowser>(), _lockIn, bowser.GetComponent<Image>());
                        hero = bowser;
                        break;
                }
                hero.SendMessage("SetTaken", _lockIn);
                hero.SendMessage("OnClickHero");
                //Debug.Log("TARGET UPDATE ISOWNER? " + _isOwner);
                //Debug.Log("TARGET UPDATE PLAYER: " + _player.GetComponent<PlayerObj>().GetPlayerName() + " of team " +_player.GetComponent<PlayerObj>().GetTeamIndex());

                if (_isOwner)
                {
                    //P_heroInfoImg.sprite = hero.P_heroIcon;
                    //P_heroInfoText.text = hero.P_heroName + "\n-----\n" + hero.P_heroDesc;

                    // activates status panel to local player to prevent them from clicking other heroes
                    P_statusPanel.SetActive(_lockIn);
                    // set lock in button to not interactable once locked in (can't un lock in)
                    P_lockInButton.GetComponent<Button>().interactable = !_lockIn;
                    // change text of button to indicate locked in status
                    P_lockInButton.GetComponentInChildren<Text>().text = "Locked In";
                }
            }
        }

        #endregion

        #region SetUI Functions

        [Client] // clients' view only because server's view doesn't matter
        private void SetUI_Parent(GameObject _player)
        {
            int teamIndex = _player.GetComponent<PlayerObj>().GetTeamIndex();

            // so that can only see own team, not the other (but will still exist in client <- intended)
            if(teamIndex == 1)
            {
                P_team1GameObject.SetActive(true);
                P_team2GameObject.SetActive(false);
            }
            else if(teamIndex == 2)
            {
                P_team1GameObject.SetActive(false);
                P_team2GameObject.SetActive(true);
            }
        }

        private void SetUI_Team(GameObject _player, int _teamIndex)
        {
            // assign player object to parent
            if(_teamIndex == 1)
                _player.transform.SetParent(P_team1GameObject.transform, false);
            else if(_teamIndex == 2)
                _player.transform.SetParent(P_team2GameObject.transform, false);
        }

        private void SetUI_Player(GameObject _player)
        {
            // to switch from player's lobby UI to player's character select UI
            _player.GetComponent<PlayerLobbyUI>().P_playerLobbyObject.SetActive(false);
            _player.GetComponent<PlayerLobbyUI>().P_playerCharacterSelectObject.SetActive(true);
        }

        private void SetUI_Hero(GameObject _player, string _heroName, bool _lockIn)
        {
            // will need to add another bool when hero lock-in is implemented
            if (!_lockIn)
            {
                Debug.Log("SETUI_HERO LOCK IN? NO...");
                if (_heroName == string.Empty)
                    _player.GetComponent<PlayerLobbyUI>().P_playerCharacter.text = "Picking a Hero...";
                else
                {
                    _player.GetComponent<PlayerLobbyUI>().P_playerCharacter.text = "Picking a Hero...(" + _heroName + ")";
                    _player.GetComponent<PlayerObj>().SetHeroName(_heroName);
                }
            }
            else if (_lockIn)
            {
                Debug.Log("SETUI_HERO LOCK IN? YES! " + _heroName);
                _player.GetComponent<PlayerObj>().SetHeroName(_heroName);
                _player.GetComponent<PlayerLobbyUI>().P_playerCharacter.text = _heroName;
            }
        }

        #endregion

        #region Debug

        public void ForceStart()
        {
            //StartCoroutine(RequestCharacterSelectUpdate(PlayerObj.instance.GetMatchID(), PlayerObj.instance.gameObject, true));
        }

        /*
        private IEnumerator UnloadLobbyScene()
        {
            if(isServer)
                SceneManager.UnloadSceneAsync(SDSceneManager.instance.P_lobbyScene);
            if(isClient)
            {
                yield return new WaitForSecondsRealtime(0.5f);
                SceneManager.UnloadSceneAsync(SDSceneManager.instance.P_lobbyScene);
            }
        }
        */

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
            MatchMaker.instance.GetMatches()[SDSceneManager.instance.P_matchID].SetCharacterSelectUIManager(instance);
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

#region archive

/*private IEnumerator RequestCharacterSelectUpdate(string _matchID, GameObject _player, bool _startGame)
{
    //CmdRequestCharacterSelectUpdate(_matchID, _player, _startGame);

    _player.GetComponent<PlayerObj>().GetComponent<PlayerLobbyUI>().P_playerLobbyObject.SetActive(false);
    _player.GetComponent<PlayerObj>().GetComponent<PlayerLobbyUI>().P_playerCharacterSelectObject.SetActive(true);

    if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 1)
    {
        P_team1GameObject.SetActive(true);
        P_team2GameObject.SetActive(false);
    }
    else if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 2)
    {
        P_team1GameObject.SetActive(false);
        P_team2GameObject.SetActive(true);
    }

    if(_startGame == true)
        _player.GetComponent<PlayerObj>().ci.GetComponent<SpawnManager>().SpawnPlayer("game");

    yield return null;
}*/

/*private IEnumerator SetUI_CharacterSelect(string _matchID)
{
    if(isServer)
    {
        while(!MatchMaker.instance.GetMatches()[_matchID].GetSDSceneManager().P_scenesLoaded)
            yield return null;

        foreach(var _player in MatchMaker.instance.GetMatches()[_matchID].GetPlayerObjList())
        {
            if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 1)
                _player.GetComponent<PlayerObj>().gameObject.transform.SetParent(MatchMaker.instance.GetMatches()[_matchID].GetCharacterSelectUIManager().P_team1GameObject.transform, false);
            else if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 2)
                _player.GetComponent<PlayerObj>().gameObject.transform.SetParent(MatchMaker.instance.GetMatches()[_matchID].GetCharacterSelectUIManager().P_team2GameObject.transform, false);
        }
    }

    if(isClient)
    {

    }

    if(isServer)
    {
        //while(!MatchMaker.instance.GetMatches()[_matchID].GetSDSceneManager().P_characterSelectSceneLoaded)
            yield return null;

        foreach(var _player in MatchMaker.instance.GetMatches()[_matchID].GetPlayerObjList())
        {
            if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 1)
            {
                MatchMaker.instance.GetMatches()[_matchID].GetCharacterSelectUIManager().P_team1GameObject.SetActive(true);
                MatchMaker.instance.GetMatches()[_matchID].GetCharacterSelectUIManager().P_team2GameObject.SetActive(false);

                _player.GetComponent<PlayerObj>().gameObject.transform.SetParent(MatchMaker.instance.GetMatches()[_matchID].GetCharacterSelectUIManager().P_team1GameObject.transform, false);
            }

            else if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 2)
            {
                MatchMaker.instance.GetMatches()[_matchID].GetCharacterSelectUIManager().P_team1GameObject.SetActive(false);
                MatchMaker.instance.GetMatches()[_matchID].GetCharacterSelectUIManager().P_team2GameObject.SetActive(true);

                _player.GetComponent<PlayerObj>().gameObject.transform.SetParent(MatchMaker.instance.GetMatches()[_matchID].GetCharacterSelectUIManager().P_team2GameObject.transform, false);
            }
        }
    }
}*/

#endregion
