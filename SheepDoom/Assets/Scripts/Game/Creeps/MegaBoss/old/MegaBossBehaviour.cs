using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using Mirror;

namespace SheepDoom
{

    public class MegaBossBehaviour : NetworkBehaviour
    {
        [Space(15)]
        // Waypoint System
        public Transform[] waypoints;
        public int StartIndex = 0;
        private int currentPoint = 0;
        private Vector3 target;
        private Vector3 direction;
        [Space(15)]

        private NavMeshAgent agent;
        [Space(15)]
        public bool ismeleeattack = false;
        Animator charAnim;

        //Aggro
        //private float speed = 15.0f;
        private Vector3 wayPointPos;
        // public float howclose;
        // private float dist;
        public float CreepMoveSpeed = 2.0f;

        //Attacking
        public float timeBetweenAttacks;
        bool alreadyattacked;

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
        private int maxHealth = 50;
        private int currenthealth;
        public float goldValue;

        [Space(15)]
        public GameObject targetObject;
        private bool isLockedOn = false;
        // public bool isplayer = false;

        //MegaBossState
        private bool passivestate = true;



        public event Action<float> OnHealthPctChanged = delegate { };

        void Start()
        {

            targetObject = null;
            isLockedOn = false;
            charAnim = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();

            currenthealth = maxHealth;
            currentPoint = StartIndex;

            StartCoroutine(TimeUntilAggressivemode());

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 8 && other.CompareTag("Player"))
            {
                if (!isLockedOn)
                {
                    targetObject = other.gameObject;
                    isLockedOn = true;
                }

            }
            else if (other.gameObject.layer == 9 && other.CompareTag("Player"))
            {
                if (!isLockedOn)
                {
                    targetObject = other.gameObject;
                    isLockedOn = true;
                }

            }
            else if (other.gameObject.tag == "TeamCoalitionRangeCreep")
            {
                if (!isLockedOn)
                {
                    targetObject = other.gameObject;
                    isLockedOn = true;
                }
            }
            else if (other.gameObject.tag == "TeamConsortiumRangeCreep")
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
            if (other.CompareTag("Player"))
            {
                // Debug.Log("Player " + other.gameObject.name + " has left minion zone");
                targetObject = null;
            }
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
                agent.autoBraking = false;
                targetObject = null;
                isLockedOn = false;
                StartMovingToWayPoint();
                return;
            }


            if (!playerInSightRange && !playerInAttackRange)
            {
                StartMovingToWayPoint();
                isLockedOn = false;
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
            if (targetObject != null)
            {
                agent.SetDestination(targetObject.transform.position);
            }


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
            //Make sure enemy doesn't move
            agent.SetDestination(transform.position);

            transform.LookAt(targetObject.transform);

            if (!alreadyattacked)
            {
                transform.LookAt(targetObject.transform);
                //Attack
                //Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
                Rigidbody rb = Instantiate(projectile, transform).GetComponent<Rigidbody>();

                rb.transform.SetParent(null, false);
                rb.transform.SetPositionAndRotation(this.transform.position, Quaternion.identity);

                rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
                //    rb.AddForce(transform.up * 8, ForceMode.Impulse);

                alreadyattacked = true;
                Invoke("ResetAttack", timeBetweenAttacks);
            }
        }

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
                if (direction.magnitude < 5 && passivestate == false)
                {

                    currentPoint++;
                }
                else
                {
                    agent.autoBraking = true;
                }

            }
            else if (currentPoint == waypoints.Length && passivestate == false)
            {
                currentPoint = 0;
            }
            else if (currentPoint == waypoints.Length && passivestate == true)
            {
                agent.autoBraking = true;
            }
            transform.LookAt(target);
            agent.SetDestination(target);
            agent.speed = CreepMoveSpeed;

        }
        public void TakeDamage(int damage)
        {
            currenthealth += damage;

            float currenthealthPct = (float)currenthealth / (float)maxHealth;
            OnHealthPctChanged(currenthealthPct);
        }

        private void Destroyy()
        {
            Destroy(this.gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, sightRange);
        }
        IEnumerator TimeUntilAggressivemode()
        {
            yield return new WaitForSeconds(20);
            passivestate = false;
            agent.autoBraking = false;
        }
    }


}
