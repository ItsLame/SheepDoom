using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerTestScript : MonoBehaviour
{
    public int speed = 1;
    void Movement()
    {
        Vector3 pos = transform.position;
        if (Input.GetKey(KeyCode.D))
            pos.z += speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
            pos.z -= speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S))
            pos.x += speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.W))
            pos.x -= speed * Time.deltaTime;

        transform.position = pos;
    }

    void Update()
    {
        Movement();    
    }
}
