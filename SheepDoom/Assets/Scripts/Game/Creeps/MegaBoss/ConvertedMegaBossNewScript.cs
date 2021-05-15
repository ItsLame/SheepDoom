using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using Mirror;

namespace SheepDoom
{
    public class ConvertedMegaBossNewScript : NetworkBehaviour
    {
        [Header("Boss TeamID")]
        [SerializeField] private float teamID;

        [Header("NavMeshAgent to attach")]
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private GameObject redBaseLocation;
        [SerializeField] private GameObject blueBaseLocation;
        [SerializeField] private GameObject targetLocation = null;

        [Header("Attack Cooldowns")]
        [Space(15)]
        //Attacking
        [SerializeField] private float cooldown1;

        [Space(15)]
        [SerializeField] private bool alreadyattacked;
        [SerializeField] private float attackRange;

        [Header("Attacking Patterns")]
        [Space(15)]
        [SerializeField] private bool FirstAttack;


        [Header("Projectiles to fire")]
        [Space(15)]
        //Ranged Projectile
        [SerializeField] private GameObject projectile;
        private GameObject firedProjectile = null;


        [Header("Health variable setting")]
        [Space(15)]
        [SerializeField] private float maxHealth;
        [SyncVar(hook = nameof(HealthBarUpdate))] [SerializeField] private float currenthealth;
        public float goldValue;

        [Header("Targetting functions")]
        [Space(15)]
        [SerializeField] private GameObject targetObject = null;
        [SerializeField] private bool isLockedOn = false;


        public event Action<float> OnHealthPctChanged = delegate { };

        public void setTeamID(float ID)
        {
            //set team ID
            teamID = ID;

            //set destination
            if (teamID == 1)
            {
                targetLocation = redBaseLocation;
            }

            else if (teamID == 2)
            {
                targetLocation = blueBaseLocation;
            }

            agent.SetDestination(targetLocation.transform.position);
        }

        void HealthBarUpdate(float oldValue, float newValue)
        {
            float currenthealthPct = newValue / maxHealth;
            OnHealthPctChanged(currenthealthPct);
        }

        public override void OnStartServer()
        {
            currenthealth = maxHealth;
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
                agent.SetDestination(targetLocation.transform.position);
            }
        }


        void Update()
        {
            if (isServer)
            {
                if (targetObject == null)
                {
                    agent.SetDestination(targetLocation.transform.position);
                    isLockedOn = false;
                    return;
                }

                //if boss 0% hp, bye bye
                if (currenthealth <= 0)
                    Destroyy();
            }
        }


        [Server]
        void ChaseTarget()
        {
            agent.SetDestination(targetObject.transform.position);
        }


        [Server]
        private void RangedAttackPlayer()
        {
            //stop moving
            agent.SetDestination(transform.position);
            agent.autoBraking = true;


            if (!alreadyattacked)
            {
      //          Debug.Log("C.Boss firing at " + targetObject.gameObject.name);
                transform.LookAt(targetObject.transform);
                FireProjectile();//Attack
                Invoke("ResetAttack", cooldown1);
            }

        }


        //normal linear projectile movement
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