using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
namespace SheepDoom
{
    public class GameScore : NetworkBehaviour
    {
        //the display text for tower scores
        [Space(20)]
        [SerializeField]
        private Text blueCaptureCounter;

        [SerializeField]
        private Text redCaptureCounter;

        [SerializeField]
        public GameObject completeGameUI;
        public GameObject completeGameUI2;

        //counters for tower captures per team
        //hard coded for now
        [Space(20)]
        [SerializeField]
        //[SyncVar(hook = nameof(updateScoreDisplayClient))] private float blueCaptureScore;
        [SyncVar] private float blueCaptureScore;
        [SerializeField]
        //[SyncVar(hook = nameof(updateScoreDisplayClient))] private float redCaptureScore;
        [SyncVar] private float redCaptureScore;

        public Text blueDisplay;
        public Text redDisplay;
        public Text blueScoreText;
        public Text redScoreText;
        public GameObject[] players;
        public string[] playerNames = new string[6];
        public int[] playerTeams = new int[6];
        public Text BluePlayerNameText1;
        public Text BluePlayerNameText2;
        public Text BluePlayerNameText3;
        public Text RedPlayerNameText1;
        public Text RedPlayerNameText2;
        public Text RedPlayerNameText3;

        // Start is called before the first frame update
        /*void Start()
        {
            // changed to initialize on server only, if u put on start method, everytime new player joins, it will reinitialize to 1-1
            //initialize score
            //blueCaptureScore = 1;
            //redCaptureScore = 1;

            // they are already text components...
            //get the attached score counters text component
            //blueCaptureCounter = blueCaptureCounter.GetComponent<Text>();
            //redCaptureCounter = redCaptureCounter.GetComponent<Text>();

            //display score
            blueCaptureCounter.text = blueCaptureScore.ToString();
            redCaptureCounter.text = redCaptureScore.ToString();
        }*/

        //update score display on all clients
        public void updateScoreDisplay()
        {
            Debug.Log("blue capture score: " + blueCaptureScore);
            Debug.Log("red capture score: " + redCaptureScore);
            blueCaptureCounter.text = blueCaptureScore.ToString();
            redCaptureCounter.text = redCaptureScore.ToString();
        }

        /*
        public void updateScoreDisplayClient(float oldValue, float newValue)
        {
            blueCaptureCounter.text = blueCaptureScore.ToString();
            redCaptureCounter.text = redCaptureScore.ToString();
        }*/

        //scoring functions
        /*public void blueScoreUp()
        {
            Debug.Log("Blue score +1");
            //if blue scores, red will -1
            blueCaptureScore += 1;
            redCaptureScore -= 1;
            //update display
            updateScoreDisplay();
        }

        public void redScoreUp()
        {
            Debug.Log("Red score +1");
            //if blue scores, red will -1
            blueCaptureScore -= 1;
            redCaptureScore += 1;
            //update display
            updateScoreDisplay();
        }*/

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
            RpcUpdateClientScoreDisplay();
        }

        // deals with syncvar delay
        [ClientRpc]
        void RpcUpdateClientScoreDisplay()
        {
            StartCoroutine(WaitForUpdate(blueCaptureScore, redCaptureScore));
        }

        private IEnumerator WaitForUpdate(float _oldBlueScore, float _oldRedScore)
        {
            while (blueCaptureScore == _oldBlueScore && redCaptureScore == _oldRedScore)
                yield return null;
            updateScoreDisplay();
        }

