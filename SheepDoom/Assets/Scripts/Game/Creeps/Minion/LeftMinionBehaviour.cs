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
        [SerializeField] private float WaypointsLength;
        [SerializeField] private int currentPoint = 0;
        [SerializeField] private Vector3 target;
        [SerializeField] private Vector3 direction;
        [SerializeField] public float DirectionMagnitude;
        [Space(15)]

        public NavMeshAgent agent;
        [Space(15)]
        public bool ismeleeattack = false;

        //Aggro
        private Vector3 wayPointPos;
        [SerializeField]
        private float CreepMoveSpeed;

        //Attacking
        public float timeBetweenAttacks;
        private bool alreadyattacked;

        //states 
        public float sightRange, attackRange;
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
        private GameObject firedProjectile = null;

        public event Action<float> OnHealthPctChanged = delegate { };

        void HealthBarUpdate(float oldValue, float newValue)
        {
            float currenthealthPct = newValue / maxHealth;
            OnHealthPctChanged(currenthealthPct);
        }

        public override void OnStartServer()
        {
            currenthealth = maxHealth;
            currentPoint = StartIndex;
            StartMovingToWayPoint();
            agent.autoBraking = false;
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            //dont bother with rest if locked on
            if (isLockedOn) return;

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

        [ServerCallback]
        private void OnTriggerStay(Collider other)
        {
            // don't bother with others thats not locked on
            if (other.gameObject != targetObject) return;

            // follow only the target if target still in range
            if (isLockedOn && other.gameObject == targetObject)
            {
                //attack player in range
                if (Vector3.Distance(gameObject.transform.position, other.gameObject.transform.position) < attackRange)
                    RangedAttackPlayer();

                //else chase in range
                else
                {
                    agent.autoBraking = false;
                    agent.SetDestination(targetObject.transform.position);
                }
            }
        }

        [ServerCallback]
        private void OnTriggerExit(Collider other) 
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
            agent.SetDestination(transform.position);
            agent.autoBraking = true;

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
                firedProjectile = Instantiate(projectile, transform);
                firedProjectile.GetComponent<RangedCreepProjectilesettings>().setOwner(gameObject);
                firedProjectile.transform.SetParent(null, false);
                firedProjectile.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
                NetworkServer.Spawn(firedProjectile);

                alreadyattacked = true;
            }
            else if (gameObject.CompareTag("TeamConsortiumRangeCreep"))
            {
                firedProjectile = Instantiate(projectile, transform);
                firedProjectile.GetComponent<RangedCreepProjectilesettings>().setOwner(gameObject);
                firedProjectile.transform.SetParent(null, false);
                firedProjectile.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
                NetworkServer.Spawn(firedProjectile);

                alreadyattacked = true;
            }
        }

        [Server]
        private void ResetAttack()
        {
            alreadyattacked = false;
        }

        [Server]
        void StartMovingToWayPoint() 
        {
            WaypointsLength = waypoints.Length;
            if (currentPoint < waypoints.Length)
            {
                target = waypoints[currentPoint].position; 
                direction = target - transform.position;
                DirectionMagnitude = direction.magnitude;
                if (direction.magnitude < 150)
                {
                    currentPoint += 1;
                }

                // for now end minion life if reach the end
                if (currentPoint == waypoints.Length)
                    Destroyy();
            }

    //        transform.LookAt(target);
            agent.SetDestination(target);
            agent.speed = CreepMoveSpeed;
        }

        [Server]
        public void TakeDamage(float damage)
        {
            currenthealth += damage;
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