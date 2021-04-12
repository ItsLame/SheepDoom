using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

//used for the buttons in login to force select your team
// BLUE IS 1, RED IS 2
public class DebugTeamSelector : NetworkBehaviour
{
    public GameObject notifyingText;

    //teamID
    // BLUE IS 1, RED IS 2
    [SerializeField]
    private float teamID;
    private Text displayTeam;

    //accessor method
    public float getTeamID()
    {
        Debug.Log("Getting Team ID: " + teamID);
        return teamID;
    }

//    [Command]
    //select blue team (TEAM 1)
    public void selectBlue()
    {
  //      if (!hasAuthority) return;
        displayTeam.text = "Blue Team Selected";
        joinBlue();
    }

 //   [Command]
    //select blue team (TEAM 2)
    public void selectRed()
    {
  //      if (!hasAuthority) return;
        displayTeam.text = "Red Team Selected";
        joinRed();
    }

//    [TargetRpc]
    public void joinBlue()
    {
        teamID = 1;
        Debug.Log("Team ID = " + teamID);
    }

//   [TargetRpc]
    public void joinRed()
    {
        teamID = 2;
        Debug.Log("Team ID = " + teamID);
    }

    //default is blue
    private void Start()
    {
        displayTeam = notifyingText.GetComponent<Text>();
        teamID = 1;
    }
}
