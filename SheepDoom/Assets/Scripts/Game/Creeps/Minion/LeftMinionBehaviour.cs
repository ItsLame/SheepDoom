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
        Animator charAnim;

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
        public bool playerInSightRange, playerInAttackRange;

        public bool TeamCoalition;

        //layermask
        public LayerMask whatisplayer;
        [Space(15)]
        //Ranged Projectile
        public GameObject projectile;
        [Space(15)]

        //animator
        public Animator animator;
        [Space(15)]
        //melee attackpoint
        public Transform meleeattackPoint;
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
        // public bool isplayer = false;

        public event Action<float> OnHealthPctChanged = delegate { };

        void HealthBarUpdate(float oldValue, float newValue)
        {
            float currenthealthPct = newValue / maxHealth;
            OnHealthPctChanged(currenthealthPct);
        }

        void Start()
        {
            charAnim = GetComponent<Animator>();
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
            if(gameObject.CompareTag("TeamCoalitionRangeCreep") && !isLockedOn)
            {
                if (other.CompareTag("TeamConsortiumRangeCreep"))
                {
                    targetObject = other.gameObject;
                    isLockedOn = true;
                }
                else if (other.CompareTag("Player") && (other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex() == 2) && !other.GetComponent<PlayerHealth>().isPlayerDead())
                {
                    targetObject = other.gameObject;
                    isLockedOn = true;
                }
            }
            else if(gameObject.CompareTag("TeamConsortiumRangeCreep") && !isLockedOn)
            {
                if(other.CompareTag("TeamCoalitionRangeCreep"))
                {
                    targetObject = other.gameObject;
                    isLockedOn = true;
                }
                else if (other.CompareTag("Player") && (other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex() == 1) && !other.GetComponent<PlayerHealth>().isPlayerDead())
                {
                    targetObject = other.gameObject;
                    isLockedOn = true;
                }
            }
        }

        [Server]
        private void OnTriggerExit(Collider other) // exit happened before playerinsight range happened
        {
            if(gameObject.CompareTag("TeamCoalitionRangeCreep"))
            {
                if (other.CompareTag("Player") && other.gameObject.layer == 9)
                    goBackToTravelling();
            }
            else if(gameObject.CompareTag("TeamConsortiumRangeCreep"))
            {
                if (other.CompareTag("Player") && other.gameObject.layer == 8)
                    goBackToTravelling();
            }
        }

        void Update()
        {
            if (isServer)
            {
                // dist = Vector3.Distance(playerTransf.position, transform.position);
                //Check if Player in sightrange
                playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatisplayer);

                //Check if Player in attackrange
                playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatisplayer);

                /*if (target == null)
                {
                    targetObject = null;
                    isLockedOn = false;
                    StartMovingToWayPoint();
                    return;
                }*/

                if(targetObject == null)
                {
                    isLockedOn = false;
                    goBackToTravelling();
                    return;
                }

                /*if (!playerInSightRange && !playerInAttackRange && !isLockedOn)
                    goBackToTravelling();*/
                    

                if (playerInSightRange && !playerInAttackRange && isLockedOn)                
                    ChasePlayer();

                if (playerInAttackRange && playerInSightRange && !ismeleeattack)
                    RangedAttackPlayer();

                if (playerInAttackRange && playerInSightRange && ismeleeattack)
                    MeleeAttackPlayer();

                if (currenthealth <= 0)
                    Destroyy();
            }
        }

        [Server]
        void ChasePlayer()
        {
            if (targetObject != null)
            {
                agent.autoBraking = false;
                agent.SetDestination(targetObject.transform.position);
            }
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

        void MeleeAttackPlayer()
        {
            //Make sure enemy doesn't move
            agent.SetDestination(transform.position);

            transform.LookAt(targetObject.transform);

            if (!alreadyattacked)
            {
                //Melee attack for dog
                transform.LookAt(targetObject.transform);
                /*animator.SetTrigger("Attack");
                animator.SetTrigger("AttackToIdle");*/
                Collider[] hitenemies = Physics.OverlapSphere(meleeattackPoint.position, attackRange, enemyLayers);

                foreach (Collider enemy in hitenemies)
                {
                    if (gameObject.CompareTag("TeamCoalitionRangeCreep"))
                    {
                        if (enemy.CompareTag("Player") && (enemy.gameObject.GetComponent<PlayerAdmin>().getTeamIndex() == 2))
                        {
                            enemy.GetComponent<PlayerHealth>().modifyinghealth(-meleedamage);
                        }
                        if (enemy.CompareTag("BaseMinion") && enemy.gameObject.layer == 9)
                        {
                            enemy.transform.parent.GetComponent<LeftMinionBehaviour>().TakeDamage(-meleedamage);
                        }
                    }
                    if (gameObject.CompareTag("TeamConsortiumRangeCreep"))
                    {
                        if (enemy.CompareTag("Player") && (enemy.gameObject.GetComponent<PlayerAdmin>().getTeamIndex() == 1))
                        {
                            enemy.GetComponent<PlayerHealth>().modifyinghealth(-meleedamage);
                        }
                        if (enemy.CompareTag("BaseMinion") && enemy.gameObject.layer == 8)
                        {
                            enemy.transform.parent.GetComponent<LeftMinionBehaviour>().TakeDamage(-meleedamage);
                        }
                    }
                    if (enemy.GetComponent<PlayerHealth>().getHealth() <= 0)
                    {
                        enemy.GetComponent<PlayerHealth>().SetPlayerDead();
                        enemy.gameObject.GetComponent<GameEvent>().isMinion = true;
                        goBackToTravelling();
                    }
                }


                Debug.Log("MeleeAttack!");
                alreadyattacked = true;
                Invoke("ResetAttack", timeBetweenAttacks);
            }
        }
   

        [Server]
        private void RangedAttackPlayer()
        {
            if (targetObject == null)
                goBackToTravelling();
            else //Make sure enemy doesn't move
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
        }

        [Server]
        private void FireProjectile()
        {
            if(gameObject.CompareTag("TeamCoalitionRangeCreep"))
            {
                //GameObject FiredProjectile = Instantiate(projectile, this.transform.position, this.transform.rotation);
                //FiredProjectile.GetComponent<RangedCreepProjectilesettings>().setOwner(gameObject);
                GameObject FiredProjectile = Instantiate(projectile, transform);
                FiredProjectile.GetComponent<RangedCreepProjectilesettings>().setOwner(gameObject);
                FiredProjectile.transform.SetParent(null, false);
                FiredProjectile.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
                NetworkServer.Spawn(FiredProjectile);
                alreadyattacked = true;
            }
            else if(gameObject.CompareTag("TeamConsortiumRangeCreep"))
            {
                GameObject FiredProjectile = Instantiate(projectile, transform);
                FiredProjectile.GetComponent<RangedCreepProjectilesettings>().setOwner(gameObject);
                FiredProjectile.transform.SetParent(null, false);
                FiredProjectile.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
                //GameObject FiredProjectile = Instantiate(projectile, this.transform.position, this.transform.rotation);
                //FiredProjectile.GetComponent<RangedCreepProjectilesettings>().setOwner(gameObject);
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
                if (direction.magnitude < 10)
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, sightRange);
        }

        public float getHealth()
        {
            return currenthealth;
        }
    }
}