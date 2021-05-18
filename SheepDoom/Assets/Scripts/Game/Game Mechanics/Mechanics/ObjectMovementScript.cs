using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class ObjectMovementScript : NetworkBehaviour
    {
        [Header("Default Spawn Point")]
        //       public Transform originalTransform;

        [Header("Movement Properties")]
        [SerializeField]
        private float moveSpd;
        [SerializeField]
        private bool destroyOnContact, isGoingUp, isGoingDown, isGoingLeft, isGoingRight, isGoingStraight, isGoingBack;


        [Header("Timing Properties")]
        [SerializeField]
        private float currentTimer;
        [SerializeField]
        private bool isMoving, stop;

        public override void OnStartClient()
        {
            if (GetComponent<GetParents>().getParent().GetComponent<NetworkIdentity>().hasAuthority)
                isMoving = false;
        }

        [Client]
        public void SetMoveSpeed(float _moveSpd)
        {
            moveSpd = _moveSpd;
        }

        //for singular movement
        [Client]
        public void move(float time, string direction)
        {
            isMoving = true;
            currentTimer = time;

            switch (direction)
            {
                case "up":
                    isGoingUp = true;
                    break;
                case "down":
                    isGoingDown = true;
                    break;
                case "left":
                    isGoingLeft = true;
                    break;
                case "right":
                    isGoingRight = true;
                    break;
                case "straight":
                    isGoingStraight = true;
                    break;
                case "back":
                    isGoingBack = true;
                    break;
                default:
                    break;
            }
        }

        [Client]
        public void stopMoving()
        {
            isMoving = false;
            stop = false;

            //set all movement to false;
            isGoingUp = false;
            isGoingDown = false;
            isGoingLeft = false;
            isGoingRight = false;
            isGoingStraight = false;
            isGoingBack = false;
            //if(GetComponent<GetParents>().getParent().GetComponent<PlayerAdmin>().getCharID() == 2)
               // GetComponent<GetParents>().getParent().GetComponent<Character2>().ServerSetHitBox(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (isServer || !GetComponent<GetParents>().getParent().GetComponent<NetworkIdentity>().hasAuthority) return;

            //stop moving when time is up
            if (stop)
                stopMoving();

            //movement choices
            if(isMoving)
            {
                if(currentTimer >= 0)
                    currentTimer -= Time.deltaTime;

                if (currentTimer <= 0)
                    stop = true;

                if (isGoingUp)
                    transform.Translate(Vector3.up * moveSpd * Time.deltaTime);

                if (isGoingDown)
                     transform.Translate(Vector3.down * moveSpd * Time.deltaTime);

                if (isGoingLeft)
                    transform.Translate(Vector3.left * moveSpd * Time.deltaTime);

                if (isGoingRight)
                    transform.Translate(Vector3.right * moveSpd * Time.deltaTime);

                if (isGoingStraight)
                    transform.Translate(Vector3.forward * moveSpd * Time.deltaTime);

                if (isGoingBack)
                    transform.Translate(-Vector3.forward * moveSpd * Time.deltaTime);
            }
        }
    }
}
