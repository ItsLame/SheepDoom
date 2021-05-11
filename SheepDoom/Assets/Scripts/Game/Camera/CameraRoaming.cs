using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraRoaming : MonoBehaviour
{
    public float camspeed = 30;

    //for left, right , up n down margins
    public float LeftMargin = Screen.width / 20;
    public float RightMargin = Screen.width / 20;
    public float UpMargin = Screen.height / 15;
    public float DownMargin = Screen.height / 20;

    public bool screenTouched = false;

   // public Text m_Text;

    // camera transform upon victory
    public void VictoryRoam(Transform _destination)
    {
        this.gameObject.GetComponent<CamSwitchManager>().isVictory = true;

        StartCoroutine(VictoryRoamStart(_destination));
    }

    private IEnumerator VictoryRoamStart(Transform _destination)
    {
        while((transform.position - _destination.position).sqrMagnitude > 1)
        {
            // adjust positioning here
            this.gameObject.transform.position = Vector3.Lerp(transform.position, _destination.position, Time.unscaledDeltaTime);
            this.gameObject.transform.rotation = Quaternion.Lerp(transform.rotation, _destination.rotation, Time.unscaledDeltaTime);

            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // to ignore update when victory
        if(this.gameObject.GetComponent<CamSwitchManager>().isVictory)
            return;
            
        //to check position
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
          //  m_Text.text = "Touch Position : " + touch.position;
      //      Debug.Log("Screen has been touched");
      //      Debug.Log("Position X:" + Input.GetTouch(0).position.x);
     //       Debug.Log("Position Y:" + Input.GetTouch(0).position.y);
        }
        else
        {
           // m_Text.text = "No touch contacts";
        }

    //    Debug.Log("Screentouched: " + screenTouched);

        if (screenTouched)
        {
            if (Input.touchCount == 0)
            {
       //         Debug.Log("Screen edges have been let go");
                this.gameObject.GetComponent<CamSwitchManager>().snapBackFunction();
                screenTouched = false;
            }
        }
        Vector3 pos = transform.position;

        //somehow the position where the camera moves is fking flipped from the area u pressed
        //changes are pos camera + to - and - to + to make sensible debug logs -jh
        //for joystick press + border press
        //Up
        if (Input.touchCount > 1 )
        {
            if (Input.GetTouch(1).position.y >= Screen.height - UpMargin)
            {
                Debug.Log("Top Screen has been pressed");
                this.gameObject.GetComponent<CamSwitchManager>().touchTop();
                screenTouched = true;
                pos.x += camspeed * Time.deltaTime;
                // pos.z -= camspeed = Time.deltaTime;
            }

            //Down
            if (Input.GetTouch(1).position.y <= DownMargin)
            {
                Debug.Log("Bottom Screen has been pressed");
                this.gameObject.GetComponent<CamSwitchManager>().touchBot();
                screenTouched = true;
                pos.x -= camspeed * Time.deltaTime;

                // pos.z -= camspeed = Time.deltaTime;
            }
            //Right
            if (Input.GetTouch(1).position.x >= Screen.width - RightMargin)
            {
                Debug.Log("Right Screen has been pressed");
                this.gameObject.GetComponent<CamSwitchManager>().touchRight();
                screenTouched = true;
                pos.z -= camspeed * Time.deltaTime;

                // pos.z -= camspeed = Time.deltaTime;
            }
            //Left
            if (Input.GetTouch(1).position.x <= LeftMargin)
            {
                Debug.Log("Left Screen has been pressed");
                this.gameObject.GetComponent<CamSwitchManager>().touchLeft();
                screenTouched = true;
                pos.z += camspeed * Time.deltaTime;

                // pos.z -= camspeed = Time.deltaTime;
            }

            if (Input.touches[1].phase == TouchPhase.Ended)
            {
                this.gameObject.GetComponent<CamSwitchManager>().snapBackFunction();
            }

            transform.position = pos;
        }

        //for single press to check the borders..
        else if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).position.y >= Screen.height - UpMargin)
            {
                Debug.Log("Top Screen has been pressed");
                this.gameObject.GetComponent<CamSwitchManager>().touchTop();
                screenTouched = true;
                pos.x += camspeed * Time.deltaTime;
                // pos.z -= camspeed = Time.deltaTime;
            }

            //Down
            if (Input.GetTouch(0).position.y <= DownMargin)
            {
                Debug.Log("Bottom Screen has been pressed");
                this.gameObject.GetComponent<CamSwitchManager>().touchBot();
                screenTouched = true;
                pos.x -= camspeed * Time.deltaTime;

                // pos.z -= camspeed = Time.deltaTime;
            }
            //Right
            if (Input.GetTouch(0).position.x >= Screen.width - RightMargin)
            {
                Debug.Log("Right Screen has been pressed");
                this.gameObject.GetComponent<CamSwitchManager>().touchRight();
                screenTouched = true;
                pos.z -= camspeed * Time.deltaTime;

                // pos.z -= camspeed = Time.deltaTime;
            }
            //Left
            if (Input.GetTouch(0).position.x <= LeftMargin)
            {
                Debug.Log("Left Screen has been pressed");
                this.gameObject.GetComponent<CamSwitchManager>().touchLeft();
                screenTouched = true;
                pos.z += camspeed * Time.deltaTime;

                // pos.z -= camspeed = Time.deltaTime;
            }

            if (Input.touches[0].phase == TouchPhase.Ended)
            {
                this.gameObject.GetComponent<CamSwitchManager>().snapBackFunction();
            }

            transform.position = pos;
        }
    }
       
}
