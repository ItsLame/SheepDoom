using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class StartAnimatingScript : NetworkBehaviour
    {
        [Header("Movement configurations")]
        [SerializeField] private bool moveDown;
        [SerializeField] private bool moveUp;

        [SerializeField] private float moveDuration;
        [SerializeField] private float moveDurationInGame;

        [SerializeField] private float moveSpd;

        public override void OnStartServer()
        {
            moveDurationInGame = moveDuration;
        }

        private void Update()
        {
            if (isServer)
            {
                if (moveDown)
                {
                    moveDurationInGame -= Time.deltaTime;
                    gameObject.transform.position += transform.TransformDirection(Vector3.right) * moveSpd * Time.deltaTime;

                    if (moveDurationInGame <= 0)
                    {
                        moveUp = true;
                        moveDurationInGame = moveDuration;
                        moveDown = false;
                    }
                }

                if (moveUp)
                {
                    moveDurationInGame -= Time.deltaTime;
                    gameObject.transform.position += transform.TransformDirection(Vector3.left) * moveSpd * Time.deltaTime;

                    if (moveDurationInGame <= 0)
                    {
                        moveDown = true;
                        moveDurationInGame = moveDuration;
                        moveUp = false;
                    }
                }
            }
        }
    }
}

