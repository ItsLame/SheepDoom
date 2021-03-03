using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSwitchManager : MonoBehaviour
{
    public CameraFollow camFollowscript;
    public CameraRoaming camRoamscript;

    bool camViewChanged = false;
    // Start is called before the first frame update
    void Start()
    {
        camRoamscript.enabled = false;
        
    }  
    // Update is called once per frame
    void Update()
    {
        if (camViewChanged == false)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                camViewChanged = true;

                camRoamscript.enabled = true;
                camFollowscript.enabled = false;
            }
        }
        else if (camViewChanged == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                camViewChanged = false;

                camRoamscript.enabled = false;
                camFollowscript.enabled = true;
            }
        }

    }
}
