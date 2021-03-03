using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAtPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    public float fixXdist = 30.0f;
    public float fixYdist = 30.0f;
    public float fixZdist = 5.0f;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if player is still alive
        if (player != null)
        {
            transform.position = new Vector3(player.transform.position.x + fixXdist, player.transform.position.y + fixYdist, player.transform.position.z + fixZdist);
        }

        //to move camera up
        if (Input.GetKey(KeyCode.UpArrow))
        {
            fixYdist += 0.1f;
        }

        //to move camera down
        if (Input.GetKey(KeyCode.DownArrow))
        {
            fixYdist -= 0.1f;
        }

        //to move camera left
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            fixZdist -= 0.1f;
        }

        //to move camera right
        if (Input.GetKey(KeyCode.RightArrow))
        {
            fixZdist += 0.1f;
        }

        //to move camera forward
        if (Input.GetKey(KeyCode.I))
        {
            fixXdist += 0.1f;
        }

        //to move camera backward
        if (Input.GetKey(KeyCode.K))
        {
            fixXdist -= 0.1f;
        }
    } 
}