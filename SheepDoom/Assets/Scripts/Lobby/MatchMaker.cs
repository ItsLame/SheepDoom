using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Security.Cryptography;
using System.Text;

namespace MirrorBasics { 

    //to store the Match ID, number of players, players, etc
    [System.Serializable]
    public class Match
    {
        //id of match
        public string matchID;
        //storing the players
        public SyncListGameObject players = new SyncListGameObject();
        public int team1;
        public int team2;
        //int to count players ready
        public int countReady;
        public bool imReady;
        //public bool inMatch = false;

        //constructor that takes in matchID and the host
        public Match(string matchID, GameObject player)
        {
            this.matchID = matchID;

            //add player directly to the list
            players.Add(player);
            team1++;
            team2 = 0;
            countReady = 0;
        }

        //default constructor for unity to be happy. blank 
        public Match() { }
    }

    [System.Serializable]
    //to store a list of game objects that needs to be synced between clients
    public class SyncListGameObject : SyncList<GameObject> 
    { 
    }

    [System.Serializable]
    //to store the list of matches of class Match
    public class SyncListMatch : SyncList<Match>
    {
    }

    public class MatchMaker : NetworkBehaviour
    {
        //single entity of matchmaker at a time
        public static MatchMaker instance;

        //create a synclistmatch storing the matches on startup
        public SyncListMatch matches = new SyncListMatch();

        //for easier checking of duplicate matchID
        public SyncList<string> matchIDs = new SyncList<string>();
        void Start()
        {
            instance = this;
        }

        //host game bool validation for same IDs..
        public bool HostGame(string _matchID, GameObject _player, out int playerIndex, out int teamIndex)
        {
            playerIndex = -1;
            teamIndex = -1;
            //if duplicate is not found in matchIDs synclist, create new match
            if (!matchIDs.Contains(_matchID))
            {
                //add a match to synclistmatch matches using the constructor that uses id + player
                matchIDs.Add(_matchID);
                playerIndex = 1;
                matches.Add(new Match(_matchID, _player));
                teamIndex = 1;
                Debug.Log($"Match ID Created");
                
                for (int i = 0; i < matches.Count; i++)
                {
                    if(matches[i].matchID == _matchID)
                    {
                        matches[i].countReady += 1;
                        Debug.Log("Room [" + _matchID + "] Ready Count: " + matches[i].countReady);
                        break;
                    }
                }
                
                return true;
            }
            else
            {
                //else its duplicate, a nono
                Debug.Log($"Match ID already exists");
                return false;
            }

            //validation for existing id
        }

