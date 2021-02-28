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

        //constructor that takes in matchID and the host
        public Match(string matchID, GameObject player)
        {
            this.matchID = matchID;

            //add player directly to the list
            players.Add(player);
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
        public SyncListString matchIDs = new SyncListString();
        void Start()
        {
            instance = this;
        }

        //host game bool validation for same IDs..
        public bool HostGame(string _matchID, GameObject _player)
        {
            //if duplicate is not found in matchIDs synclist, create new match
            if (!matchIDs.Contains(_matchID))
            {
                //add a match to synclistmatch matches using the constructor that uses id + player
                matches.Add(new Match(_matchID, _player));
                Debug.Log($"Match ID Created");
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