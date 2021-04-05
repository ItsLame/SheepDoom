using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Mirror;

namespace SheepDoom
{   [RequireComponent(typeof(Rigidbody))]
    public class CharacterMovement : NetworkBehaviour
    {
        public float speed;
      //  private Rigidbody myRigidBody;

        [Space(15)]
        public bool isDead;

        // Start is called before the first frame update
        void Awake()
        {
           //myRigidBody = GetComponent<Rigidbody>();
            isDead = false;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!hasAuthority) return;
            Debug.Log("Did i run fixedupdate");
            Move();
        }

        private void Move()
        {
            if (!isDead)
            {
                Debug.Log("Did i run in ifstatement1");
                Vector3 moveMe = new Vector3(CrossPlatformInputManager.GetAxisRaw("Vertical"), 0.0f,
                                             -CrossPlatformInputManager.GetAxisRaw("Horizontal")) * speed;

                if ((moveMe.x != 0) || (moveMe.z != 0))
                {
                    Debug.Log("Did i run in ifstatement2");
                    this.transform.rotation = Quaternion.LookRotation(moveMe);
                }

                this.transform.position += moveMe;
                //myRigidBody.velocity = moveMe;
            }
        }
    }
}