        // game winning condition (will be called when base is taken)
        // shows scoreboard etc when game ends, will add timer counter condition in the future
        public void GameEnd(int TeamID)
        {
            //Scoreboard display Victory/Defeat for blue team
            GameObject blueTeam = FindMe.instance.P_BlueWinLose;
            blueDisplay = blueTeam.GetComponent<Text>();

            //Scoreboard display Victory/Defeat for red team
            GameObject redTeam = FindMe.instance.P_RedWinLose;
            redDisplay = redTeam.GetComponent<Text>();

            //Scoreboard Blue Team's Score
            GameObject blueScore = FindMe.instance.P_ScoreboardBlue;
            blueScoreText = blueScore.GetComponent<Text>();
            blueScoreText.text = blueCaptureScore.ToString();

            //Scoreboard Red Team's Score
            GameObject redScore = FindMe.instance.P_ScoreboardRed;
            redScoreText = redScore.GetComponent<Text>();
            redScoreText.text = redCaptureScore.ToString();

            //Get all players' name and team id
            if (players.Length == 0)
            {
                Debug.Log("Scoreboard: FindGameObjectsWithTag");
                players = GameObject.FindGameObjectsWithTag("Player");
            }

            foreach (GameObject player in players)
            {
                Debug.Log("Scoreboard: players.Length:" + players.Length);
                for (int i = 0; i < players.Length; i++)
                {
                    string name = player.GetComponent<PlayerObj>().GetPlayerName();
                    string[] playerNames = new string[] { name };
                    Debug.Log("Scoreboard: playerName: " + playerNames[i]);
                    int team = (int)player.GetComponent<PlayerAdmin>().getTeamIndex();
                    int[] playerTeams = new int[] { team };
                    Debug.Log("Scoreboard: playerTeam: " + playerTeams[i]);

                    if (playerTeams[i] == 1) //Blue Team
                    {
                        //Pull UI
                        GameObject BluePlayerName1 = FindMe.instance.P_BluePlayerName1;
                        BluePlayerNameText1 = BluePlayerName1.GetComponent<Text>();

                        GameObject BluePlayerName2 = FindMe.instance.P_BluePlayerName2;
                        BluePlayerNameText2 = BluePlayerName2.GetComponent<Text>();

                        GameObject BluePlayerName3 = FindMe.instance.P_BluePlayerName2;
                        BluePlayerNameText3 = BluePlayerName3.GetComponent<Text>();

                        Debug.Log("Scoreboard: End of FindMe for PlayerNames");
                        //Put name into UI 
                        if (BluePlayerNameText1.text != null)
                        {
                            BluePlayerNameText1.text = playerNames[i];
                        }
                        else if (BluePlayerNameText2.text != null)
                        {
                            BluePlayerNameText2.text = playerNames[i];
                        }
                        else if (BluePlayerNameText3.text != null)
                        {
                            BluePlayerNameText3.text = playerNames[i];
                        }
                    }
                    else if (playerTeams[i] == 2) //Red Team
                    {
                        //Pull UI
                        GameObject RedPlayerName1 = FindMe.instance.P_RedPlayerName1;
                        RedPlayerNameText1 = RedPlayerName1.GetComponent<Text>();

                        GameObject RedPlayerName2 = FindMe.instance.P_RedPlayerName2;
                        RedPlayerNameText2 = RedPlayerName2.GetComponent<Text>();

                        GameObject RedPlayerName3 = FindMe.instance.P_RedPlayerName3;
                        RedPlayerNameText3 = RedPlayerName3.GetComponent<Text>();

                        //Put name into UI 
                        if (RedPlayerNameText1.text != null)
                        {
                            RedPlayerNameText1.text = playerNames[i];
                        }
                        else if (RedPlayerNameText2.text != null)
                        {
                            RedPlayerNameText2.text = playerNames[i];
                        }
                        else if (RedPlayerNameText3.text != null)
                        {
                            RedPlayerNameText3.text = playerNames[i];
                        }
                    }
                }
            }


            //if blue team wins
            if (TeamID == 1)
            {
                Debug.Log("Blue Team Wins!");
                blueDisplay.text = "Victory";
                redDisplay.text = "Defeat";
                completeGameUI.SetActive(true);
                completeGameUI.GetComponent<Animator>().SetTrigger("Complete");
            }

            //if red team wins, not gonna use else for precision
            if (TeamID == 2)
            {
                Debug.Log("Red Team Wins!");
                blueDisplay.text = "Defeat";
                redDisplay.text = "Victory";
                completeGameUI.SetActive(true);
                completeGameUI.GetComponent<Animator>().SetTrigger("Complete");
            }
        }

        public override void OnStartServer()
        {
            blueCaptureScore = 1;
            redCaptureScore = 1;
            updateScoreDisplay();
        }

        public override void OnStartClient()
        {
            updateScoreDisplay(); // when start on client, it will automatically take values from server
        }
    }
}
