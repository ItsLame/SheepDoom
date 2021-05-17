using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class MegaBossProjectileScript : NetworkBehaviour
    {
        [Header("Owner of the projectile")]
        [SerializeField] private GameObject owner;

        [Header("Target Properties")]
        [Space(15)]
        [SerializeField] private bool detonates;
        [SerializeField] private bool hasTarget;
        private bool targetted;
        [SerializeField] private GameObject targetObject;

        [Header("Projectile Properties")]
        [Space(15)]
        [SerializeField] private float damage;
        [SerializeField] private float m_Speed; // default speed of projectile
        [SerializeField] private float m_Lifespan; // Lifespan per second
        [SerializeField] private float m_startTime; // to use in game
        [SerializeField] private bool startPosDone;
        [SerializeField] private Vector3 startPos;
        [SerializeField] private bool destroyOnContact;
        [SerializeField] private Rigidbody m_Rigidbody;

        [Header("Detonate timer settings")]
        [Space(15)]
        [SerializeField] private float detonateTimer;
        [SerializeField] [SyncVar] public float detonateTimerInGame;

        public override void OnStartServer()
        {
            detonateTimerInGame = detonateTimer;
        }

        //give the object its target
        public void setTarget(GameObject target)
        {
 //           hasTarget = true;
            targetObject = target;
        }

        //set detonate to true
        public void setDetonate()
        {
            detonates = true;
        }

        // SET IF HOMING TO TRUE DONT BE CONFUSED SRY
        public void setHoming()
        {
            hasTarget = true;
        }
        
        public void setOwner(GameObject firer)
        {
            owner = firer;
        }

        [ServerCallback]
        void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag("Player") && !col.GetComponent<PlayerHealth>().isPlayerDead())
            {

                col.GetComponent<PlayerHealth>().modifyinghealth(-damage);


                if (col.GetComponent<PlayerHealth>().getHealth() <= 0)
                {
                    col.gameObject.GetComponent<GameEvent>().isBoss = true;
                    col.GetComponent<PlayerHealth>().SetPlayerDead();
                }

                if (!destroyOnContact)
                    Destroyy();
            }

            if (col.CompareTag("Other"))
            {
                if (!destroyOnContact)
                    Destroyy();
            }

            if (col.CompareTag("Shield"))
            {
                if (!destroyOnContact)
                    Destroyy();
            }


            if (col.CompareTag("TeamConsortiumRangeCreep"))
            {
                col.transform.parent.GetComponent<LeftMinionBehaviour>().TakeDamage(-damage);
                Destroyy();
            }

            if (col.CompareTag("TeamCoalitionRangeCreep"))
            {
                col.transform.parent.GetComponent<LeftMinionBehaviour>().TakeDamage(-damage);
                Destroyy();
            }



        }
        private void Destroyy()
        {
            NetworkServer.Destroy(gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            //basic forward movement
            if (isServer)
            {
                //linear movement
                if (!hasTarget && !detonates)
                {
                    //then go straight
                    m_startTime += Time.deltaTime;
                    transform.Translate(Vector3.forward * m_Speed * Time.deltaTime);

                    //lifespan up
                    if (m_startTime > m_Lifespan)
                    {
                        Destroyy();
                    }

                }

                //targetted movement
                if (hasTarget)
                {
                    m_startTime += Time.deltaTime;

                    //start fast end slow effect
                    transform.position = Vector3.Lerp(transform.position, targetObject.transform.position, 0.03f);

                    //lifespan up
                    if (m_startTime > m_Lifespan)
                    {
                        Destroyy();
                    }
                }

                //detonate after x time
                if (detonates && !hasTarget)
                {
                    //start counting
                    detonateTimer -= Time.deltaTime;

                    //activate collider once times up to 'explode'
                    if (detonateTimer <= 0)
                    {
                        this.gameObject.GetComponent<SphereCollider>().enabled = true;
                        Invoke("Destroyy", 1f);
                    }

                }

                //follows then detonates
                if (detonates && hasTarget)
                {
                    //start counting
                    if (detonateTimerInGame >= 0)
                    {
                        detonateTimerInGame -= Time.deltaTime;
                    }


                    //start following until 3s left
                    if (detonateTimerInGame >= 3)
                    {
                        transform.LookAt(targetObject.transform);
                        transform.Translate(Vector3.forward * m_Speed / 2 * Time.deltaTime);
                    }

                    //activate collider once times up to 'explode'
                    if (detonateTimerInGame <= 0)
                    {
                        this.gameObject.GetComponent<SphereCollider>().enabled = true;
                        Invoke("Destroyy", 1f);
                    }
                }
            }
        }
    }
}