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


        [SyncVar] public bool isSpeedAltered;
        [SyncVar] public float speedAlterDuration;
        [SyncVar] public float speedAlterStrength;

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
        public void changeSpeed(string type, float duration, float strength)
        {
            isSpeedAltered = true;

            if (type == "slow")
            {
                Debug.Log("Inflicting slow debuff");
                speedAlterDuration = duration;
                speedAlterStrength = strength;
                speed *= speedAlterStrength;
            }

            else if (type == "stop")
            {
                Debug.Log("Inflicting stop debuff");
                speedAlterDuration = duration;
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

            if (isSpeedAltered)
            {
                DebuffTimerCountdown();
            }
        }

        [Command]
        public void DebuffTimerCountdown()
        {
            //reduce timer per second
            speedAlterDuration -= Time.deltaTime;
            Debug.Log("Speed Alter Duration: " + speedAlterDuration);

            //remove debuff when duration is up
            if (speedAlterDuration <= 0)
            {
                speedAlterStrength = 1;
                speed = baseSpeed;
                Debug.Log("Debuff Over:" + speedAlterStrength + ", " + speed);
                isSpeedAltered = false;
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