using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace SheepDoom
{
    public class PlayerAttack : NetworkBehaviour
    {
        [Header("For checking if player purchased special and ulti")]
        [Space(15)]
        public bool hasPurchasedSpecial = false;
        public bool hasPurchasedUlti = false;

        [Header("Bools to check which type of skills player chose")]
        [Space(15)]
        public bool AlternateSpecial = false;
        public bool AlternateUlti = false;

        [Header("If player skills are firing projectiles")]
        public bool isSpecial1Projectile;
        public bool isSpecial2Projectile;
        public bool isUlti1Projectile;
        public bool isUlti2Projectile;

        [Header("If player skills is activating child instead")]
        public bool isSpecial1Child;
        public bool isSpecial2Child;
        public bool isUlti1Child;
        public bool isUlti2Child;

        [Header("If player skills are self buffs instead")]
        public bool isSpecial1Buff;
        public bool isSpecial2Buff;
        public bool isUlti1Buff;
        public bool isUlti2Buff;

        [Header("Skill projectiles")]
        [Space(15)]
        public GameObject Projectile;
        public GameObject MeleeAttackObject;
        public GameObject Projectile2;
        public GameObject Projectile2_v2;
        public GameObject Projectile3;
        public GameObject Projectile3_v2;

        [Header("Children to be activated")]
        [Space(15)]
        public GameObject SkillChild;
        public GameObject Skillchild_v2;
        public GameObject UltiChild;
        public GameObject UltiChild_v2;

        [Header("Skill launch point")]
        [Space(15)]
        public Transform SpawnPoint;

        //skillcd basetrackers
        [SerializeField]
        private float cooldown1, cooldown2, cooldown3;

        [Header("skillcd values to be used n manipulated in game")]
        private float cooldown1_inGame, cooldown2_inGame, cooldown3_inGame;

        [Header("Melee attack variables")]
        [Space(15)]
        public bool ismeeleeattack = false;
        public float meleeAttackDuration1;
        public float meleeCombo = 1;
        [Space(15)]
        //Melee
        public Animator animator;


        // Start is called before the first frame update
        void Start()
        {
            if (!hasAuthority) return;
            cooldown1_inGame = 0;
            cooldown2_inGame = 0;
            cooldown3_inGame = 0;

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
            if (!GetComponent<PlayerHealth>().isPlayerDead())
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
                    Debug.Log("Melee Normal Atk");

                    //turn on hitbox script
                    MeleeAttackObject.gameObject.GetComponent<OnTouchHealth>().hitboxActive = true;

 
                    if (meleeCombo == 1)
                    {
                        TargetMeleeAttack(connectionToClient, "right");
                        meleeCombo = 2;
                    }

                    else if (meleeCombo == 2)
                    {
                        TargetMeleeAttack(connectionToClient, "left");
                        meleeCombo = 1;
                    }

                    //resetcd
                    cooldown1_inGame = cooldown1;
                }
            }

        }

        [TargetRpc]
        void TargetMeleeAttack(NetworkConnection conn, string _meleeAtkDirection)
        {
            MeleeAttackObject.GetComponent<ObjectMovementScript>().move(meleeAttackDuration1, _meleeAtkDirection);
        }

        [TargetRpc]
        void TargetActivateChild1(NetworkConnection conn)
        {
            SkillChild.GetComponent<MeshRenderer>().enabled = true;
            SkillChild.GetComponent<BoxCollider>().enabled = true;
            SkillChild.GetComponent<PlayerChild>().refreshDuration();
        }

        //if special skill is pressed
        [Command]
        void CmdSpecialSkillClick()
        {
            if (!GetComponent<PlayerHealth>().isPlayerDead())
            {
                //only available if special skill is purchased
                if (hasPurchasedSpecial)
                {
                    if (cooldown2_inGame <= 0)
                    {
                        //if 1st special is chosen
                        if (!AlternateSpecial)
                        {
                            //special1 projectile settings
                            if (isSpecial1Projectile)
                            {
                                Debug.Log("Firing Special Atk");
                                var FiredProjectile = Instantiate(Projectile2, SpawnPoint.position, SpawnPoint.rotation);
                                FiredProjectile.GetComponent<PlayerProjectileSettings>().CMD_setOwnerProjectile(this.gameObject);
                                NetworkServer.Spawn(FiredProjectile);
                                cooldown2_inGame = cooldown2;
                            }

                            else if (isSpecial1Child)
                            {
                                TargetActivateChild1(connectionToClient);
                                cooldown2_inGame = cooldown2;
                            }

                            else if (isSpecial1Buff)
                            {

                            }

                        }

                        //else its the second special
                        else
                        {
                            //special2 projectile settings
                            if (isSpecial2Projectile)
                            {
                                Debug.Log("Firing Special Atk 2");
                                var FiredProjectile = Instantiate(Projectile2_v2, SpawnPoint.position, SpawnPoint.rotation);
                                FiredProjectile.GetComponent<PlayerProjectileSettings>().CMD_setOwnerProjectile(this.gameObject);
                                NetworkServer.Spawn(FiredProjectile);
                                cooldown2_inGame = cooldown2;
                            }

                            else if (isSpecial2Child)
                            {

                            }

                            else if (isSpecial2Buff)
                            {

                            }
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
            if (!GetComponent<PlayerHealth>().isPlayerDead())
            {
                if (hasPurchasedUlti)
                {
                    if (cooldown3_inGame <= 0)
                    {
                        //if 1st ulti is chosen
                        if (!AlternateUlti)
                        {
                            if (isUlti1Projectile)
                            {
                                Debug.Log("Firing Ultimate Atk");
                                var FiredProjectile = Instantiate(Projectile3, SpawnPoint.position, SpawnPoint.rotation);
                                FiredProjectile.GetComponent<PlayerProjectileSettings>().CMD_setOwnerProjectile(gameObject);
                                NetworkServer.Spawn(FiredProjectile);
                                cooldown3_inGame = cooldown3;
                            }

                            else if (isUlti1Child)
                            {

                            }

                            else if (isUlti1Buff)
                            {

                            }

                        }

                        //else its 2nd ulti
                        else
                        {
                            if (isUlti2Projectile)
                            {
                                Debug.Log("Firing Ultimate Atk 2");
                                var FiredProjectile = Instantiate(Projectile3_v2, SpawnPoint.position, SpawnPoint.rotation);
                                FiredProjectile.GetComponent<PlayerProjectileSettings>().CMD_setOwnerProjectile(gameObject);
                                NetworkServer.Spawn(FiredProjectile);
                                cooldown3_inGame = cooldown3;
                            }

                            else if (isUlti2Child)
                            {

                            }

                            else if (isUlti2Buff)
                            {

                            }

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
