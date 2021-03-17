using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{
    public class LobbyUI : MonoBehaviour
    {
        //public static LobbyUI instance;
        [SerializeField]
        private Text MatchIDText;

        private void Start()
        {
            StartCoroutine(SetUI_MatchID());
        }

        IEnumerator SetUI_MatchID()
        {
            string _matchID = string.Empty;

            if(LobbyManager.instance)
            {
                LobbyManager.instance.GetMatchID(out _matchID);

                while(_matchID == null)
                {
                    Debug.Log("matchID still null!");
                    yield return null;
                }

                Debug.Log("matchID not null anymore!");

                //set matchID to UI (server)
                MatchIDText.text = _matchID;
            }
            else if(!LobbyManager.instance)
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
    }
}
