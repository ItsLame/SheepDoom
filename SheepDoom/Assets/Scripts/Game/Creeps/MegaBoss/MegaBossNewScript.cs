using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using Mirror;

namespace SheepDoom
{
    public class MegaBossNewScript : NetworkBehaviour
    {
        [Header("Debug")]
        public float angle1;
        public float angle2;
        public bool spawnedConvertedBoss = false;
        public GameObject deadBossModel;

        [Header("Killer teamID")]
        [SerializeField] private float killerTeamID;
        [SerializeField] private GameObject blueTeamPos;
        [SerializeField] private GameObject redTeamPos;

        public Material color;
        [Header("NavMeshAgent to attach")]
        [SerializeField] private NavMeshAgent agent;

        [Header("Attack Cooldowns")]
        [Space(15)]
        //Attacking
        [SerializeField] private float cooldown1;
        [SerializeField] private float cooldown2;
        [SerializeField] private float cooldown3;
        [SerializeField] private float restTime;
        [SerializeField] private float restTimeInGame;

        [Space(15)]
        [SerializeField] private bool alreadyattacked;
        [SerializeField] private float attackRange;

        [Header("Attacking Patterns")]
        [Space(15)]
        [SerializeField] private bool FirstAttack;
        [SerializeField] private bool SecondAttack;
        [SerializeField] private bool ThirdAttack;
        [SerializeField] private float FirstAttackCounter;
        [SerializeField] private float SecondAttackCounter;
        [SerializeField] private float ThirdAttackCounter;
        [SerializeField] private float FirstAttackNumber;
        [SerializeField] private float SecondAttackNumber;
        [SerializeField] private float ThirdAttackNumber;
        [SerializeField] private bool enraged;
        [SerializeField] private bool callEnraged1;
        [SerializeField] private bool callEnraged2;

        [Header("Projectiles to fire")]
        [Space(15)]
        //Ranged Projectile
        [SerializeField] private GameObject firePosition;
        [SerializeField] private GameObject projectile;
        [SerializeField] private GameObject projectile2;
        [SerializeField] private GameObject projectile3;
        private GameObject firedProjectile = null;
        private GameObject firedProjectile2 = null;
        private GameObject firedProjectile3 = null;

        [Header("Health variable setting")]
        [Space(15)]
        [SerializeField] private float maxHealth;
        [SyncVar(hook = nameof(HealthBarUpdate))] [SerializeField] private float currenthealth;
        public float goldValue;

        [Header("Targetting functions")]
        [Space(15)]
        private GameObject targetObject = null;
        private bool isLockedOn = false;


        public event Action<float> OnHealthPctChanged = delegate { };

        void HealthBarUpdate(float oldValue, float newValue)
        {
            float currenthealthPct = newValue / maxHealth;
            OnHealthPctChanged(currenthealthPct);
        }

