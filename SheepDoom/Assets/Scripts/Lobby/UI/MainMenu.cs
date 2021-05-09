using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;


namespace SheepDoom
{
    public class MainMenu : MonoBehaviour
    {
        public static MainMenu instance;
        [SerializeField]
        private Text playerName;

        [Header("Setup match")]
        [SerializeField] InputField matchID;

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
        }

        public void SetPlayerName(string name)
        {
            playerName.text = name;
        }

        // When clicked host button
        private void Host()
        {
            GameObject _player = PlayerObj.instance.gameObject;
            if (_player.GetComponent<NetworkIdentity>().hasAuthority)
                _player.GetComponent<HostGame>().Host();
        }

        // When clicked Join button...may change to room listing
        private void Join()
        {
            string matchIdInput = matchID.text;
            GameObject _player = PlayerObj.instance.gameObject;
            if (_player.GetComponent<NetworkIdentity>().hasAuthority)
            {
                _player.GetComponent<JoinGame>().Join(matchIdInput.ToUpper());
                matchID.text = string.Empty;
            }
        }
    }
}