        public bool JoinGame(string _matchID, GameObject _player, out int playerIndex, out int teamIndex)
        {
            playerIndex = -1;
            teamIndex = -1;
            //joining a room
            if (matchIDs.Contains(_matchID))
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    if(matches[i].matchID == _matchID)
                    {
                        matches[i].players.Add(_player);
                        if(matches[i].team1 < 3)
                        {
                            matches[i].team1++;
                            playerIndex = matches[i].team1;
                            teamIndex = 1;
                        }
                        else if(matches[i].team2 < 3)
                        {
                            matches[i].team2++;
                            playerIndex = matches[i].team2;
                            teamIndex = 2;
                        }
                        break;
                    }
                }
                Debug.Log("Match Joined");
                return true;
            }
            else
            {
                //else its duplicate, a nono
                Debug.Log($"Match ID does not exists");
                return false;
            }
        }

        public void EnterRoomInit(string _matchID, GameObject _player)
        {
            _player.GetComponent<Lobby_Player>().isReady = true;

            for(int i = 0; i < matches.Count; i++)
            {
                //search match
                if(matches[i].matchID == _matchID)
                {
                    //loop through players in match
                    for(int k = 1; k != matches[i].players.Count; k++)
                    {
                        //update ready status of pre-existing clients to the client who just entered
                        if(matches[i].players[k].GetComponent<Lobby_Player>().isReady)
                        {
                            Debug.Log("MM EnterRoomInit: Updating Player " + matches[i].players[k].GetComponent<Lobby_Player>().playerIndex + " to wait");
                        }
                        else if(!matches[i].players[k].GetComponent<Lobby_Player>().isReady)
                        {
                            Debug.Log("MM EnterRoomInit: Updating Player " + matches[i].players[k].GetComponent<Lobby_Player>().playerIndex + " to ready");
                        }

                        matches[i].players[k].GetComponent<Lobby_Player>().RpcReadyGame();
                    }
                }
            }
        }

        public void ReadyGame(string _matchID, GameObject _player)
        {
            for(int i = 0; i < matches.Count; i++)
            {
                //search match
                if(matches[i].matchID == _matchID)
                {
                    //switch player's ready status
                    if(_player.GetComponent<Lobby_Player>().isReady == true)
                    {
                        matches[i].countReady += 1;

                        Debug.Log("Player " + _player.GetComponent<Lobby_Player>().playerIndex + " change ready status to: " + _player.GetComponent<Lobby_Player>().isReady);
                        Debug.Log("Player " + _player.GetComponent<Lobby_Player>().playerIndex + " changed to ready!");
                        Debug.Log("Room [" + _matchID + "] Ready Count: " + matches[i].countReady);
                        
                        break;
                    }
                    else if(_player.GetComponent<Lobby_Player>().isReady == false)
                    {
                         matches[i].countReady -= 1;

                        Debug.Log("Player " + _player.GetComponent<Lobby_Player>().playerIndex + " change ready status to: " + _player.GetComponent<Lobby_Player>().isReady);
                        Debug.Log("Player " + _player.GetComponent<Lobby_Player>().playerIndex + " changed to waiting!");
                        Debug.Log("Room [" + _matchID + "] Ready Count: " + matches[i].countReady);
                        
                        break;
                    }
                }
            }

            _player.GetComponent<Lobby_Player>().UpdateReadyCountUI();
        }

        // does not account for player disconnect, when player gone, won't deduct or increment slots
        public bool SwitchTeam(string _matchID, out int teamIndex, string team)
        {
            Debug.Log("In Matchmaker switchteam");
            bool isSwitch = false;
            teamIndex = -1;
            for(int i = 0; i < matches.Count; i++)
            {
                if(matches[i].matchID == _matchID)
                {
                    if(team == "team1")
                    {
                        if(matches[i].team1 < 3)
                        {
                            matches[i].team2--;
                            matches[i].team1++;
                            teamIndex = 1;
                            isSwitch = true;
                        }
                        else
                        {
                            Debug.Log("Team 1 is already full");
                            isSwitch = false;
                        }
                    }
                    else if (team == "team2")
                    {
                        if(matches[i].team2 < 3)
                        {
                            matches[i].team1--;
                            matches[i].team2++;
                            teamIndex = 2;
                            isSwitch = true;
                        }
                        else
                        {
                            Debug.Log("Team 2 is already full");
                            isSwitch = false;
                        }
                    }
                    break;
                }
            }
            return isSwitch;
        }

        //start game for everyone
        public void StartGame(string startButton, string _matchID)
        {
            if(startButton == "Force Start")
            {
                for(int i = 0; i < matches.Count; i++)
                {
                    if (matches[i].matchID == _matchID) // find the correct match
                    {
                        //matches[i].inMatch = true;
                        foreach (var player in matches[i].players)
                        {
                            Lobby_Player _player = player.GetComponent<Lobby_Player>();
                            Debug.Log("MatchMaker: [Force]StartGame foreach loop");
                            _player.BeginGame(true); // start the correspondin g match
                        }
                        break;
                    }
                }
            }
            else if(startButton == "Normal Start")
            {
                for(int i = 0; i < matches.Count; i++)
                {
                    if (matches[i].matchID == _matchID) // find the correct match
                    {
                        foreach (var player in matches[i].players)
                        {
                            Lobby_Player _player = player.GetComponent<Lobby_Player>();
                            Debug.Log("MatchMaker: [Normal]StartGame foreach loop");

                            if(matches[i].countReady == matches[i].players.Count)
                            {
                                _player.BeginGame(true); // start the corresponding match
                            }
                            else if(matches[i].countReady < matches[i].players.Count)
                            {
                                _player.BeginGame(false);
                            }
                        }
                        break;
                    }
                }
            }
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
    }

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
    }
}

// ------------- ARCHIVE ------------- //

/*Debug.Log("MatchMaker: Game Started!");
//game is starting <-- previously in StartGame() function
Lobby_Player.localPlayer.isGameStart = true; // not working in server build
Debug.Log("isGameStart set to true");*/

//switch team viewable for everyone <-- function transferred to Lobby_Player.cs RpcSwitchTeam,
//                                      previously in StartGame() function
        /*public void SwitchTeam(Transform _teamParentGroup, out int _teamIndex)
        {
            _teamIndex = Lobby_Player.localPlayer.teamIndex;
            bool isSwitch = true;
            int _playerIndex = Lobby_Player.localPlayer.playerIndex;
            
            if(isSwitch == true)
            {
                if(_teamIndex == 1)
                {
                    Debug.Log("player " + _playerIndex + ": switches to team 2!");
                    _teamIndex = 2;
                    UI_LobbyScript.instance.gameObject.transform.SetParent(_teamParentGroup);
                    UI_LobbyScript.instance.SwitchToTeam2();
                }
                else if(_teamIndex == 2)
                {
                    Debug.Log("player " + _playerIndex + ": switches to team 1!");
                    _teamIndex = 1;
                    UI_LobbyScript.instance.gameObject.transform.SetParent(_teamParentGroup);
                    UI_LobbyScript.instance.SwitchToTeam1();
                }

                isSwitch = false;
            }
        }*/