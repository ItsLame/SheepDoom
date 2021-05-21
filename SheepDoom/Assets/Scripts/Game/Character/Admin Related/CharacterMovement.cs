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

        public bool isSpeedAltered;
        public float speedAlterDuration;
        public float speedAlterStrength;

        [Header("Debuffs check")]
        [Space(15)]
        [SyncVar] public bool isSlowed;
        [SyncVar] public bool isStopped;
        [SyncVar] public bool isSleeped;
        [SyncVar] public bool isSpeedUp;



        //Animation
        [SerializeField] private Animator animator;

        public override void OnStartClient()
        {
            if (!hasAuthority) return;
            speed = baseSpeed;
        }

        [Server]
        public void changeSpeed(string type, float duration, float strength)
        {
            TargetChangeSpeed(connectionToClient, type, duration, strength);
        }


        //for player debuffs handling
        [TargetRpc] // calculate effects on movement on client, then client-auth sync movement into server
        void TargetChangeSpeed(NetworkConnection conn, string type, float duration, float strength)
        {
            //if target is already debuffed, replace it with new debuff
            if (isSpeedAltered)
            {
                CmdSetBuffStatus("all", false);
            }

            isSpeedAltered = true;
            speedAlterDuration = duration;
            if (type == "slow")
            {
            //    Debug.Log("Inflicted with slow debuff");
                CmdSetBuffStatus("slow", true);
                speedAlterStrength = strength;
                speed *= speedAlterStrength;
            }
            else if (type == "stop")
            {
           //     Debug.Log("Inflicted with stop debuff");
                CmdSetBuffStatus("stop", true);
                speed = 0;
            }

            else if (type == "sleep")
            {
           //     Debug.Log("Inflicted with sleep debuff");
                CmdSetBuffStatus("sleep", true);
                speed = 0;
            }

            else if (type == "speedUp")
            {
        //        Debug.Log("Speeding up..");
                CmdSetBuffStatus("speedUp", true);
                speedAlterStrength = strength;
                speed *= speedAlterStrength;
            }
        }

        void FixedUpdate()
        {
            if (!hasAuthority) return;

            Move();

            if (isSpeedAltered)
                DebuffTimerCountdown();
        }

        [Client]
        private void DebuffTimerCountdown()
        {
            //reduce timer per second
            speedAlterDuration -= Time.fixedDeltaTime;

            //remove debuff when duration is up
            if (speedAlterDuration <= 0)
            {
                speedAlterStrength = 1;
                speed = baseSpeed;
           //     Debug.Log("Speed Over:" + speedAlterStrength + ", " + speed);
                isSpeedAltered = false;

                //falsify all movespd effects on target
                if (isSlowed || isStopped || isSleeped || isSpeedUp)
                {
                    CmdSetBuffStatus("all", false);
                }

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
                {
                    animator.SetBool("Iswalking", true);
                    animator.SetTrigger("Move");
                    animator.ResetTrigger("Attack");
                    this.transform.rotation = Quaternion.LookRotation(moveMe);
                    this.transform.position += moveMe;
                }

                if ((moveMe.x == 0) && (moveMe.z == 0))
                {
                    animator.SetBool("Iswalking", false);
                    animator.ResetTrigger("Move");
                }
            }
        }

        [Command]
        void CmdSetBuffStatus(string name, bool _buffStatus)
        {
            if (name == "sleep")
            {
                isSleeped = _buffStatus;
            }

            else if (name == "slow")
            {
                isSlowed = _buffStatus;
            }

            else if (name == "stop")
            {
                isStopped = _buffStatus;
            }

            else if (name == "speedUp")
            {
                isSpeedUp = _buffStatus;
            }

            else if (name == "all")
            {
                isSleeped = _buffStatus;
                isSlowed = _buffStatus;
                isStopped = _buffStatus;
                isSpeedUp = _buffStatus;
            }
        }
    }
}
