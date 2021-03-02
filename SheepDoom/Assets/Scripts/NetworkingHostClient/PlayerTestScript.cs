using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerTestScript : MonoBehaviour
{
    public int speed = 1;
    void Movement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal/100 * speed, 0, moveVertical/100 * speed);
        transform.position = transform.position + movement;

    }

    void Update()
    {
        Movement();    
    }
}
