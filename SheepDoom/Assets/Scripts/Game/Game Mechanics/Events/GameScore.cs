using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
namespace SheepDoom
{
    public class GameScore : NetworkBehaviour
    { 
        [SerializeField] private string TopPlayer;
        [SerializeField] private double TempHighestScore;
        [SerializeField] private float TempTopPlayerNoOfTower;
        [SerializeField] private GameObject gameStatus;
        //the display text for tower scores
        [Space(20)]
        [SerializeField]
        private Text blueCaptureCounter;

        [SerializeField]
        private Text redCaptureCounter;

        [Header("--- Kill, Death, Capture ---")]
        [Space(5)]
        [SerializeField] private GameObject completeGameUI;
        [SerializeField] private GameObject ScoreboardBlue;
        [SerializeField] private GameObject ScoreboardRed;
        [SerializeField] private GameObject BlueWinLose;
        [SerializeField] private GameObject RedWinLose;
        [SerializeField] private GameObject BP1Star;
        [SerializeField] private GameObject BluePlayer1Image;
        [SerializeField] private GameObject BluePlayer1Name;
        [SerializeField] private GameObject BP2Star;
        [SerializeField] private GameObject BluePlayer2Image;
        [SerializeField] private GameObject BluePlayer2Name;
        [SerializeField] private GameObject BP3Star;
        [SerializeField] private GameObject BluePlayer3Image;
        [SerializeField] private GameObject BluePlayer3Name;
        [SerializeField] private GameObject RP1Star;
        [SerializeField] private GameObject RedPlayer1Image;
        [SerializeField] private GameObject RedPlayer1Name;
        [SerializeField] private GameObject RP2Star;
        [SerializeField] private GameObject RedPlayer2Image;
        [SerializeField] private GameObject RedPlayer2Name;
        [SerializeField] private GameObject RP3Star;
        [SerializeField] private GameObject RedPlayer3Image;
        [SerializeField] private GameObject RedPlayer3Name;

        private Sprite CharacterImage, Character1, Character2, Character3;

        //counters for tower captures per team
        [Space(20)]
        [SerializeField]
        [SyncVar] private float blueCaptureScore;
        [SerializeField]
        [SyncVar] private float redCaptureScore;

        void Start()
        {
            Character1 = Resources.Load<Sprite>("AlmaCS");
            Character2 = Resources.Load<Sprite>("AstarothCS");
            Character3 = Resources.Load<Sprite>("IsabellaCS");
        }

        //update score display on all clients
        public void updateScoreDisplay()
        {
            blueCaptureCounter.text = blueCaptureScore.ToString();
            redCaptureCounter.text = redCaptureScore.ToString();
        }

        // causes a syncvar delay cuz this is only run on server
        public void ScoreUp(bool _byBlue, bool _byRed)
        {
            if (_byBlue && !_byRed)
            {
                blueCaptureScore += 1;
                redCaptureScore -= 1;
            }
            else if (_byRed && !_byBlue)
            {
                redCaptureScore += 1;
                blueCaptureScore -= 1;
            }
            updateScoreDisplay();
            RpcUpdateClientScoreDisplay(true, false, false, null, 0);
        }

        // deals with syncvar delay
        [ClientRpc]
        void RpcUpdateClientScoreDisplay(bool _isOngoing, bool _isEnd, bool _displayScoreBoard, GameObject _player, int _teamID)
        {
            if (_isOngoing)
                StartCoroutine(WaitForUpdate(blueCaptureScore, redCaptureScore));
            else if (_isEnd)
            {
                CalculateEndGame(_player, _teamID);
                
                // disable map objects
                FindMe.instance.P_GameStatusManager.GetComponent<GetGameStatus>().Disable_MapObjects();
            }
            else if (_displayScoreBoard)
                DisplayScoreBoard(_teamID);
        }

        private IEnumerator WaitForUpdate(float _oldBlueScore, float _oldRedScore)
        {
            while (blueCaptureScore == _oldBlueScore && redCaptureScore == _oldRedScore)
                yield return null;
            updateScoreDisplay();
        }

        // game winning condition (will be called when base is taken)
        // shows scoreboard etc when game ends, will add timer counter condition in the future
        [Server]
        public void GameEnd(int TeamID)
        {
            if(gameStatus != null)
            {
                foreach (GameObject _player in MatchMaker.instance.GetMatches()[gameStatus.GetComponent<GameStatus>().P_matchID].GetHeroesList())
                {
                    CalculateEndGame(_player, TeamID);
                    RpcUpdateClientScoreDisplay(false, true, false, _player, TeamID);
                }
                DisplayScoreBoard(TeamID);
                RpcUpdateClientScoreDisplay(false, false, true, null, TeamID);
            }
        }

