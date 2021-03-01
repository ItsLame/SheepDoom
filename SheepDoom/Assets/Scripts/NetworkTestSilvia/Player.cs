using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{
    //to move local player only
    void HandleMovement()
    {
        if(isLocalPlayer)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveHorizontal * 0.1f, moveVertical * 0.1f, 0);
            transform.position = transform.position + movement;
        }
    }

    void Update()
    {
        HandleMovement();

        if(isLocalPlayer && Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Sending a message to server!");
            Hello();
        }
    }

    public override void OnStartServer()
    {
        Debug.Log("Player spawned to server!");
    }

    [Command]
    void Hello()
    {
        Debug.Log("Received a message from client!: Hello!");
    }
}
