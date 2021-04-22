using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using Mirror;

namespace SheepDoom
{
    public class TeamCoalitionLeftMinionBehaviour : NetworkBehaviour
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
        private float speed = 15.0f;
        private Vector3 wayPointPos;
        // public float howclose;
        // private float dist;
        public float CreepMoveSpeed = 2.0f;

        //Attacking
        [SyncVar] public float timeBetweenAttacks;
        [SyncVar] bool alreadyattacked;

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
        public Transform attackPoint;
        public LayerMask enemyLayers;
        public int meleedamage = 50;
        [Space(15)]
        //Health
        [SerializeField]
        [SyncVar]
        public float maxHealth;
        
        [SyncVar] public float currenthealth;
        public float goldValue;

        [Space(15)]
        public GameObject targetObject;
        public GameObject Murderer;
        private Transform targetTransf;
        [SyncVar] private bool isLockedOn = false;
        // public bool isplayer = false;

        public event Action<float> OnHealthPctChanged = delegate { };

        void Start()
        {
            targetObject = null;
            targetTransf = null;
            isLockedOn = false;
            charAnim = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();

            currenthealth = maxHealth;
            currentPoint = StartIndex;
            StartMovingToWayPoint();
            agent.autoBraking = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "TeamConsortiumRangeCreep")
            {
                if (!isLockedOn)
                {
                    targetObject = other.gameObject;
                    targetTransf = targetObject.transform;
                    isLockedOn = true;
                }
            }

            else if (other.gameObject.layer == 9 && other.gameObject.CompareTag("Player") && !other.gameObject.GetComponent<PlayerHealth>().isPlayerDead())
            {
                if (!isLockedOn)
                {
                    targetObject = other.gameObject;
                    targetTransf = targetObject.transform;
                    isLockedOn = true;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && other.gameObject.layer == 9)
            {
                goBackToTravelling();
            }
        }

        //go back to patrol when player is dead
        public void goBackToTravelling()
        {
            isLockedOn = false;
            targetObject = null;
            targetTransf = null;
            agent.autoBraking = false;
            StartMovingToWayPoint();
        }

        void Update()
        {
            // dist = Vector3.Distance(playerTransf.position, transform.position);
            //Check if Player in sightrange
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatisplayer);

            //Check if Player in attackrange
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatisplayer);

            if (target == null)
            {
                targetObject = null;
                targetTransf = null;
                isLockedOn = false;
                StartMovingToWayPoint();
                return;
            }

            if (!playerInSightRange && !playerInAttackRange && !isLockedOn)
            {
                goBackToTravelling();
            }

            if (playerInSightRange && !playerInAttackRange)
            {
                agent.autoBraking = false;
                ChasePlayer();
            }

            if (playerInAttackRange && playerInSightRange && ismeleeattack == false)
            {
                RangedAttackPlayer();
            }
            if (playerInAttackRange && playerInSightRange && ismeleeattack == true)
            {
                MeleeAttackPlayer();
            }

            if (currenthealth <= 0)
            {
                // Debug.Log(this.gameObject.name + "has died");

                Destroyy();

            }

        }
        void ChasePlayer()
        {
            if (targetTransf != null || targetObject != null)
            {
                agent.SetDestination(targetTransf.position);
            }

        }
        void MeleeAttackPlayer()
        {
            //Make sure enemy doesn't move
            agent.SetDestination(transform.position);

            transform.LookAt(targetTransf);

            if (!alreadyattacked)
            {
                //Meele attack for dog
                transform.LookAt(targetTransf);
                animator.SetTrigger("Attack");
                animator.SetTrigger("AttackToIdle");
                Collider[] hitenmies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

                foreach (Collider enemy in hitenmies)
                {
                    enemy.GetComponent<PlayerHealth>().modifyinghealth(-meleedamage);

                }
                Debug.Log("MeleeAttack!");
                alreadyattacked = true;
                Invoke("ResetAttack", timeBetweenAttacks);
            }
        }

        void RangedAttackPlayer()
        {
            //Make sure enemy doesn't move
            agent.SetDestination(transform.position);
            agent.autoBraking = true;
            transform.LookAt(targetTransf);

            if (!alreadyattacked)
            {
                transform.LookAt(targetTransf);
                //Attack
                FireProjectile();

                Invoke("ResetAttack", timeBetweenAttacks);
            }
        }

        [Server]
        public void FireProjectile()
        {
            GameObject FiredProjectile = Instantiate(projectile, this.transform.position, this.transform.rotation);
            FiredProjectile.gameObject.GetComponent<TeamCoalitionCreepProjectilesettings>().setOwner(this.gameObject);
            NetworkServer.Spawn(FiredProjectile);
            alreadyattacked = true;
        }

        [Server]
        void ResetAttack()
        {
            alreadyattacked = false;
        }

        void StartMovingToWayPoint()
        {
            if (currentPoint < waypoints.Length)
            {
                target = waypoints[currentPoint].position;
                direction = target - transform.position;
                if (direction.magnitude < 5)
                    currentPoint++;
            }
            else if (currentPoint == waypoints.Length)
            {
                agent.autoBraking = true;
            }
            transform.LookAt(target);
            agent.SetDestination(target);
            agent.speed = CreepMoveSpeed;

        }
        public void TakeDamage(float damage)
        {
            if (isServer)
            {
                currenthealth += damage;
  //              RpcUpdateMinionHp();
                float currenthealthPct = (float)currenthealth / (float)maxHealth;
                OnHealthPctChanged(currenthealthPct);
            }
        }

        /*
        [ClientRpc]
        void RpcUpdateMinionHp()
        {
            StartCoroutine(WaitForUpdate(currenthealth));
        }

        private IEnumerator WaitForUpdate(float _oldcurrenthealth)
        {
            while (currenthealth == _oldcurrenthealth)
            {
                yield return null;
            }
        }*/

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