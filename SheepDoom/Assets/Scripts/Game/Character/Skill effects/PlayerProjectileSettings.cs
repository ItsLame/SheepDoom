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
        public GameObject owner;
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
        public float m_Speed = 10f;   // default speed of projectile
        public float m_Lifespan = 3f; // Lifespan per second
        private float m_StartTime = 0f;
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

        //method to set direction of projectile
        public void setDirection(string direction)
        {
            if (direction == "left")
                isMovingLeft = true;
            else if (direction == "right")
                isMovingRight = true;
            else if (direction == "down")
                isMovingDown = true;
        }

        public override void OnStartServer()
        {
            ownerTeamID = owner.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
        }

        [ServerCallback]
        void OnTriggerEnter(Collider col)
        {
            //if hit player
            if (col.gameObject.CompareTag("Player"))
            {
                if (ownerTeamID == 1 && col.gameObject.GetComponent<PlayerAdmin>().getTeamIndex() == 1) return;
                if (ownerTeamID == 2 && col.gameObject.GetComponent<PlayerAdmin>().getTeamIndex() == 2) return;

                //dont hurt the owner of the projectile, dont increase score if hitting dead player
                if (col.gameObject != owner && !col.gameObject.GetComponent<PlayerHealth>().isPlayerDead())
                {
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
                        owner.GetComponent<PlayerAdmin>().IncreaseCount(false, true, false);
                        col.gameObject.GetComponent<GameEvent>().whoKilled = owner.gameObject.GetComponent<PlayerAdmin>().P_playerName;

                        //increase gold
                        owner.gameObject.GetComponent<CharacterGold>().ServerVaryGold(10);
                    }

                    if (destroyOnContact)
                        Destroyy();
                }
            }
            else if (col.gameObject.CompareTag("BaseMinion"))
            {
                if (ownerTeamID == 2)
                {
                    if (col.gameObject.layer == 8)
                    {
                        GameObject target = col.gameObject.GetComponent<GetParents>().getParent();
                        target.gameObject.GetComponent<LeftMinionBehaviour>().TakeDamage(-damage);
                        if (target.gameObject.GetComponent<LeftMinionBehaviour>().getHealth() <= 0)
                            owner.gameObject.GetComponent<CharacterGold>().ServerVaryGold(2);

                        if (destroyOnContact)
                            Destroyy();
                    }
                }
                else if (ownerTeamID == 1)
                {
                    if (col.gameObject.layer == 9)

                    {
                        GameObject target = col.gameObject.GetComponent<GetParents>().getParent();
                        target.gameObject.GetComponent<LeftMinionBehaviour>().TakeDamage(-damage);
                        if (target.gameObject.GetComponent<LeftMinionBehaviour>().getHealth() <= 0)
                            owner.gameObject.GetComponent<CharacterGold>().ServerVaryGold(2);

                        if (destroyOnContact)
                            Destroyy();
                    }
                }
            }
            else if (col.gameObject.CompareTag("MegaBoss"))
            {
                GameObject bossParent = col.gameObject.GetComponent<GetParents>().getParent();
                bossParent.GetComponent<MegaBossNewScript>().TakeDamage(-damage);
                bossParent.GetComponent<MegaBossNewScript>().setKillerTeamID(ownerTeamID);

                //announce killing of big boss
                if (bossParent.GetComponent<MegaBossNewScript>().getHealth() <= 0)
                {
                    string defeater = owner.gameObject.GetComponent<PlayerAdmin>().P_playerName;
                    bossParent.GetComponent<GameEvent>().AnnounceBossDeath(defeater, ownerTeamID);
                }

                if (destroyOnContact)
                    Destroyy();

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

        [Server]
        public void SetOwnerProjectile(GameObject player)
        {
            owner = player;
        }

        void Update()
        {
            if (isClient && hasAuthority)
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
            }

            if (isServer)
            {
                m_StartTime += Time.deltaTime;
                if (m_StartTime >= m_Lifespan)
                    Destroyy();
            }
        }
    }
}
