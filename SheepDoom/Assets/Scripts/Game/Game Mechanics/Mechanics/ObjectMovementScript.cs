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
            gameObject.GetComponentInParent<PlayerAttack>().ServerSetHitBox(false);
            GetComponent<GetParents>().getParent().GetComponent<PlayerAttack>().ServerSetHitBox(false);
            //this.gameObject.GetComponent<OnTouchHealth>().SetHitBox(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (isServer || !GetComponent<GetParents>().getParent().GetComponent<NetworkIdentity>().hasAuthority) return;
            //calculate timer
            /*if (isMoving && currentTimer >= 0)
                currentTimer -= Time.deltaTime;*/

            //change bool to call stop once when time is up
            /*if (isMoving && currentTimer <= 0)
                stop = true;*/

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


/*
        public void goStraight(float duration)
        {
            currentTimer = duration;

            while (currentTimer >= 0)
            {
                currentTimer -= Time.deltaTime;
                transform.Translate(Vector3.forward * moveSpd * Time.deltaTime);
            }

        }

        public void goBack(float duration)
        {
            currentTimer = duration;

            while (currentTimer >= 0)
            {
                currentTimer -= Time.deltaTime;
                transform.Translate(-Vector3.forward * moveSpd * Time.deltaTime);
            }

        }

        public void goUp(float duration)
        {
            currentTimer = duration;

            while (currentTimer >= 0)
            {
                currentTimer -= Time.deltaTime;
                transform.Translate(Vector3.up * moveSpd * Time.deltaTime);
            }

        }

        public void goDown(float duration)
        {
            currentTimer = duration;

            while (currentTimer >= 0)
            {
                currentTimer -= Time.deltaTime;
                transform.Translate(Vector3.down * moveSpd * Time.deltaTime);
            }

        }

        public void goLeft(float duration)
        {
            currentTimer = duration;

            while (currentTimer >= 0)
            {
                Debug.Log("currentTimer = " + currentTimer);
                currentTimer -= Time.deltaTime;
                transform.Translate(Vector3.left * moveSpd * Time.deltaTime);
            }

        }

        public void goRight(float duration)
        {
            currentTimer = duration;

            while (currentTimer >= 0)
            {
                Debug.Log("currentTimer = " + currentTimer);
                //              currentTimer -= Time.deltaTime;
                transform.Translate(Vector3.right * moveSpd * Time.deltaTime);
            }

        }

*/

/*
public bool isGoingUp;
public float goUpTimer;

public bool isGoingDown;
public float goDownTimer;

public bool isGoingLeft;
public float goLeftTimer;

public bool isGoingRight;
public float goRightTimer;

public bool isGoingStraight;
public float goStraightTimer;

public bool isGoingBack;
public float goBackTimer;

 
        // NOT WORKING NOT WORKING NOT WORKING to call the movements n execute them step by step
        // DIRECTIONS: up, down, left, right, straight, back
        // TIME means how long u want it to go
        // numberOfMovements = how many repetitions
        public void SetMovement(float numberOfMovements, string firstMovement, string secondMovement, string thirdMovement, float firstTime, float secondTime, float thirdTime)
        {
            float movementsLeft = numberOfMovements;

            while (movementsLeft != 0)
            {
                currentTimer = firstTime;
                isMoving = true;

                //first movement
                if (currentTimer >= 0)
                {
                    switch (firstMovement)
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

                currentTimer = secondTime;

                if (currentTimer >= 0)
                {
                    switch (secondMovement)
                    {
                        case "up":
                            isGoingUp = true;
                            currentTimer = firstTime;
                            break;

                        case "down":
                            isGoingDown = true;
                            currentTimer = firstTime;
                            break;

                        case "left":
                            isGoingLeft = true;
                            currentTimer = firstTime;
                            break;

                        case "right":
                            isGoingRight = true;
                            currentTimer = firstTime;
                            break;

                        case "straight":
                            isGoingStraight = true;
                            currentTimer = firstTime;
                            break;

                        case "back":
                            isGoingBack = true;
                            currentTimer = firstTime;
                            break;

                        default:
                            break;
                    }
                }

                currentTimer = thirdTime;

                if (currentTimer >= 0)
                {
                    switch (thirdMovement)
                    {
                        case "up":
                            isGoingUp = true;
                            currentTimer = firstTime;
                            break;

                        case "down":
                            isGoingDown = true;
                            currentTimer = firstTime;
                            break;

                        case "left":
                            isGoingLeft = true;
                            currentTimer = firstTime;
                            break;

                        case "right":
                            isGoingRight = true;
                            currentTimer = firstTime;
                            break;

                        case "straight":
                            isGoingStraight = true;
                            currentTimer = firstTime;
                            break;

                        case "back":
                            isGoingBack = true;
                            currentTimer = firstTime;
                            break;

                        default:
                            break;
                    }
                }


                movementsLeft -= 1;
            }

            stop = true;

            //reset position
            //           this.gameObject.transform.position = originalTransform.position;

            //turn off object renderer
  //                     this.gameObject.GetComponent<MeshRenderer>().enabled = false;

            //turn off hitbox script
    //          this.gameObject.GetComponent<OnTouchHealth>().enabled = false;
        }
 */
