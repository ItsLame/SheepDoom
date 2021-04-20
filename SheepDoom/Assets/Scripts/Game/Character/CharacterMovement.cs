using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Mirror;

namespace SheepDoom
{   
    public class CharacterMovement : NetworkBehaviour
    {
        //original speed
        public float baseSpeed;
        //speed to be used in game
        public float speed;

        public bool isDebuffed;
        public float debuffDuration;
        public float debuffStrength;

        private void Start()
        {
            speed = baseSpeed;
        }


        //for player debuffs handling
        public void debuffCharacter(string type, float duration, float strength)
        {
            isDebuffed = true;

            if (type == "slow")
            {
                Debug.Log("Inflicting slow debuff");
                debuffDuration = duration;
                debuffStrength = strength;
                speed *= debuffStrength;
            }

            else if (type == "stop")
            {
                Debug.Log("Inflicting stop debuff");
                debuffDuration = duration;
                speed = 0;
            }

        }


        // Update is called once per frame
        void FixedUpdate()
        {
            if (!hasAuthority) return;

            if (isDebuffed)
            {
                //reduce timer per second
                debuffDuration -= Time.deltaTime;

                //remove debuff when duration is up
                if (debuffDuration <= 0)
                {
                    isDebuffed = false;
                    debuffStrength = 1;
                    speed = baseSpeed;
                }
            }

            Move();
        }

        private void Move()
        {
            if (!GetComponent<PlayerHealth>().isPlayerDead())
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