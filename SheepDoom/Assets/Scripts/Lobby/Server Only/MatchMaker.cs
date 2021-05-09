using UnityEngine;
using Mirror;
using System.Collections;
using UnityEngine.SceneManagement;

// This script only runs on the server, not available to clients
namespace SheepDoom
{
    [System.Serializable]
    public class Match
    {
        private string matchID;
        private int team1Count;
        private int team2Count;
        private int countReady;
        private int countLockIn;
        private SyncListGameObject players = new SyncListGameObject();
        private SyncListGameObject heroes = new SyncListGameObject();
        private SyncList<Scene> matchScenes = new SyncList<Scene>(); // 0 = lobby, 1 = character select, 2 = game
        private SDSceneManager sdSceneManager;
        private LobbyUIManager lobbyUIManager;
        private CharacterSelectUIManager characterSelectUIManager;
        private GameStatus gameStatusManager;

        public Match(string matchID, GameObject player, SDSceneManager sdSceneManager)
        {
            this.matchID = matchID;
            players.Add(player);
            team1Count++;
            countReady++; // host by default already ready
            this.sdSceneManager = sdSceneManager;
        }

        #region Get
        
        public string GetMatchID()
        {
            return matchID;
        }

        public SyncListGameObject GetPlayerObjList()
        {
            return players;
        }

        public SyncListGameObject GetHeroesList()
        {
            return heroes;
        }

        public int GetTeam1Count()
        {
            return team1Count;
        }

        public int GetTeam2Count()
        {
            return team2Count;
        }

        public int GetReadyCount()
        {
            return countReady;
        }

        public int GetLockInCount()
        {
            return countLockIn;
        }

        public SDSceneManager GetSDSceneManager()
        {
            return sdSceneManager;
        }

        public LobbyUIManager GetLobbyUIManager()
        {
            return lobbyUIManager;
        }

        public CharacterSelectUIManager GetCharacterSelectUIManager()
        {
            return characterSelectUIManager;
        }

        public SyncList<Scene> GetScenes()
        {
            return matchScenes;
        }

        public GameStatus GetGameStatusManager()
        {
            return gameStatusManager;
        }

        #endregion

        #region Set

        public void AddTeam1Count()
        {
            team1Count++;
        }

        public void MinusTeam1Count()
        {
            team1Count--;
        }

        public void AddTeam2Count()
        {
            team2Count++;
        }

        public void MinusTeam2Count()
        {
            team2Count--;
        }

        public void AddCountReady()
        {
            countReady++;
        }

        public void MinusCountReady()
        {
            countReady--;
        }

        public void AddCountLockIn()
        {
            countLockIn++;
        }

        public void MinusCountLockIn()
        {
            countLockIn--;
        }

        public void SetLobbyUIManager(LobbyUIManager lobbyUIManager)
        {
            this.lobbyUIManager = lobbyUIManager;
        }

        public void SetCharacterSelectUIManager(CharacterSelectUIManager characterSelectUIManager)
        {
            this.characterSelectUIManager = characterSelectUIManager;
        }

        public void SetScene(Scene scene)
        {
            matchScenes.Add(scene);
        }

        public void SetGameStatusManager(GameStatus _gameStatusManageer)
        {
            gameStatusManager = _gameStatusManageer;
        }

        #endregion

        public Match() { }
    }

    [System.Serializable]
    // to store a list of game objects that needs to be synced between clients
    public class SyncListGameObject : SyncList<GameObject> { }

    public class MatchMaker : NetworkBehaviour
    {
        public static MatchMaker instance;

        // track matches
        private SyncDictionary<string, Match> matches = new SyncDictionary<string, Match>();
        [SerializeField] GameObject SDSceneManager;

        void Start()
        {
            instance = this;
        }

        public SyncDictionary<string, Match> GetMatches()
        {
            return matches;
        }

        //generate random match ID
        public static string GetRandomMatchID()
        {
            //create empty string 
            string _id = string.Empty;
            //generate time            
            for (int i = 0; i < 5; i++)
            {
                //generate random letter / number, 0~26 = letter and 27~36 = number
                int random = UnityEngine.Random.Range(0, 36);

                //letters
                if (random < 26)
                {
                    //randomly get char, capital + non caps
                    _id += (char)(random + 65);
                }

                //numbers
                else
                {
                    _id += (random - 26).ToString();
                }
            }

            Debug.Log($"Random Match ID: {_id}");
            return _id;
        }

        public bool HostGame(string _matchID, GameObject _player, NetworkConnection conn)
        {
            if (!matches.ContainsKey(_matchID))
            {
                GameObject sceneManager = Instantiate(SDSceneManager);
                NetworkServer.Spawn(sceneManager);
                Match newMatch = new Match(_matchID, _player, sceneManager.GetComponent<SDSceneManager>());
                matches.Add(_matchID, newMatch);
                newMatch.GetSDSceneManager().StartScenes(conn);
                newMatch.GetSDSceneManager().P_matchID = _matchID;
                _player.GetComponent<PlayerObj>().SetTeamIndex(1); // syncvared
                return true;
            }
            else
            {
                Debug.Log("Match ID already exists");
                return false;
            }
        }

