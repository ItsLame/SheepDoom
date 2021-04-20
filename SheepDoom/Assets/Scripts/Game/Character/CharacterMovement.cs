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
        [SyncVar(hook = nameof(SyncPlayerSpeed))]
        public float speed;


        [SyncVar] public bool isDebuffed;
        [SyncVar] public float debuffDuration;
        [SyncVar] public float debuffStrength;

        private void Start()
        {
            speed = baseSpeed;
        }

        public void SyncPlayerSpeed(float oldValue, float newValue)
        {
    //        if (hasAuthority)
    //        {

    //        }
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

            Move();
        }

        private void Update()
        {
            if (!hasAuthority) return;

            if (isDebuffed)
            {
                DebuffTimerCountdown();
            }
        }

        [Command]
        public void DebuffTimerCountdown()
        {
            //reduce timer per second
            debuffDuration -= Time.deltaTime;
            Debug.Log("Debuff Duration: " + debuffDuration);

            //remove debuff when duration is up
            if (debuffDuration <= 0)
            {
                debuffStrength = 1;
                speed = baseSpeed;
                Debug.Log("Debuff Over:" + debuffStrength + ", " + speed);
                isDebuffed = false;
            }
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