        public override void OnStartServer()
        {
            currenthealth = maxHealth;
            agent.autoBraking = false;

            //attacking variables
            FirstAttackNumber = 3;
            SecondAttackNumber = 1;
            ThirdAttackNumber = 1;
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            //dont bother with rest if locked on
            if (isLockedOn) return;

            //priority 1 = player
            else if (other.CompareTag("Player") && !other.GetComponent<PlayerHealth>().isPlayerDead())
            {
                targetObject = other.gameObject;
                agent.SetDestination(targetObject.transform.position);
                isLockedOn = true;
            }

            //atk da enemy creepu
            else if (other.CompareTag("TeamConsortiumRangeCreep"))
            {
                targetObject = other.gameObject;
                agent.SetDestination(targetObject.transform.position);
                isLockedOn = true;
            }

            //same attack the enemy creepes
            else if (other.CompareTag("TeamCoalitionRangeCreep"))
            {
                targetObject = other.gameObject;
                agent.SetDestination(targetObject.transform.position);
                isLockedOn = true;
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
                if (other.gameObject.GetComponent<PlayerHealth>().isPlayerDead())
                {
                    agent.SetDestination(transform.position);
                    isLockedOn = false;
                    targetObject = null;
                    agent.autoBraking = false;
                    agent.SetDestination(transform.position);
                }

                //attack player in range
                if (Vector3.Distance(gameObject.transform.position, other.gameObject.transform.position) < attackRange)
                {
                    //first attack 
                    if (FirstAttack)
                    {
                        RangedAttackPlayer();
                    }

                    //switch to second attack when first atk x times
                    if (FirstAttack && (FirstAttackCounter == FirstAttackNumber))
                    {
                        SecondAttack = true;
                        FirstAttack = false;
                        FirstAttackCounter = 0;
                    }

                    //second attack 
                    if (SecondAttack)
                    {
                        //if not enraged fire homing
                        if (!enraged)
                        {
                            RangedAttackPlayer2();
                        }

                        //if enraged fire normally
                        else if (enraged)
                        {
                            firedProjectile = Instantiate(projectile2, transform);
                            firedProjectile.GetComponent<MegaBossProjectileScript>().setOwner(gameObject);
                            firedProjectile.GetComponent<MegaBossProjectileScript>().setTarget(targetObject);
                            firedProjectile.transform.SetParent(null, false);
                            firedProjectile.transform.SetPositionAndRotation(firePosition.transform.position, firePosition.transform.rotation);
                            NetworkServer.Spawn(firedProjectile);

                            //add 1 to firing counter
                            SecondAttackCounter += 1;

                            alreadyattacked = true;
                        }
                    }

                    if (SecondAttack && (SecondAttackCounter == SecondAttackNumber))
                    {
                        SecondAttack = false;
                        SecondAttackCounter = 0;

                        if (enraged)
                        {
                            ThirdAttack = true;
                            FirstAttack = false;
                        }

                        else if (!enraged)
                        {
                            FirstAttack = true;
                        }
                    }

                    // enraged third atk from the ground 
                    // explodes after certain amount of time
                    if (ThirdAttack)
                    {
                        RangedAttackPlayer3();
                    }

                    if (ThirdAttack && (ThirdAttackCounter == ThirdAttackNumber))
                    {
                        ThirdAttack = false;
                        FirstAttack = true;
                        ThirdAttackCounter = 0;

                        if (enraged)
                        {
                            ThirdAttack = true;
                        }
                    }

                }
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
                agent.SetDestination(transform.position);
                isLockedOn = false;
                targetObject = null;
                agent.autoBraking = false;

                //reset attacking values
                FirstAttack = true;
                SecondAttack = false;
                ThirdAttack = false;

                SecondAttackCounter = 0;
                ThirdAttackCounter = 0;
                FirstAttackCounter = 0;
            }
        }

        void Update()
        {
            if (isServer)
            {
                //look at target if locked on
                if (isLockedOn)
                {
                    //aim fireposition at target
                    firePosition.transform.LookAt(targetObject.transform);

             //       this.transform.LookAt(targetObject.transform);
                    //rotate boss object horizontally only
                    Quaternion lookOnLook = Quaternion.LookRotation(targetObject.transform.position - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime);
                }

                if (targetObject == null)
                {
                    isLockedOn = false;
                    agent.SetDestination(transform.position);
                    return;
                }

                //if boss below 50% HP, activate enrage
                if ((currenthealth / maxHealth < 0.5) && callEnraged1 == false)
                {
              //      Debug.Log("Call Boss Enrage v1");
                    increaseAttackTimes();
                    callEnraged1 = true;
                }

                //if boss below 20% HP, activate enrage #2
                if ((currenthealth / maxHealth < 0.3) && callEnraged2 == false)
                {
            //        Debug.Log("Call Boss Enrage v2");
                    increaseAttackTimes2();
                    callEnraged2 = true;
                }

                //if boss 0% hp, bye bye
                if (currenthealth <= 0)
                {
                    if (!spawnedConvertedBoss)
                    {
                        if (killerTeamID == 1)
                        {
               //             Debug.Log("Spawning boss from blue team");

                            blueTeamPos.GetComponent<BaseCreepSpawner>().spawnConvertedBoss();
                        }

                        else if (killerTeamID == 2)
                        {
                 //           Debug.Log("Spawning boss from red team");
                            redTeamPos.GetComponent<BaseCreepSpawner>().spawnConvertedBoss();
                        }

                        spawnedConvertedBoss = true;
                    }

                    firedProjectile = Instantiate(deadBossModel, transform);
                    firedProjectile.transform.SetParent(null, false);
                    firedProjectile.transform.SetPositionAndRotation(this.transform.position + new Vector3(0, -38, 0), firePosition.transform.rotation);
                    NetworkServer.Spawn(firedProjectile);

                    Destroyy();
                }

            }
        }


        //enrage values
        //increases attack patterns + introduce new atk pattern
        void increaseAttackTimes()
        {
            FirstAttackNumber += 2;
            SecondAttackNumber += 1;
            cooldown1 -= 0.2f;
            cooldown2 -= 0.5f;
        }