        private void DisplayScoreBoard(int TeamID)
        {
            ScoreboardBlue.GetComponent<Text>().text = blueCaptureScore.ToString();
            ScoreboardRed.GetComponent<Text>().text = redCaptureScore.ToString();

            if (!string.IsNullOrEmpty(TopPlayer))
            {
                if (!string.IsNullOrEmpty(BluePlayer1Name.GetComponent<Text>().text) && BluePlayer1Name.GetComponent<Text>().text.Substring(0, TopPlayer.Length) == TopPlayer)
                    BP1Star.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                else if (!string.IsNullOrEmpty(BluePlayer2Name.GetComponent<Text>().text) && BluePlayer2Name.GetComponent<Text>().text.Substring(0, TopPlayer.Length) == TopPlayer)
                    BP2Star.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                else if (!string.IsNullOrEmpty(BluePlayer3Name.GetComponent<Text>().text) && BluePlayer3Name.GetComponent<Text>().text.Substring(0, TopPlayer.Length) == TopPlayer)
                    BP3Star.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                else if (!string.IsNullOrEmpty(RedPlayer1Name.GetComponent<Text>().text) && RedPlayer1Name.GetComponent<Text>().text.Substring(0, TopPlayer.Length) == TopPlayer)
                    RP1Star.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                else if (!string.IsNullOrEmpty(RedPlayer2Name.GetComponent<Text>().text) && RedPlayer2Name.GetComponent<Text>().text.Substring(0, TopPlayer.Length) == TopPlayer)
                    RP2Star.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                else if (!string.IsNullOrEmpty(RedPlayer3Name.GetComponent<Text>().text) && RedPlayer3Name.GetComponent<Text>().text.Substring(0, TopPlayer.Length) == TopPlayer)
                    RP3Star.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
                //if blue team wins
            if (TeamID == 1)
            {
                BlueWinLose.GetComponent<Text>().text = "Victory";
                RedWinLose.GetComponent<Text>().text = "Defeat";
                completeGameUI.SetActive(true);
                completeGameUI.GetComponent<Animator>().SetTrigger("Complete");
            }
            else if (TeamID == 2) //if red team wins, not gonna use else for precision
            {
                BlueWinLose.GetComponent<Text>().text = "Defeat";
                RedWinLose.GetComponent<Text>().text = "Victory";
                completeGameUI.SetActive(true);
                completeGameUI.GetComponent<Animator>().SetTrigger("Complete");
            }
        }

        private void CalculateEndGame(GameObject player, int TeamID)
        {
            //======================= GET CURRENT PLAYER INFORMATION ========================
            string name = player.GetComponent<PlayerAdmin>().P_playerName;
            float charId = player.GetComponent<PlayerAdmin>().getCharID();
            float team = player.GetComponent<PlayerAdmin>().getTeamIndex();
            float kills = player.GetComponent<PlayerAdmin>().P_playerKills;
            float deaths = player.GetComponent<PlayerAdmin>().P_playerDeaths;
            float towerCap = player.GetComponent<PlayerAdmin>().P_towerCaptures;

            //======================= CALCULATE PLAYER SCORE (STAR PLAYER) ========================
            double currentPlayerScore = (kills - deaths) + (towerCap * 1.5);
            if (TempHighestScore < currentPlayerScore)
            {
                TempHighestScore = currentPlayerScore;
                TempTopPlayerNoOfTower = towerCap;
                TopPlayer = name;
            }
            else if (TempHighestScore == currentPlayerScore) //if tie score
            {
                //check who capture more towers
                if (TempTopPlayerNoOfTower < towerCap)
                {
                    TempHighestScore = currentPlayerScore;
                    TempTopPlayerNoOfTower = towerCap;
                    TopPlayer = name;
                }
            }

            //======================= GET CURRENT PLAYER'S CHARACTER IMAGE ========================
            if (charId == 1)
                CharacterImage = Character1;
            else if (charId == 2)
                CharacterImage = Character2;
            else if (charId == 3)
                CharacterImage = Character3;

            string playerDetails = name + "\t" + kills.ToString() + "/" + deaths.ToString() + "/" + towerCap.ToString();
            if (team == 1)
            {
                //Put name & image into UI 
                if (string.IsNullOrEmpty(BluePlayer1Name.GetComponent<Text>().text))
                {
                    BluePlayer1Name.GetComponent<Text>().text = playerDetails;
                    BluePlayer1Image.GetComponent<Image>().sprite = CharacterImage;
                    BluePlayer1Image.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                }
                else if (string.IsNullOrEmpty(BluePlayer2Name.GetComponent<Text>().text))
                {
                    BluePlayer2Name.GetComponent<Text>().text = playerDetails;
                    BluePlayer2Image.GetComponent<Image>().sprite = CharacterImage;
                    BluePlayer2Image.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                }
                else if (string.IsNullOrEmpty(BluePlayer3Name.GetComponent<Text>().text))
                {
                    BluePlayer3Name.GetComponent<Text>().text = playerDetails;
                    BluePlayer3Image.GetComponent<Image>().sprite = CharacterImage;
                    BluePlayer3Image.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                }
            }
            else if (team == 2)
            {
                if (string.IsNullOrEmpty(RedPlayer1Name.GetComponent<Text>().text))
                {
                    RedPlayer1Name.GetComponent<Text>().text = playerDetails;
                    RedPlayer1Image.GetComponent<Image>().sprite = CharacterImage;
                    RedPlayer1Image.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                }
                else if (string.IsNullOrEmpty(RedPlayer2Name.GetComponent<Text>().text))
                {
                    RedPlayer2Name.GetComponent<Text>().text = playerDetails;
                    RedPlayer2Image.GetComponent<Image>().sprite = CharacterImage;
                    RedPlayer2Image.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                }
                else if (string.IsNullOrEmpty(RedPlayer3Name.GetComponent<Text>().text))
                {
                    RedPlayer3Name.GetComponent<Text>().text = playerDetails;
                    RedPlayer3Image.GetComponent<Image>().sprite = CharacterImage;
                    RedPlayer3Image.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                }
            }
        }

        public override void OnStartServer()
        {
            blueCaptureScore = 2;
            redCaptureScore = 2;
            updateScoreDisplay();
        }

        public override void OnStartClient()
        {
            updateScoreDisplay(); // when start on client, it will automatically take values from server
        }

        //accessor method for scores
        public float getBlueScore()
        {
            return blueCaptureScore;
        }

        public float getRedScore()
        {
            return redCaptureScore;
        }
    }
}
