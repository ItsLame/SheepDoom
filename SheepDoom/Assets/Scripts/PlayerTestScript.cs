using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerTestScript : NetworkBehaviour
{
    void Movement()
    {
        if (isLocalPlayer)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);
            transform.position = transform.position + movement;
        }

    }

    void Update()
    {
        Movement();    
    }
}
