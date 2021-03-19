using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*namespace SheepDoom
{
    //public delegate void UIEvent();
    
    public class LobbyUI : MonoBehaviour
    {   
        //public event UIEvent uiEvent;

        [Header("Lobby UI Setup")]
        public static LobbyUI instance;
        [SerializeField] private Text MatchIDText;
        [SerializeField] private GameObject Team1GameObject;
        [SerializeField] private GameObject Team2GameObject;
        private string _matchID = string.Empty;
        private int team1Count = 0;
        private int team2Count = 0;

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

        public void Start()
        //private void Update()
        {
            StartSetting();
            //uiEvent += UpdateUI_Team;
        }

        private void StartSetting()
        {
            //Debug.Log("startsetting");
            StartCoroutine(SetUI_MatchID());
        }

        public void StartAgain()
        {
            StartCoroutine(SetUI_Team(_matchID, PlayerObj.instance));
        }

        public void TeamSetting()
        {
            while(myTeam1Count != MatchMaker.instance.GetTeam1Count() ||
                myTeam2Count != MatchMaker.instance.GetTeam2Count())
            {
                Debug.Log("no changes to teamcount! updating...");

                myTeam1Count = MatchMaker.instance.GetTeam1Count();
                myTeam2Count = MatchMaker.instance.GetTeam2Count();
            }

            //set again to notify uievent
            myTeam1Count = MatchMaker.instance.GetTeam1Count();
            myTeam2Count = MatchMaker.instance.GetTeam2Count();
        }

        IEnumerator SetUI_MatchID()
        {
            //set matchid to UI if not set yet
            //server view
            if(LobbyManager.instance)
            {
                if(_matchID == string.Empty)
                {
                    LobbyManager.instance.GetMatchID();

                    while(_matchID == null)
                    {
                        Debug.Log("matchID still null!");
                        yield return null;
                    }

                    Debug.Log("matchID not null anymore!");

                    //set matchID to UI (server)
                    MatchIDText.text = _matchID;
                }
            }
            //client view
            else if(!LobbyManager.instance)
            {
                if(_matchID == string.Empty)
                {
                    while(PlayerObj.instance.GetMatchID() == null)
                    {
                        Debug.Log("matchID still null!");
                        yield return null;
                    }

                    Debug.Log("matchID not null anymore!");
                    _matchID = PlayerObj.instance.GetMatchID();

                    //set matchID to UI (client)
                    MatchIDText.text = _matchID;
                }
            }

            //if already set or is not null, start coroutine set team UI
            StartCoroutine(SetUI_Team(_matchID, PlayerObj.instance));
        }

        IEnumerator SetUI_Team(string _matchID, PlayerObj _playerObj)
        {
            //set player object transform parent
            //server view
            if(LobbyManager.instance)
            {
                while(myTeam1Count != MatchMaker.instance.GetTeam1Count() ||
                    myTeam2Count != MatchMaker.instance.GetTeam2Count())
                {
                    Debug.Log("no changes to teamcount! updating...");

                    myTeam1Count = MatchMaker.instance.GetTeam1Count();
                    myTeam2Count = MatchMaker.instance.GetTeam2Count();

                    yield return null;
                }

                //set again to notify uievent
                myTeam1Count = MatchMaker.instance.GetTeam1Count();
                myTeam2Count = MatchMaker.instance.GetTeam2Count();
            }
            //client view
            else if(!LobbyManager.instance)
            {
                if(_playerObj.GetTeamIndex() == 1)
                {
                    Debug.Log("updating team1 UI");
                    _playerObj.transform.parent = Team1GameObject.transform;
                }
                if(_playerObj.GetTeamIndex() == 2)
                {
                    Debug.Log("updating team2 UI");
                    _playerObj.transform.parent = Team1GameObject.transform; 
                }

                PlayerObj.instance.transform.parent = Team1GameObject.transform;
                while(myTeam1Count != MatchMaker.instance.GetTeam1Count() ||
                    myTeam2Count != MatchMaker.instance.GetTeam2Count())
                {
                    Debug.Log("no changes to teamcount! updating...");

                    myTeam1Count = MatchMaker.instance.GetTeam1Count();
                    myTeam2Count = MatchMaker.instance.GetTeam2Count();

                    yield return null;
                }

                Debug.Log("myteam1count: " + myTeam1Count);
            }
        }

        private void UpdateUI_Team()
        {
            //set again to notify uievent
            //myTeam1Count = MatchMaker.instance.GetTeam1Count();
            //myTeam2Count = MatchMaker.instance.GetTeam2Count();

            Debug.Log("myteam1count: " + myTeam1Count);

            Debug.Log("playerobj instance getteamindex:" + PlayerObj.instance.GetTeamIndex());

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
    }
}
*/