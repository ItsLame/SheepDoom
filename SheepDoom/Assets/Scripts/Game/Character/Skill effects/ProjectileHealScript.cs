using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    [RequireComponent(typeof(Rigidbody))]
    public class ProjectileHealScript : NetworkBehaviour
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
        [SerializeField] private bool isMovingDown;
        [SerializeField] private bool isMovingLeft;
        [SerializeField] private bool isMovingRight;

        [Header("Damage/Healing Properties")]
        [Space(15)]
        public bool healsAllies;
        public bool healsSelf;
        public int damage;
        public bool destroyOnContact; //if projectile ill stop on first contact
        [SerializeField]
        private Rigidbody m_Rigidbody;


        [Header("Healing AOE / object)")]
        [Space(15)]
        public GameObject HealingRadiusObject;
        [SerializeField] private bool hasHealingRadiusObject;

        [Header("Bullet Properties")]
        [Space(15)]
        public float m_Speed; // default speed of projectile
        public float m_Lifespan = 3f; // Lifespan per second
        [SerializeField] private float durationBeforeDestroy;
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

        public override void OnStartServer()
        {
            ownerTeamID = owner.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
            if(HealingRadiusObject != null) HealingRadiusObject.gameObject.GetComponent<HealActivateScript>().setTeamID(ownerTeamID);
        }

        [ServerCallback]
        void OnTriggerEnter(Collider col)
        {
            //if hit player
            if (col.gameObject.CompareTag("Player"))
            {
                if (healsSelf)
                {
                    if (col.gameObject == owner)
                    {
                        col.gameObject.GetComponent<PlayerHealth>().modifyinghealth(damage);
                    }
                }

                //dont hurt the owner of the projectile, dont increase score if hitting dead player
                if (col.gameObject != owner && !col.gameObject.GetComponent<PlayerHealth>().isPlayerDead())
                {
                    //if heals allies, heals first ally on touch
                    if (healsAllies)
                    {
                        if (col.gameObject.GetComponent<PlayerAdmin>().getTeamIndex() == ownerTeamID)
                        {
                            col.gameObject.GetComponent<PlayerHealth>().modifyinghealth(damage);
                        }
                    }

                    //normal damaging
                    else
                    {
                        if (col.gameObject != owner) return;

                        //reduce HP of hit target
                        col.gameObject.GetComponent<PlayerHealth>().modifyinghealth(-damage);

                        //increase killer's kill count if target is killed
                        if (col.gameObject.GetComponent<PlayerHealth>().getHealth() <= 0)
                        {
                            col.gameObject.GetComponent<PlayerHealth>().SetPlayerDead();
                            owner.GetComponent<PlayerAdmin>().IncreaseCount(false, true, false);
                            col.gameObject.GetComponent<GameEvent>().whoKilled = owner.gameObject.GetComponent<PlayerObj>().GetPlayerName();
                        }
                    }

                    //if healing area is set by another object activate it 
                    if (hasHealingRadiusObject)
                    {
                        HealingRadiusObject.gameObject.GetComponent<HealActivateScript>().activateHeal();
                    }


                    if (destroyOnContact)
                        Invoke("Destroyy", durationBeforeDestroy);
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
                            owner.gameObject.GetComponent<CharacterGold>().CmdVaryGold(5);

                        //if healing area is set by another object activate it 
                        if (hasHealingRadiusObject)
                        {
                            HealingRadiusObject.gameObject.GetComponent<HealActivateScript>().activateHeal();
                        }

                        if (destroyOnContact)
                            Invoke("Destroyy", durationBeforeDestroy);
                    }
                }
                else if (ownerTeamID == 1)
                {
                    if (col.gameObject.layer == 9)

                    {
                        GameObject target = col.gameObject.GetComponent<GetParents>().getParent();
                        target.gameObject.GetComponent<LeftMinionBehaviour>().TakeDamage(-damage);
                        if (target.gameObject.GetComponent<LeftMinionBehaviour>().getHealth() <= 0)
                            owner.gameObject.GetComponent<CharacterGold>().CmdVaryGold(5);

                        //if healing area is set by another object activate it 
                        if (hasHealingRadiusObject)
                        {
                            HealingRadiusObject.gameObject.GetComponent<HealActivateScript>().activateHeal();
                        }

                        if (destroyOnContact)
                            Invoke("Destroyy", durationBeforeDestroy);
                    }
                }
            }
            else if (col.gameObject.CompareTag("MegaBoss"))
            {
                col.GetComponent<GetParents>().getParent().GetComponent<MegaBossNewScript>().TakeDamage(-damage);

                //if healing area is set by another object activate it 
                if (hasHealingRadiusObject)
                {
                    HealingRadiusObject.gameObject.GetComponent<HealActivateScript>().activateHeal();
                }

                if (destroyOnContact)
                    Invoke("Destroyy", durationBeforeDestroy);
            }
            else if (col.gameObject.CompareTag("Other"))
            {
     //           Debug.Log(gameObject.name + "touched other" + col.gameObject.name); ;
                if (destroyOnContact)
                    Invoke("Destroyy", durationBeforeDestroy);
            }
        }

        [Server]
        public void Destroyy()
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

