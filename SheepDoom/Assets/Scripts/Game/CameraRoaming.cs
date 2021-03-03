using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraRoaming : MonoBehaviour
{
    public float camspeed = 30;
    public float screenSizeThickness = 200.0f;
    public bool screenTouched = false;
    public Text m_Text;
    // Update is called once per frame
    void Update()
    {
        //to check position
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            m_Text.text = "Touch Position : " + touch.position;
            Debug.Log("Screen has been touched");
            Debug.Log("Position X:" + Input.GetTouch(0).position.x);
            Debug.Log("Position Y:" + Input.GetTouch(0).position.y);

        }

        else
        {
            m_Text.text = "No touch contacts";
        }

        Debug.Log("Screentouched: " + screenTouched);

        if (screenTouched)
        {
            if (Input.touchCount == 0)
            {
                Debug.Log("Screen edges have been let go");
                this.gameObject.GetComponent<CamSwitchManager>().snapBackFunction();
                screenTouched = false;
            }

        }
        Vector3 pos = transform.position;

        //Up
        if(Input.GetTouch(0).position.y <= Screen.height - screenSizeThickness)
        {
            Debug.Log("Top Screen has been pressed");
            this.gameObject.GetComponent<CamSwitchManager>().touchTop();
            screenTouched = true;
            pos.x -= camspeed * Time.deltaTime;
            // pos.z -= camspeed = Time.deltaTime;
        }

        //Down
        if (Input.GetTouch(0).position.y >= screenSizeThickness)
        {
            Debug.Log("Bottom Screen has been pressed");
            this.gameObject.GetComponent<CamSwitchManager>().touchBot();
            screenTouched = true;
            pos.x += camspeed * Time.deltaTime;

            // pos.z -= camspeed = Time.deltaTime;
        }
        //Right
        if (Input.GetTouch(0).position.x <= Screen.height - screenSizeThickness)
        {
            Debug.Log("Right Screen has been pressed");
            this.gameObject.GetComponent<CamSwitchManager>().touchRight();
            screenTouched = true;
            pos.z += camspeed * Time.deltaTime;

            // pos.z -= camspeed = Time.deltaTime;
        }
        //Left
        if (Input.GetTouch(0).position.x >= screenSizeThickness)
        {
            Debug.Log("Left Screen has been pressed");
            this.gameObject.GetComponent<CamSwitchManager>().touchLeft();
            screenTouched = true;
            pos.z -= camspeed * Time.deltaTime;

            // pos.z -= camspeed = Time.deltaTime;
        }

        transform.position = pos;




    }
}
