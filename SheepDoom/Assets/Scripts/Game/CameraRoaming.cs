using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRoaming : MonoBehaviour
{

    public float camspeed = 1000;
    public float screenSizeThickness = 10;


    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;

        //Up
        if(Input.mousePosition.y <= Screen.height - screenSizeThickness)
        {
            pos.x -= camspeed = Time.deltaTime;

            // pos.z -= camspeed = Time.deltaTime;
        }
        //Down
        if (Input.mousePosition.y >= screenSizeThickness)
        {
            pos.x += camspeed = Time.deltaTime;

            // pos.z -= camspeed = Time.deltaTime;
        }
        //Right
        if (Input.mousePosition.x <= Screen.height - screenSizeThickness)
        {
            pos.z += camspeed = Time.deltaTime;

            // pos.z -= camspeed = Time.deltaTime;
        }
        //Left
        if (Input.mousePosition.x >= screenSizeThickness)
        {
            pos.z -= camspeed = Time.deltaTime;

            // pos.z -= camspeed = Time.deltaTime;
        }

        transform.position = pos;




    }
}
