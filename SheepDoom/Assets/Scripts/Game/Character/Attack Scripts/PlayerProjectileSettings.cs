using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerProjectileSettings : NetworkBehaviour
    {
        //projectileOwner
        //[SyncVar] cant syncvar an object of type gameobject
        public GameObject owner;

        [Space(15)]
        //rotation controls
        public float x_rotaspeed;
        public float y_rotaspeed;
        public float z_rotaspeed;

        [Header("Damage Properties")]
        [Space(15)]
        public int damage;
        public bool destroyOnContact; //if projectile ill stop on first contact
        private Rigidbody m_Rigidbody;


        [Header("Bullet Properties")]
        [Space(15)]
        public float m_Speed = 10f;   // default speed of projectile
        public float m_Lifespan = 3f; // Lifespan per second
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

        [Space(15)]
        public bool SlowDebuff = false;
        public float slowRate;
        public float slowDebuffDuration;

        [Space(15)]
        public bool StopDebuff = false;
        public float stopDebuffDuration;

        //bool for calling kill counter increase once
        bool killCounterIncreaseCalled = false;

        void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        void Start()
        {
            Destroy(gameObject, m_Lifespan);
         //   Debug.Log("Owner: " + owner);
        }

        [Server]
        void OnTriggerEnter(Collider col)
        {
            //if hit player
            if (col.gameObject.CompareTag("Player"))
            {
                //dont hurt the owner of the projectile, dont increase score if hitting dead player
                if (col.gameObject != owner && !col.gameObject.GetComponent<PlayerHealth>().isPlayerDead())
                {
                    //reduce HP of hit target
                    col.gameObject.GetComponent<PlayerHealth>().modifyinghealth(-damage);

                    //debuff player depending on bullet properties in inspector
                    if (SlowDebuff)
                    {
                        col.gameObject.GetComponent<CharacterMovement>().debuffCharacter("slow", slowDebuffDuration, slowRate);
                    }

                    if (StopDebuff)
                    {
                        col.gameObject.GetComponent<CharacterMovement>().debuffCharacter("stop", stopDebuffDuration, 0);
                    }


                    //increase killer's kill count if target is killed
                    if (col.gameObject.GetComponent<PlayerHealth>().getHealth() <= 0)
                    {
                        col.gameObject.GetComponent<PlayerHealth>().SetPlayerDead();
                        owner.GetComponent<PlayerAdmin>().IncreaseCount(false, true, false);
                        col.gameObject.GetComponent<GameEvent>().whoKilled = owner.gameObject.GetComponent<PlayerObj>().GetPlayerName();
                    }

                    if (destroyOnContact)
                    {
                        Object.Destroy(this.gameObject);
                    }
                }
            }

            else if (col.gameObject.CompareTag("Tower"))
            {
           //     col.transform.parent.gameObject.GetComponent<CapturePointScript>().ModifyingHealth(-damage);
           //     Object.Destroy(this.gameObject);
            }


            //used to test gold for now
            else if (col.gameObject.CompareTag("NeutralMinion"))
            {
                
                //take damage
 //               col.gameObject.GetComponent<NeutralCreepScript>().Attacker = owner;
 //               col.gameObject.GetComponent<NeutralCreepScript>().neutralTakeDamage(-damage);
                //inform that its under atk
 //               col.gameObject.GetComponent<NeutralCreepScript>().isUnderAttack();
                
                
//                Debug.Log(owner + " hitting neutral minion");
                owner.gameObject.GetComponent<CharacterGold>().varyGold(5);

                if (destroyOnContact)
                {
                    Object.Destroy(this.gameObject);
                }

            }

            else if (col.gameObject.CompareTag("BaseMinion"))
            {
                if (col.gameObject.layer == 8)
                {
                    GameObject target = col.gameObject.GetComponent<GetParents>().getParent();
                    target.gameObject.GetComponent<LeftMinionBehaviour>().TakeDamage(-damage);
                    if (target.gameObject.GetComponent<LeftMinionBehaviour>().getHealth() <= 0)
                    {
                        owner.gameObject.GetComponent<CharacterGold>().varyGold(5);
                    }
                }

                if (col.gameObject.layer == 9)
                {
                    GameObject target = col.gameObject.GetComponent<GetParents>().getParent();
                    target.gameObject.GetComponent<LeftMinionBehaviour>().TakeDamage(-damage);
                    if (target.gameObject.GetComponent<LeftMinionBehaviour>().getHealth() <= 0)
                    {
                        owner.gameObject.GetComponent<CharacterGold>().varyGold(5);
                    }
                }

                //                col.transform.parent.gameObject.GetComponent<TeamCoalitionLeftMinionBehaviour>().TakeDamage(-damage);
                //                col.transform.parent.gameObject.GetComponent<TeamCoalitionLeftMinionBehaviour>().Murderer = owner;

                //                col.gameObject.GetComponent<TeamConsortiumLeftMinionBehaviour>().TakeDamage(-damage);
                //                col.transform.parent.gameObject.GetComponent<TeamConsortiumLeftMinionBehaviour>().Murderer = owner;
                //  Debug.Log("health: baseMinion hit by " + m_Rigidbody);

                if (destroyOnContact)
                {
                    Object.Destroy(this.gameObject);

                }

            }

            else if (col.gameObject.CompareTag("MegaBoss"))
            {
                col.transform.parent.gameObject.GetComponent<MegaBossBehaviour>().TakeDamage(-damage);
                //  Debug.Log("health: baseMinion hit by " + m_Rigidbody);

                if (destroyOnContact)
                {
                    Object.Destroy(this.gameObject);

                }

            }

            else if (col.gameObject.CompareTag("Other"))
            {
                if (destroyOnContact)
                {
                    Object.Destroy(this.gameObject);

                }
            }

        }

        //command to set owner of projectile
        public void CMD_setOwnerProjectile(GameObject player)
        {
            owner = player;
        }

        void Update()
        {
            //rotation movement
            transform.Rotate(1.0f * x_rotaspeed, 1.0f * y_rotaspeed, 1.0f * z_rotaspeed);

            //basic forward movement
            transform.Translate(Vector3.forward * m_Speed * Time.deltaTime);

            //adjust acceleration rate per frame
            if (isAccelerating)
            {
                //stop accelerating once hit speed limit
                if (hasSpeedLimit && (m_Speed <= speedLimit))
                {
                    m_Speed += accelerationRate;
                }

                else
                {
                    m_Speed += accelerationRate;
                }

            }

            //stop deccelerating once hit speed limit
            if (isDeccelerating && (m_Speed >= speedLimit))
            {
                m_Speed -= accelerationRate;

            }

            //adjusting acceleration ratess 
            if (accelMultiplierOn)
            {
                if (m_Speed <= speedLimit) return;
                accelerationRate *= accelMultiplier;
            }


        }
    }
}
