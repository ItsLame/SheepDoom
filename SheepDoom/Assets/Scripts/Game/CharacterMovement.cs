using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CharacterMovement : MonoBehaviour
{
    public float speed;
    private int idle;
    private Rigidbody myRigidBody;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 moveMe = new Vector3(-CrossPlatformInputManager.GetAxis("Vertical"), 0.0f,
                                     CrossPlatformInputManager.GetAxis("Horizontal")) * speed;
                                     
        myRigidBody.velocity = moveMe;

        /*if(CrossPlatformInputManager.GetAxis("Horizontal") > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            if (CrossPlatformInputManager.GetAxis("Vertical") > 0)
            {
                Vector3 moveVec = new Vector3(1, 0, 1) * speed;
                myRigidBody.velocity = moveVec;
            }else if (CrossPlatformInputManager.GetAxis("Vertical") < 0)
            {
                Vector3 moveVec = new Vector3(1, 0, -1) * speed;
                myRigidBody.velocity = moveVec;
            }
            idle = 1;
        } else if (CrossPlatformInputManager.GetAxis("Horizontal") < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            if(CrossPlatformInputManager.GetAxis("Vertical") > 0)
            {
                Vector3 moveVec = new Vector3(-1, 0, 1) * speed;
                myRigidBody.velocity = moveVec;
            }else if (CrossPlatformInputManager.GetAxis("Vertical") < 0)
            {
                Vector3 moveVec = new Vector3(-1, 0, -1) * speed;
                myRigidBody.velocity = moveVec;
            }
            idle = 1;
        }
        else
        {
            if (idle == 1)
            {
                Vector3 moveVec = new Vector3(0, 0, 0);
                myRigidBody.velocity = moveVec;
                idle = 0;
            }
        }*/
    }
}
