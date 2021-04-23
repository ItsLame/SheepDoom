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
        public float moveSpd;
        public bool destroyOnContact;

        public bool isGoingUp = false; 

        public bool isGoingDown = false;

        public bool isGoingLeft = false;

        public bool isGoingRight = false;

        public bool isGoingStraight = false;

        public bool isGoingBack = false;

        [Header("Timing Properties")]
        public float currentTimer;
        public bool isMoving = false;
        public bool stop = false;

        // Start is called before the first frame update
        void Start()
        {
            isMoving = false;

        }


        /*
        // to call the movements n execute them step by step
        // DIRECTIONS: up, down, left, right, straight, back
        // TIME means how long u want it to go
        // numberOfMovements = how many repetitions
        public void SetMovement(float numberOfMovements, string firstMovement, string secondMovement, string thirdMovement, float firstTime, float secondTime, float thirdTime)
        {
            float movementsLeft = numberOfMovements;

            while (movementsLeft != 0)
            {
                switch (firstMovement)
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

                if (secondMovement != null)
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

                if (thirdMovement != null)
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

                movementsLeft -= 1;
            }

            //reset position
            //           this.gameObject.transform.position = originalTransform.position;

            //turn off object renderer
            //           this.gameObject.GetComponent<MeshRenderer>().enabled = false;

            //turn off hitbox script
            //           this.gameObject.GetComponent<OnTouchHealth>().enabled = false;
        } */

        //for singular movement
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

        public void stopMoving()
        {
            isMoving = false;
            Debug.Log("Melee Stop");
            stop = false;
        }

        // Update is called once per frame
        void Update()
        {
            //calculate timer
            if (isMoving && currentTimer >= 0)
            {
                currentTimer -= Time.deltaTime;
                Debug.Log(currentTimer);
            }

            //change bool to call stop once when time is up
            if (isMoving && currentTimer <= 0)
            {
                stop = true;
            }

            //stop moving when time is up
            if (stop)
            {
                stopMoving();
            }

            /*
            //movement choices
            if (isGoingUp)
            {
                while (isMoving)
                {
                    currentTimer -= Time.deltaTime;
                    transform.Translate(Vector3.up * moveSpd * Time.deltaTime);
                }

                isGoingUp = false;
            }

            if (isGoingDown)
            {
                while (isMoving)
                {
                    currentTimer -= Time.deltaTime;
                    transform.Translate(Vector3.down * moveSpd * Time.deltaTime);
                }

                isGoingDown = false;
            }

            if (isGoingLeft)
            {
                while (isMoving)
                {
                    Debug.Log("currentTimer = " + currentTimer);
                    currentTimer -= Time.deltaTime;
                    transform.Translate(Vector3.left * moveSpd * Time.deltaTime);
                }

                isGoingLeft = false;
            }

            */

            if (isGoingRight)
            {
                while (isMoving)
                {
                    Debug.Log("currentTimer = " + currentTimer);
                    transform.Translate(Vector3.right * moveSpd * Time.deltaTime);
                }

                isGoingRight = false;
            }

            /*
            if (isGoingStraight)
            {
                while (isMoving)
                {
                    currentTimer -= Time.deltaTime;
                    transform.Translate(Vector3.forward * moveSpd * Time.deltaTime);
                }

                isGoingStraight = false;
            }

            if (isGoingBack)
            {
                while (isMoving)
                {
                    currentTimer -= Time.deltaTime;
                    transform.Translate(-Vector3.forward * moveSpd * Time.deltaTime);
                }

                isGoingBack = false;
            }*/
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
public float goBackTimer;*/
