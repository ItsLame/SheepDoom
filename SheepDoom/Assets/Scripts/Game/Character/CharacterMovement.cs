using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Mirror;

namespace SheepDoom
{   
    public class CharacterMovement : NetworkBehaviour
    {
        public float speed;

        [Space(15)]
        public bool isDead;

        // Start is called before the first frame update
        void Awake()
        {
            isDead = false;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!hasAuthority) return;
            Move();
        }

        private void Move()
        {
            if (!isDead)
            {
                Vector3 moveMe = new Vector3(CrossPlatformInputManager.GetAxisRaw("Vertical"), 0.0f,
                                             -CrossPlatformInputManager.GetAxisRaw("Horizontal")) * speed;

                if ((moveMe.x != 0) || (moveMe.z != 0))
                    this.transform.rotation = Quaternion.LookRotation(moveMe);

                this.transform.position += moveMe;
            }
        }
    }
}