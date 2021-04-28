using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using Mirror;

/*namespace SheepDoom
{
    public class TeamConsortiumLeftMinionBehaviour : NetworkBehaviour
    {
        [Space(15)]
        // Waypoint System
        public Transform[] waypoints;
        public int StartIndex = 0;
        private int currentPoint = 0;
        [SyncVar] private Vector3 target;
        [SyncVar] private Vector3 direction;
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
        private float maxHealth;
        [SyncVar] private float currenthealth;
        public float goldValue;

        [Space(15)]
        private GameObject targetObject = null;
        [SyncVar] private bool isLockedOn = false;
        // public bool isplayer = false;
        public event Action<float> OnHealthPctChanged = delegate { };

        void Start()
        {
            charAnim = GetComponent<Animator>();
            //agent = GetComponent<NavMeshAgent>();

            //currenthealth = maxHealth;
            //currentPoint = StartIndex;
            //StartMovingToWayPoint();
            //agent.autoBraking = false;
        }

        public override void OnStartServer()
        {
            currenthealth = maxHealth;
            currentPoint = StartIndex;
            StartMovingToWayPoint();
            agent.autoBraking = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "TeamCoalitionRangeCreep")
            {
                if (!isLockedOn)
                {
                    targetObject = other.gameObject;
                    isLockedOn = true;
                }
            }
            else if (other.CompareTag("Player") && other.gameObject.layer == 8 && !other.GetComponent<PlayerHealth>().isPlayerDead())
            {
                if (!isLockedOn)
                {
                    targetObject = other.gameObject;
                    isLockedOn = true;
                }
            }


        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && other.gameObject.layer == 8)
            {
                goBackToTravelling();
            }
        }

        //go back to patrol when player is dead
        public void goBackToTravelling()
        {
            isLockedOn = false;
            targetObject = null;
            //           agent.autoBraking = false;
            StartMovingToWayPoint();

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
                }

                if (!playerInSightRange && !playerInAttackRange && !isLockedOn)
                    goBackToTravelling();

                if (playerInSightRange && !playerInAttackRange && isLockedOn)
                {
                    agent.autoBraking = false;
                    ChasePlayer();
                }

                if (playerInAttackRange && playerInSightRange && ismeleeattack == false)
                    RangedAttackPlayer();

                if (playerInAttackRange && playerInSightRange && ismeleeattack == true)
                    MeleeAttackPlayer();

                if (currenthealth <= 0)
                    Destroyy();
            }

        }

        void ChasePlayer()
        {
            if (targetObject != null)
                agent.SetDestination(targetObject.transform.position);
        }

        void MeleeAttackPlayer()
        {
            //Make sure enemy doesn't move
            agent.SetDestination(transform.position);

            transform.LookAt(targetObject.transform);

            if (!alreadyattacked)
            {
                //Meele attack for dog
                transform.LookAt(targetObject.transform);
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
            if (!targetObject)
            {
                goBackToTravelling();
            }

            else
            {
                //Make sure enemy doesn't move
                agent.SetDestination(transform.position);
                agent.autoBraking = true;
                transform.LookAt(targetObject.transform);

                if (!alreadyattacked)
                {
                    transform.LookAt(targetObject.transform);
                    //Attack
                    FireProjectile();

                    Invoke("ResetAttack", timeBetweenAttacks);
                }
            }

        }

        [Server]
        public void FireProjectile()
        {
            GameObject FiredProjectile = Instantiate(projectile, this.transform.position, this.transform.rotation);
            FiredProjectile.gameObject.GetComponent<TeamConsortiumprojectilesettings>().setOwner(this.gameObject);
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
   //             RpcUpdateMinionHp();
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
}*/