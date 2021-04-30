using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using Mirror;

namespace SheepDoom
{
    public class LeftMinionBehaviour : NetworkBehaviour
    {
        [Space(15)]
        // Waypoint System
        public Transform[] waypoints;
        public int StartIndex = 0;
        private int currentPoint = 0;
        private Vector3 target;
        private Vector3 direction;
        [Space(15)]

        public NavMeshAgent agent;
        [Space(15)]
        public bool ismeleeattack = false;
        //       Animator charAnim;

        //Aggro
        //private float speed = 15.0f;
        private Vector3 wayPointPos;
        // public float howclose;
        // private float dist;
        [SerializeField]
        private float CreepMoveSpeed;

        //Attacking
        public float timeBetweenAttacks;
        private bool alreadyattacked;

        //states 
        public float sightRange, attackRange;

        //       public bool TeamCoalition;

        //layermask
        //       public LayerMask whatisplayer;
        [Space(15)]
        //Ranged Projectile
        public GameObject projectile;
        [Space(15)]

        //animator
        public Animator animator;
        [Space(15)]
        //melee attackpoint
        public Transform attackPoint;
        public LayerMask enemyLayers;
        public int meleedamage = 50;
        [Space(15)]
        //Health
        [SerializeField]
        private float maxHealth;
        [SyncVar(hook = nameof(HealthBarUpdate))] private float currenthealth;
        public float goldValue;

        [Space(15)]
        private GameObject targetObject = null;
        private bool isLockedOn = false;

        public event Action<float> OnHealthPctChanged = delegate { };

        void HealthBarUpdate(float oldValue, float newValue)
        {
            float currenthealthPct = newValue / maxHealth;
            OnHealthPctChanged(currenthealthPct);
        }

        void Start()
        {
        }

        public override void OnStartServer()
        {
            currenthealth = maxHealth;
            currentPoint = StartIndex;
            StartMovingToWayPoint();
            agent.autoBraking = false;
        }

        [Server]
        private void OnTriggerEnter(Collider other)
        {
            //if not locked on, search for new targets
            if (gameObject.CompareTag("TeamCoalitionRangeCreep") && !isLockedOn)
            {
                //atk da enemy creepu
                if (other.CompareTag("TeamConsortiumRangeCreep"))
                {
                    targetObject = other.gameObject;
                    isLockedOn = true;
                }
                //check for not dead player thats in the enemy team
                else if (other.CompareTag("Player") && (other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex() == 2) && !other.GetComponent<PlayerHealth>().isPlayerDead())
                {
                    targetObject = other.gameObject;
                    isLockedOn = true;
                }
            }
            else if (gameObject.CompareTag("TeamConsortiumRangeCreep") && !isLockedOn)
            {
                //same attack the enemy creepes
                if (other.CompareTag("TeamCoalitionRangeCreep"))
                {
                    targetObject = other.gameObject;
                    isLockedOn = true;
                }

                //enemy players r not spared
                else if (other.CompareTag("Player") && (other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex() == 1) && !other.GetComponent<PlayerHealth>().isPlayerDead())
                {
                    targetObject = other.gameObject;
                    isLockedOn = true;
                }
            }
        }

        [Server]
        private void OnTriggerStay(Collider other)
        {
            // don't bother with others thats not locked on
            if (other.gameObject != targetObject) return;

            // follow only the target if target still in range
            if (isLockedOn && other.gameObject == targetObject)
            {
                //attack player in range
                if (Vector3.Distance(gameObject.transform.position, other.gameObject.transform.position) < attackRange)
                {
                    RangedAttackPlayer();
                }

                //else chase in range
                else
                {
                    //                  transform.LookAt(targetObject.transform);
                    agent.autoBraking = false;
                    agent.SetDestination(targetObject.transform.position);
                }


            }
        }

        [Server]
        private void OnTriggerExit(Collider other) // exit happened before playerinsight range happened
        {
            //go back to travelling if target goes out of range
            if (isLockedOn && other.gameObject == targetObject)
            {
                isLockedOn = false;
                targetObject = null;
                agent.autoBraking = false;
                goBackToTravelling();
            }

        }

        void Update()
        {
            if (isServer)
            {
                //check for position relative to current waypoint
                if (currentPoint < waypoints.Length)
                {
                    target = waypoints[currentPoint].position;
                    direction = target - transform.position;
                    if (direction.magnitude < 10)
                        currentPoint++;
                }

                if (targetObject == null)
                {
                    isLockedOn = false;
                    goBackToTravelling();
                    return;
                }


                if (currenthealth <= 0)
                    Destroyy();
            }
        }

        [Server]
        void ChaseTarget()
        {
            agent.autoBraking = false;
            agent.SetDestination(targetObject.transform.position);
        }

        //go back to patrol when player is dead
        [Server]
        public void goBackToTravelling()
        {
            isLockedOn = false;
            targetObject = null;
            agent.autoBraking = false;
            StartMovingToWayPoint();
        }

        [Server]
        private void RangedAttackPlayer()
        {
            //               Debug.Log("Attacking enemy");
            agent.SetDestination(transform.position);
            agent.autoBraking = true;
            //              transform.LookAt(targetObject.transform);

            if (!alreadyattacked)
            {
                transform.LookAt(targetObject.transform);
                FireProjectile();//Attack
                Invoke("ResetAttack", timeBetweenAttacks);
            }

        }

        [Server]
        private void FireProjectile()
        {
            if (gameObject.CompareTag("TeamCoalitionRangeCreep"))
            {

                GameObject FiredProjectile = Instantiate(projectile, transform);
                FiredProjectile.GetComponent<RangedCreepProjectilesettings>().setOwner(gameObject);
                FiredProjectile.transform.SetParent(null, false);
                FiredProjectile.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
                NetworkServer.Spawn(FiredProjectile);

                alreadyattacked = true;
            }
            else if (gameObject.CompareTag("TeamConsortiumRangeCreep"))
            {
                GameObject FiredProjectile = Instantiate(projectile, transform);
                FiredProjectile.GetComponent<RangedCreepProjectilesettings>().setOwner(gameObject);
                FiredProjectile.transform.SetParent(null, false);
                FiredProjectile.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
                NetworkServer.Spawn(FiredProjectile);

                alreadyattacked = true;
            }
        }

        [Server]
        private void ResetAttack()
        {
            alreadyattacked = false;
        }

        [Server]
        void StartMovingToWayPoint() // nothing updating this
        {
            if (currentPoint < waypoints.Length)
            {
                target = waypoints[currentPoint].position;
                direction = target - transform.position;
                if (direction.magnitude < 1)
                    currentPoint++;
            }

            transform.LookAt(target);
            agent.SetDestination(target);
            agent.speed = CreepMoveSpeed;
        }

        [Server]
        public void TakeDamage(float damage)
        {
            currenthealth += damage;
            //              RpcUpdateMinionHp();
            float currenthealthPct = currenthealth / maxHealth;
            OnHealthPctChanged(currenthealthPct);
        }

        private void Destroyy()
        {
            NetworkServer.Destroy(this.gameObject);
        }

        public float getHealth()
        {
            return currenthealth;
        }
    }
}