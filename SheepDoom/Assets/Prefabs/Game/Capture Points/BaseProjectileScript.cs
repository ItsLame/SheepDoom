using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    [RequireComponent(typeof(Rigidbody))]
    public class BaseProjectileScript : NetworkBehaviour
    {
        //projectileOwner
        [SyncVar] public float ownerTeamID;

        [Space(15)]
        //rotation controls
        public float x_rotaspeed;
        public float y_rotaspeed;
        public float z_rotaspeed;

        [Header("Movement Properties")]
        [SerializeField] [SyncVar] public bool isMovingDown;
        [SerializeField] [SyncVar] public bool isMovingLeft;
        [SerializeField] [SyncVar] public bool isMovingRight;

        [Header("Damage Properties")]
        [Space(15)]
        public int damage;
        public bool destroyOnContact; //if projectile ill stop on first contact
        [SerializeField]
        private Rigidbody m_Rigidbody;


        [Header("Bullet Properties")]
        [Space(15)]
        public float m_Speed;   // default speed of projectile
        public float m_Lifespan; // Lifespan per second
        private float m_StartTime;
        [Space(10)]
        public bool isAccelerating = false;
        public bool isDeccelerating = false;
        public float accelerationRate;
        public float deccelerationRate;
        [Space(10)]
        public bool accelMultiplierOn;
        public float accelMultiplier;
        [Space(10)]
        public bool hasSpeedLimit;
        public float speedLimit;

        [Header("Projectile Debuff Properties")]
        [Space(15)]
        public bool SlowDebuff;
        public float slowRate;
        public float slowDebuffDuration;

        [Space(15)]
        public bool StopDebuff;
        public float stopDebuffDuration;

        [Space(15)]
        public bool SleepDebuff;
        public float sleepDebuffDuration;


        [ServerCallback]
        void OnTriggerEnter(Collider col)
        {
            //if hit player
            if (col.gameObject.CompareTag("Player"))
            {
                bool hit = false; 
                //dont hurt the owner of the projectile, dont increase score if hitting dead player
                if (!col.gameObject.GetComponent<PlayerHealth>().isPlayerDead())
                {
                    Debug.Log("How many times i hit player?" + (damage));
                    //reduce HP of hit target
                    col.gameObject.GetComponent<PlayerHealth>().modifyinghealth(-damage);

                    //debuff player depending on bullet properties in inspector
                    if (SlowDebuff)
                        col.gameObject.GetComponent<CharacterMovement>().changeSpeed("slow", slowDebuffDuration, slowRate);

                    else if (StopDebuff)
                        col.gameObject.GetComponent<CharacterMovement>().changeSpeed("stop", stopDebuffDuration, 0);

                    else if (SleepDebuff)
                        col.gameObject.GetComponent<CharacterMovement>().changeSpeed("sleep", sleepDebuffDuration, 0);

                    //increase killer's kill count if target is killed
                    if (col.gameObject.GetComponent<PlayerHealth>().getHealth() <= 0)
                    {
                        col.gameObject.GetComponent<PlayerHealth>().SetPlayerDead();
                        col.gameObject.GetComponent<GameEvent>().whoKilled = this.gameObject.name;

                    }

                    if (destroyOnContact)
                    {
                        Debug.Log("How many times was i destroyed?");
                        Destroyy();
                    }
                        
                }
            }


            else if (col.gameObject.CompareTag("Other"))
            {
                if (destroyOnContact)
                    Destroyy();
            }

            else if (col.gameObject.CompareTag("Shield"))
            {
                if (col.gameObject.GetComponent<ObjectFollowScript>().teamID != ownerTeamID)
                {
                    if (destroyOnContact)
                        Destroyy();
                }
            }
        }

        [Server]
        private void Destroyy()
        {
            NetworkServer.Destroy(gameObject);
        }


        void Update()
        {
            if (isServer)
            {
                //rotation movement
                transform.Rotate(1.0f * x_rotaspeed, 1.0f * y_rotaspeed, 1.0f * z_rotaspeed);

                if (isMovingDown)
                    transform.Translate(Vector3.down * m_Speed * Time.deltaTime);
                else if (isMovingLeft)
                    transform.Translate(Vector3.left * m_Speed * Time.deltaTime);
                else if (isMovingRight)
                    transform.Translate(Vector3.right * m_Speed * Time.deltaTime);
                else
                    transform.Translate(Vector3.forward * m_Speed * Time.deltaTime); //basic forward movement

                //adjust acceleration rate per frame
                if (isAccelerating)
                {
                    //stop accelerating once hit speed limit
                    if (hasSpeedLimit && (m_Speed <= speedLimit))
                        m_Speed += accelerationRate;
                    else
                        m_Speed += accelerationRate;
                }

                //stop deccelerating once hit speed limit
                if (isDeccelerating && (m_Speed >= speedLimit))
                    m_Speed -= accelerationRate;

                //adjusting acceleration ratess 
                if (accelMultiplierOn)
                {
                    if (m_Speed <= speedLimit) return;
                    accelerationRate *= accelMultiplier;
                }

                m_StartTime += Time.deltaTime;
                if (m_StartTime >= m_Lifespan)
                    Destroyy();
            }
        }
    }
}
