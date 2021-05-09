using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class touch_fortesting : MonoBehaviour
{
    public GameObject player;
    public float speed;

    public bool left;
    public bool right;
    public bool up;
    public bool down;

    public void leftTrue()
    {
        left = true;
    }

    public void rightTrue()
    {
        right = true;
    }

    public void upTrue()
    {
        up = true;
    }

    public void downTrue()
    {
        down = true;
    }

    // ---------------------------------
    public void leftFalse()
    {
        left = false;
    }

    public void rightFalse()
    {
        right = false;
    }

    public void upFalse()
    {
        up = false;
    }

    public void downFalse()
    {
        down = false;
    }

    public void Update()
    {
        if (up)
        {
            player.transform.position += Vector3.forward * speed * Time.deltaTime;
        }

        if (down)
        {
            player.transform.position -= Vector3.forward * speed * Time.deltaTime;
        }

        if (left)
        {
            player.transform.position += Vector3.left * speed * Time.deltaTime;
        }

        if (right)
        {
            player.transform.position += Vector3.right * speed * Time.deltaTime;
        }
    }
}
