using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace SheepDoom
{
    public class PlayerAttack : NetworkBehaviour
    {
        [Space(15)]
        [Header("for checking if player purchased special and ulti")]
        public bool hasPurchasedSpecial = false;
        public bool hasPurchasedUlti = false;

        [Space(15)]
        [Header("Bools to check which type of skills player chose")]
        public bool AlternateSpecial = false;
        public bool AlternateUlti = false;

        [Space(15)]
        [Header("Skill projectiles")]
        public GameObject Projectile;
        public GameObject Projectile2;
        public GameObject Projectile2_v2;
        public GameObject Projectile3;
        public GameObject Projectile3_v2;



        [Space(15)]
        [Header("skill launch point")]
        public Transform SpawnPoint;

        //skillcd basetrackers
        [SerializeField]
        private float cooldown1, cooldown2, cooldown3;

        [Header("skillcd values to be used n manipulated in game")]
        private float cooldown1_inGame, cooldown2_inGame, cooldown3_inGame;
        [Space(15)]
        //Melee Bool false
        public bool ismeeleeattack = false;
        [Space(15)]
        //Melee
        public Animator animator;
        [Space(15)]
        public Transform attackPoint;
        public float attackRange = 0.5f;
        public int meleedamage = 50;
        [Space(15)]
        public LayerMask enemyLayers;
        [Space(15)]
        [SyncVar] public bool isDead;

        // Start is called before the first frame update
        void Start()
        {
            if (!hasAuthority) return;
            cooldown1_inGame = cooldown1;
            cooldown2_inGame = cooldown2;
            cooldown3_inGame = cooldown3;

        }

        public void AttackClick()
        {
            if (!hasAuthority) return;
            CmdAttackClick();
        }

        public void SpecialSkillClick()
        {
            if (!hasAuthority) return;
            CmdSpecialSkillClick();
        }

        public void UltiClick()
        {
            if (!hasAuthority) return;
            CmdUltiClick();
        }

        [Command]
        //if atk button is pressed
        void CmdAttackClick()
        {
            //if not dead
            if (!isDead)
            {
                //if off cd
                if (cooldown1_inGame <= 0 && ismeeleeattack == false)
                {
                    Debug.Log("Firing Normal Atk");
                    var FiredProjectile = Instantiate(Projectile, SpawnPoint.position, SpawnPoint.rotation);
                    FiredProjectile.GetComponent<PlayerProjectileSettings>().CMD_setOwnerProjectile(this.gameObject);
                    NetworkServer.Spawn(FiredProjectile);

                    //resetcd
                    cooldown1_inGame = cooldown1;
                }
                else if (cooldown1_inGame <= 0 && ismeeleeattack == true)
                {
                    // animator.SetTrigger("Attack");
                    // animator.SetTrigger("AttackToIdle");
                    Collider[] hitenmies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

                    foreach (Collider enemy in hitenmies)
                    {
                        enemy.GetComponent<PlayerHealth>().modifyinghealth(-meleedamage);

                    }
                }
            }

        }

        //if special skill is pressed
        [Command]
        void CmdSpecialSkillClick()
        {
            if (!isDead)
            {
                //only available if special skill is purchased
                if (hasPurchasedSpecial)
                {
                    if (cooldown2_inGame <= 0)
                    {
                        //if 1st special is chosen
                        if (!AlternateSpecial)
                        {
                            Debug.Log("Firing Special Atk");
                            var FiredProjectile = Instantiate(Projectile2, SpawnPoint.position, SpawnPoint.rotation);
                            FiredProjectile.GetComponent<PlayerProjectileSettings>().CMD_setOwnerProjectile(this.gameObject);
                            NetworkServer.Spawn(FiredProjectile);
                            cooldown2_inGame = cooldown2;
                        }

                        //else its the second special
                        else
                        {
                            Debug.Log("Firing Special Atk 2");
                            var FiredProjectile = Instantiate(Projectile2_v2, SpawnPoint.position, SpawnPoint.rotation);
                            FiredProjectile.GetComponent<PlayerProjectileSettings>().CMD_setOwnerProjectile(this.gameObject);
                            NetworkServer.Spawn(FiredProjectile);
                            cooldown2_inGame = cooldown2;
                        }


                    }
                }
                else
                {
                    Debug.Log("Player hasn't purchased special skill");
                }
            }
        }

        //if ulti button is pressed
        [Command]
        void CmdUltiClick()
        {
            if (!isDead)
            {
                if (hasPurchasedUlti)
                {
                    if (cooldown3_inGame <= 0)
                    {
                        //if 1st ulti is chosen
                        if (!AlternateUlti)
                        {
                            Debug.Log("Firing Ultimate Atk");
                            var FiredProjectile = Instantiate(Projectile3, SpawnPoint.position, SpawnPoint.rotation);
                            FiredProjectile.GetComponent<PlayerProjectileSettings>().CMD_setOwnerProjectile(this.gameObject);
                            NetworkServer.Spawn(FiredProjectile);
                            cooldown3_inGame = cooldown3;
                        }

                        //else its 2nd ulti
                        else
                        {
                            Debug.Log("Firing Ultimate Atk 2");
                            var FiredProjectile = Instantiate(Projectile3_v2, SpawnPoint.position, SpawnPoint.rotation);
                            FiredProjectile.GetComponent<PlayerProjectileSettings>().CMD_setOwnerProjectile(this.gameObject);
                            NetworkServer.Spawn(FiredProjectile);
                            cooldown3_inGame = cooldown3;
                        }

                    }
                }
                else
                {
                    Debug.Log("Player hasn't purchased ultimate skill");
                }
            }

        }

        //command to set purchasespecial to true
        [Command]
        public void CMD_playerHasPurchasedSpecial()
        {
            hasPurchasedSpecial = true;
        }

        //command to set purchasedulti to true
        [Command]
        public void CMD_playerHasPurchasedUlti()
        {
            hasPurchasedUlti = true;
        }

        //command to set alternate special to true
        [Command]
        public void CMD_AlternateSpecial()
        {
            AlternateSpecial = true;
        }

        //command to set alternate ulti to true
        [Command]
        public void CMD_AlternateUlti()
        {
            AlternateUlti = true;
        }

        // Update is called once per frame
        void Update()
        {
            //if not 0, reduce cd per second
            if (cooldown1_inGame >= 0)
            {
                cooldown1_inGame -= Time.deltaTime;
            }

            if (cooldown2_inGame >= 0)
            {
                cooldown2_inGame -= Time.deltaTime;
            }

            if (cooldown3_inGame >= 0)
            {
                cooldown3_inGame -= Time.deltaTime;
            }
        }
    }
}