        void increaseAttackTimes2()
        {
            FirstAttackNumber += 2;
            cooldown1 -= 0.2f;
            enraged = true;

            //change color to red
            GetComponent<MeshRenderer>().materials = new Material[2] { GetComponent<MeshRenderer>().materials[2], color};
        }


        [Server]
        private void RangedAttackPlayer()
        {
            //stop moving
            agent.SetDestination(transform.position);
            agent.autoBraking = true;



            if (!alreadyattacked)
            {
                //               transform.LookAt(targetObject.transform);
                FireProjectile();//Attack
                Invoke("ResetAttack", cooldown1);
            }

        }

        [Server]
        private void RangedAttackPlayer2()
        {
            //stop moving
            agent.SetDestination(transform.position);
            agent.autoBraking = true;

            if (!alreadyattacked)
            {
                //               transform.LookAt(targetObject.transform);
                FireProjectile2();//Attack
                Invoke("ResetAttack", cooldown2);
            }

        }

        [Server]
        private void RangedAttackPlayer3()
        {
            //stop moving
            agent.SetDestination(transform.position);
            agent.autoBraking = true;

            if (!alreadyattacked)
            {
                //               transform.LookAt(targetObject.transform);
                FireProjectile3();//Attack
                Invoke("ResetAttack", cooldown3);
            }

        }

        //normal linear projectile movement
        [Server]
        private void FireProjectile()
        {
            firedProjectile = Instantiate(projectile, transform);
            firedProjectile.GetComponent<MegaBossProjectileScript>().setOwner(gameObject);
            firedProjectile.GetComponent<MegaBossProjectileScript>().setTarget(targetObject);
            firedProjectile.transform.SetParent(null, false);
            firedProjectile.transform.SetPositionAndRotation(firePosition.transform.position, firePosition.transform.rotation);
            NetworkServer.Spawn(firedProjectile);

            // 2 more bullets diagonally when enraged
            // 2hu modo
            if (enraged)
            {
                var firedProjectile2 = Instantiate(projectile, transform);
                firedProjectile2.GetComponent<MegaBossProjectileScript>().setOwner(gameObject);
                firedProjectile2.GetComponent<MegaBossProjectileScript>().setTarget(targetObject);
                firedProjectile2.transform.SetParent(null, false);
                firedProjectile2.transform.SetPositionAndRotation(firePosition.transform.position, 
                    firePosition.transform.rotation * (Quaternion.Euler(0, angle1, 0)));
                NetworkServer.Spawn(firedProjectile2);


                var firedProjectile3 = Instantiate(projectile, transform);
                firedProjectile3.GetComponent<MegaBossProjectileScript>().setOwner(gameObject);
                firedProjectile3.GetComponent<MegaBossProjectileScript>().setTarget(targetObject);
                firedProjectile3.transform.SetParent(null, false);
                firedProjectile3.transform.SetPositionAndRotation(firePosition.transform.position,
                    firePosition.transform.rotation * (Quaternion.Euler(0, angle2, 0)));
                NetworkServer.Spawn(firedProjectile3);
            }

            //add 1 to firing counter
            FirstAttackCounter += 1;

            alreadyattacked = true;
        }

        //homing projectile
        [Server]
        private void FireProjectile2()
        {
            firedProjectile = Instantiate(projectile2, transform);
            firedProjectile.GetComponent<MegaBossProjectileScript>().setOwner(gameObject);
            firedProjectile.GetComponent<MegaBossProjectileScript>().setHoming();
            firedProjectile.GetComponent<MegaBossProjectileScript>().setTarget(targetObject);
            firedProjectile.transform.SetParent(null, false);
            firedProjectile.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
            NetworkServer.Spawn(firedProjectile);

            //add 1 to firing counter
            SecondAttackCounter += 1;

            alreadyattacked = true;
        }

        //homing projectile
        [Server]
        private void FireProjectile3()
        {
            firedProjectile = Instantiate(projectile3, transform);
            firedProjectile.GetComponent<MegaBossProjectileScript>().setOwner(gameObject);
            firedProjectile.GetComponent<MegaBossProjectileScript>().setTarget(targetObject);
            firedProjectile.GetComponent<MegaBossProjectileScript>().setDetonate();
            firedProjectile.transform.SetParent(null, false);
            firedProjectile.transform.SetPositionAndRotation(targetObject.transform.position, targetObject.transform.rotation);
            NetworkServer.Spawn(firedProjectile);

            //add 1 to firing counter
            ThirdAttackCounter += 1;

            alreadyattacked = true;
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

        public void setKillerTeamID(float ID)
        {
            killerTeamID = ID;
        }

        public float getHealth()
        {
            return currenthealth;
        }
    }
}