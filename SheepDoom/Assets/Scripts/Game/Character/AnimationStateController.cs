using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class AnimationStateController : MonoBehaviour
{
    Animator animator;
    public float speed;
    private int idle;
    private Rigidbody myRigidBody;
    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        
    }

    void FixedUpdate()
    {
        Vector3 moveMe = new Vector3(CrossPlatformInputManager.GetAxis("Vertical"), 0.0f,
                                     -CrossPlatformInputManager.GetAxis("Horizontal")) * speed;

        if ((moveMe.x != 0) || (moveMe.z != 0))
        {
            animator.SetBool("IsWalking", true);
            myRigidBody.rotation = Quaternion.LookRotation(moveMe);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
        myRigidBody.velocity = moveMe;

    }
}