        public bool JoinGame(string _matchID, GameObject _player)
        {
            if (matches.ContainsKey(_matchID) && matches[_matchID].GetPlayerObjList().Count < 6)
            {
                matches[_matchID].GetPlayerObjList().Add(_player);

                if (matches[_matchID].GetTeam1Count() < 3)
                {
                    matches[_matchID].AddTeam1Count();
                    _player.GetComponent<PlayerObj>().SetTeamIndex(1);
                }
                else if (matches[_matchID].GetTeam2Count() < 3)
                {
                    matches[_matchID].AddTeam2Count();
                    _player.GetComponent<PlayerObj>().SetTeamIndex(2);
                }

                return true;
            }
            else
            {
                Debug.Log("Match ID does not exist or match full");
                return false;
            }
        }

        public void StartNewScene(string _matchID, bool _charSelect, bool _game)
        {
            if (matches.ContainsKey(_matchID))
            {
                if (_charSelect)
                {
                    matches[_matchID].GetSDSceneManager().MoveToNewScene(matches[_matchID].GetSDSceneManager().gameObject, matches[_matchID].GetScenes()[1]);
                    foreach (GameObject player in matches[_matchID].GetPlayerObjList())
                        player.GetComponent<StartGame>().MoveToNewScene(matches[_matchID].GetScenes()[1], _matchID, true, false);
                    matches[_matchID].GetSDSceneManager().UnloadScenes(null, _matchID, true, false);
                    matches[_matchID].GetCharacterSelectUIManager().StartCharSelect(_matchID);
                }
                else if (_game)
                {
                    matches[_matchID].GetSDSceneManager().MoveToNewScene(matches[_matchID].GetSDSceneManager().gameObject, matches[_matchID].GetScenes()[2]);
                    foreach (GameObject player in matches[_matchID].GetPlayerObjList())
                    {
                        NetworkConnection conn = player.GetComponent<NetworkIdentity>().connectionToClient;
                        if (conn != null)
                            matches[_matchID].GetSDSceneManager().JoinGame(conn, _matchID);
                    }

                    foreach (GameObject player in matches[_matchID].GetPlayerObjList())
                        player.GetComponent<StartGame>().MoveToNewScene(matches[_matchID].GetScenes()[2], _matchID, false, true);
                    matches[_matchID].GetSDSceneManager().UnloadScenes(null, _matchID, false, true);

                    foreach (GameObject player in matches[_matchID].GetPlayerObjList())
                    {
                        Client ci = player.GetComponent<PlayerObj>().ci;
                        ci.GetComponent<SpawnManager>().SpawnForGame("game", player);
                        matches[_matchID].GetHeroesList().Add(ci.GetComponent<SpawnManager>().GetPlayerObj());
                    }
                }
            }
        }

        public void ClearMatch(string _matchID, bool _isLobby, bool _isCharSelect, bool _isGame)
        {
            if(matches.ContainsKey(_matchID))
            {
                Match closedMatch = matches[_matchID];
                if(closedMatch.GetSDSceneManager().gameObject.activeInHierarchy)
                    closedMatch.GetSDSceneManager().MoveToNewScene(closedMatch.GetSDSceneManager().gameObject, SceneManager.GetSceneAt(0));

                if (_isLobby)
                {
                    closedMatch.GetSDSceneManager().UnloadScenes(null, _matchID, true, false);
                    closedMatch.GetSDSceneManager().UnloadScenes(null, _matchID, false, true);
                    StartCoroutine(WaitForSceneUnload(closedMatch, true, false, false));
                }
                else if (_isCharSelect)
                {
                    closedMatch.GetSDSceneManager().UnloadScenes(null, _matchID, false, true);
                    StartCoroutine(WaitForSceneUnload(closedMatch, false, true, false));
                }
                else if(_isGame)
                {
                    closedMatch.GetSDSceneManager().UnloadScenes(null, _matchID, false, false);
                    StartCoroutine(WaitForSceneUnload(closedMatch, false, false, true));
                }
            }
        }

        private IEnumerator WaitForSceneUnload(Match _closedMatch, bool _isLobby, bool _isCharSelect, bool _isGame)
        {
            if (_isLobby)
            {
                while (_closedMatch.GetScenes()[0].isLoaded || _closedMatch.GetScenes()[1].isLoaded)
                    yield return null;
            }
            else if (_isCharSelect)
            {
                while (_closedMatch.GetScenes()[1].isLoaded)
                    yield return null;
            }
            else if (_isGame)
            {
                while (_closedMatch.GetSDSceneManager().P_gameSceneLoaded)
                    yield return null;
            }

            _closedMatch.GetScenes().Clear();
            NetworkServer.Destroy(_closedMatch.GetSDSceneManager().gameObject);
            matches.Remove(_closedMatch.GetMatchID());
        }

        #region Start & Stop Callbacks

        /// <summary>
        /// This is invoked for NetworkBehaviour objects when they become active on the server.
        /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
        /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
        /// </summary>
        public override void OnStartServer() 
        {
            
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
    /*
    //converting id to guid using some service
    //convert 5 digit string into a guid
    //remember hashing in system security? huehehue
    public static class MatchExtensions
    {
        public static Guid ToGuid(this string id)
        {
            //create instance
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();

            //copied word for word as its the same enconding thing
            byte[] inputBytes = Encoding.Default.GetBytes(id);
            byte[] hashBytes = provider.ComputeHash(inputBytes);

            return new Guid(hashBytes);
        }
    }*/
}