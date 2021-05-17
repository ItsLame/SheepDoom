using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSwitchManager : MonoBehaviour
{
    public CameraFollow camFollowscript;
    public CameraRoaming camRoamscript;

    public bool camViewChanged = false;


    //to check whether sides are touched
    public bool topScreenClicked = false;
    public bool bottomScreenClicked = false;
    public bool leftScreenClicked = false;
    public bool rightScreenClicked = false;

    public bool snapBackToPlayer = true;

    public bool isVictory = false;

    public void touchTop()
    {
        topScreenClicked = true;
    }

    public void touchBot()
    {
        bottomScreenClicked = true;
    }

    public void touchLeft()
    {
        leftScreenClicked = true;
    }

    public void touchRight()
    {
        rightScreenClicked = true;
    }

    public void untouchTop()
    {
        topScreenClicked = false;
    }

    public void untouchBot()
    {
        bottomScreenClicked = false;
    }

    public void untouchLeft()
    {
        leftScreenClicked = false;
    }

    public void untouchRight()
    {
        rightScreenClicked = false;
    }


    // Start is called before the first frame update
    void Start()
    {
        camRoamscript.enabled = true;
    }

    //snap camera back to player
    public void snapBackFunction()
    {
        topScreenClicked = false;
        bottomScreenClicked = false;
        leftScreenClicked = false;
        rightScreenClicked = false;
        snapBackToPlayer = true;
    }

    // Update is called once per frame
    void Update()
    {
         // to ignore update when victory
        if(isVictory)
            return;

        if (topScreenClicked || bottomScreenClicked || leftScreenClicked || rightScreenClicked)
            camFollowscript.enabled = false;
        else
            camFollowscript.enabled = true;


        if (camViewChanged == false)
        {
            if (topScreenClicked || bottomScreenClicked || leftScreenClicked || rightScreenClicked)
            {
                snapBackToPlayer = false;
                camViewChanged = true;
                camFollowscript.enabled = false;
            }
        }
        else if (camViewChanged == true)
        {
            if (snapBackToPlayer)
            {
                camViewChanged = false;
                camFollowscript.enabled = true;
            }
        }
    }
}
