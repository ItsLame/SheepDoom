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
        //[SyncVar(hook = nameof(SyncPlayerSpeed))]
        public float speed;

        public bool isSpeedAltered;
        public float speedAlterDuration;
        public float speedAlterStrength;

        public override void OnStartClient()
        {
            if (!hasAuthority) return;
            speed = baseSpeed;
        }

        [Server]
        public void changeSpeed(string type, float duration, float strength)
        {
            TargetChangeSpeed(type, duration, strength);
        }

        
        //for player debuffs handling
        [TargetRpc] // calculate effects on movement on client, then client-auth sync movement into server
        void TargetChangeSpeed(string type, float duration, float strength)
        {
            isSpeedAltered = true;
            speedAlterDuration = duration;
            if (type == "slow")
            {
                Debug.Log("Inflicting slow debuff");
                speedAlterStrength = strength;
                speed *= speedAlterStrength;
            }
            else if (type == "stop")
            {
                Debug.Log("Inflicting stop debuff");
                speed = 0;
            }
            else if (type == "speedUp")
            {
                Debug.Log("Speeding up..");
                speedAlterStrength = strength;
                speed *= speedAlterStrength;
            }
        }

        void FixedUpdate()
        {
            if(!hasAuthority) return;

            Move();

            if (isSpeedAltered)
                DebuffTimerCountdown();
        }

        [Client]
        private void DebuffTimerCountdown()
        {
            //reduce timer per second
            speedAlterDuration -= Time.deltaTime;
            Debug.Log("Speed Alter Duration: " + speedAlterDuration);

            //remove debuff when duration is up
            if (speedAlterDuration <= 0)
            {
                speedAlterStrength = 1;
                speed = baseSpeed;
                Debug.Log("Speed Over:" + speedAlterStrength + ", " + speed);
                isSpeedAltered = false;
            }
        }

        [Client]
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