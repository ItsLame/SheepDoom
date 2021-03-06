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
        Vector3 moveMe = new Vector3(CrossPlatformInputManager.GetAxis("Vertical"), 0.0f,
                                     -CrossPlatformInputManager.GetAxis("Horizontal")) * speed;

        if ((moveMe.x != 0) || (moveMe.z != 0))
        {
            myRigidBody.rotation = Quaternion.LookRotation(moveMe);
        }
        myRigidBody.velocity = moveMe;
    }
}
