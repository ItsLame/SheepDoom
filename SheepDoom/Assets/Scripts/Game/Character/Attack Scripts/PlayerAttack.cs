using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace SheepDoom
{
    public class PlayerAttack : NetworkBehaviour
    {
        [Header("PlayerID")]
        [SerializeField]
        private float charID;

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

        //cooldown 1 cd multiplier
        [SerializeField]
        [Space(15)]
        private float cooldown1Multiplier, cooldown1MultiplerTimer, cooldown1MultiplierTimerInGame;
        private bool cooldown1MultiplierActive;
        private bool resetNormalAtk = false;

        [Header("Skillcd values to be used n manipulated in game")]
        [SerializeField]
        private float cooldown1_inGame, cooldown2_inGame, cooldown3_inGame;
        private bool cooldown1Happening;

        [Header("Melee attack variables")]
        [Space(15)]
        [SerializeField]
        private bool ismeeleeattack;
        [SerializeField]
        private float meleeAttackDuration1, meleeAttackSpeedMultiplier;
        private float meleeCombo;
        [Space(15)]
        //Melee
        public Animator animator;

        public override void OnStartClient()
        {
            if (!hasAuthority) return;
            cooldown1_inGame = 0;
            cooldown2_inGame = 0;
            cooldown3_inGame = 0;

            if (ismeeleeattack && MeleeAttackObject != null)
                meleeCombo = 1;

            cooldown1MultiplierActive = false;
            cooldown1MultiplierTimerInGame = cooldown1MultiplerTimer;
            //            meleeAttackSpeedMultiplier = 1;

            //get playerID
            charID = this.gameObject.GetComponent<PlayerAdmin>().getCharID();
        }

        public void AttackClick()
        {
            if (!hasAuthority) return;

            if (cooldown1_inGame <= 0)
            {
                CmdAttackClick(ismeeleeattack);

                if (!ismeeleeattack)
                    cooldown1_inGame = cooldown1 * cooldown1Multiplier;
                else if (ismeeleeattack && MeleeAttackObject != null)
                {
                    if (meleeCombo == 1)
                    {
                        if (!cooldown1MultiplierActive)
                        {
                            MeleeAttackObject.GetComponent<ObjectMovementScript>().SetMoveSpeed(120);
                            MeleeAttackObject.GetComponent<ObjectMovementScript>().move(meleeAttackDuration1, "right");
                        }
                        else if (cooldown1MultiplierActive)
                        {
                            MeleeAttackObject.GetComponent<ObjectMovementScript>().SetMoveSpeed(120 * meleeAttackSpeedMultiplier);
                            MeleeAttackObject.GetComponent<ObjectMovementScript>().move((meleeAttackDuration1 / meleeAttackSpeedMultiplier), "right");
                        }

                        meleeCombo = 2;
                    }
                    else if (meleeCombo == 2)
                    {
                        if (!cooldown1MultiplierActive)
                        {
                            MeleeAttackObject.GetComponent<ObjectMovementScript>().SetMoveSpeed(120);
                            MeleeAttackObject.GetComponent<ObjectMovementScript>().move(meleeAttackDuration1, "left");
                        }
                        else if (cooldown1MultiplierActive)
                        {
                            MeleeAttackObject.GetComponent<ObjectMovementScript>().SetMoveSpeed(120 * meleeAttackSpeedMultiplier);
                            MeleeAttackObject.GetComponent<ObjectMovementScript>().move((meleeAttackDuration1 / meleeAttackSpeedMultiplier), "left");
                        }

                        meleeCombo = 1;
                    }

                    cooldown1_inGame = cooldown1;
                }
            }
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
        void CmdAttackClick(bool attackType)
        {
            //if not dead
            if (!GetComponent<PlayerHealth>().isPlayerDead())
            {
                if (!attackType) // ranged projectile
                {
                    GameObject FiredProjectile = Instantiate(Projectile, SpawnPoint.position, SpawnPoint.rotation);
                    FiredProjectile.GetComponent<PlayerProjectileSettings>().SetOwnerProjectile(gameObject);
                    NetworkServer.Spawn(FiredProjectile, connectionToClient);
                    //TargetAttackManipulate(connectionToClient, true, false, (cooldown1 * cooldown1Multiplier));
                }
                else if (attackType && MeleeAttackObject != null) // melee attack
                    MeleeAttackObject.GetComponent<OnTouchHealth>().SetHitBox(true);
                //if off cd
                /*if (cooldown1_inGame <= 0 && ismeeleeattack == false)
                {
                    Debug.Log("Firing Normal Atk");
                    var FiredProjectile = Instantiate(Projectile, SpawnPoint.position, SpawnPoint.rotation);
                    FiredProjectile.GetComponent<PlayerProjectileSettings>().SetOwnerProjectile(this.gameObject);
                    NetworkServer.Spawn(FiredProjectile, connectionToClient);

                    //resetcd
                    cooldown1_inGame = cooldown1 * cooldown1Multiplier;
                }*/

                /* else if (cooldown1_inGame <= 0 && ismeeleeattack == true)
                 {
                     Debug.Log("Melee Normal Atk");

                     //turn on hitbox script
                     MeleeAttackObject.GetComponent<OnTouchHealth>().SetHitBox(true);

                     if (!cooldown1MultiplierActive)
                     {
                         MeleeAttackObject.gameObject.GetComponent<ObjectMovementScript>().SetMoveSpeed(120);
                     }

                     if (cooldown1MultiplierActive)
                     {
                         MeleeAttackObject.gameObject.GetComponent<ObjectMovementScript>().SetMoveSpeed(120 * meleeAttackSpeedMultiplier);
                     }


                     if (meleeCombo == 1)
                     {
                         //TargetMeleeAttack(/*connectionToClient, "right");
                         meleeCombo = 2;
                     }

                     else if (meleeCombo == 2)
                     {
                         //TargetMeleeAttack(/*connectionToClient, "left");
                         meleeCombo = 1;
                     }

                     //resetcd
                     cooldown1_inGame = cooldown1;
                 }*/
            }
        }

        [Client] // call from client to tell server
        public void ServerSetHitBox(bool _hitBox)
        {
            if (!hasAuthority) return;
            CmdSetHitBox(_hitBox);
        }

        [Command]
        void CmdSetHitBox(bool _hitBox)
        {
            MeleeAttackObject.GetComponent<OnTouchHealth>().SetHitBox(_hitBox);
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
                            // Special 1 for Character 1
                            // Fires a ball that starts out fast then ends at a slow constant speed
                            // Inflicts slow debuff
                            if (charID == 1)
                            {
                                Debug.Log("Firing Special Atk");
                                var FiredProjectile = Instantiate(Projectile2, SpawnPoint.position, SpawnPoint.rotation);
                                FiredProjectile.GetComponent<PlayerProjectileSettings>().SetOwnerProjectile(this.gameObject);
                                NetworkServer.Spawn(FiredProjectile, connectionToClient);
                                cooldown2_inGame = cooldown2;
                            }


                            // Special 1 for Character 2
                            // Activates a Shield in front of the player (child)
                            // Blocks all projectiles except piercing projectiles (not destroyed on contact)]
                            else if (charID == 2)
                            {
                                Debug.Log("Activating Shield");
                                TargetActivateChild1_Clients();
                                TargetActivateChild1_Server();
                                cooldown2_inGame = cooldown2;
                            }

                            /*
                                //special1 projectile settings
                                if (isSpecial1Projectile)
                                {
                                Debug.Log("Firing Special Atk");
                                var FiredProjectile = Instantiate(Projectile2, SpawnPoint.position, SpawnPoint.rotation);
                                FiredProjectile.GetComponent<PlayerProjectileSettings>().SetOwnerProjectile(this.gameObject);
                                NetworkServer.Spawn(FiredProjectile, connectionToClient);
                                cooldown2_inGame = cooldown2;
                                }*/

                            /*
                            else if (isSpecial1Child)
                            {
                                TargetActivateChild1_Clients();
                                TargetActivateChild1_Server();
                                cooldown2_inGame = cooldown2;
                            } */

                            else if (isSpecial1Buff)
                            {


                            }

                        }

                        //else its the second special
                        else
                        {
                            // Special 2 for Character 1
                            // Drops a landmine projectile with a very long duration
                            // Detonate & stuns on enemy contact
                            if (charID == 1)
                            {
                                Debug.Log("Firing Special Atk 2");
                                var FiredProjectile = Instantiate(Projectile2_v2, SpawnPoint.position, SpawnPoint.rotation);
                                FiredProjectile.GetComponent<PlayerProjectileSettings>().SetOwnerProjectile(this.gameObject);
                                NetworkServer.Spawn(FiredProjectile, connectionToClient);
                                cooldown2_inGame = cooldown2;
                            }

                            // Special 2 for Character 2 
                            // Activates a self attack speed buff that increases atk intervals by 2x
                            else if (charID == 2)
                            {
                                //dont cast buff before atk animation finish
                                if (cooldown1Happening) return;
                                Debug.Log("Atk Spd Buff activated");
                                cooldown1MultiplierTimerInGame = cooldown1MultiplerTimer;
                                cooldown1MultiplierActive = true;
                                cooldown1 = (cooldown1 * cooldown1Multiplier) + 0.05f;
                                cooldown2_inGame = cooldown2;
                            }

                            /*
                            //special2 projectile settings
                            if (isSpecial2Projectile)
                            {
                                Debug.Log("Firing Special Atk 2");
                                var FiredProjectile = Instantiate(Projectile2_v2, SpawnPoint.position, SpawnPoint.rotation);
                                FiredProjectile.GetComponent<PlayerProjectileSettings>().SetOwnerProjectile(this.gameObject);
                                NetworkServer.Spawn(FiredProjectile, connectionToClient);
                                cooldown2_inGame = cooldown2;
                            }

                            else if (isSpecial2Child)
                            {

                            }
                            
                            else if (isSpecial2Buff)
                            {
                                //dont cast buff before atk animation finish
                                if (cooldown1Happening) return;
                                Debug.Log("Atk Spd Buff activated");
                                cooldown1MultiplierTimerInGame = cooldown1MultiplerTimer;
                                cooldown1MultiplierActive = true;
                                cooldown1 = (cooldown1 * cooldown1Multiplier) + 0.05f;
                                cooldown2_inGame = cooldown2;
                            }*/
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
                            // Ultimate 1 for Character 1
                            // A linear piercing laser with high speed & dmg
                            // Is global
                            if (charID == 1)
                            {
                                Debug.Log("Firing Ultimate Atk");
                                var FiredProjectile = Instantiate(Projectile3, SpawnPoint.position, SpawnPoint.rotation);
                                FiredProjectile.GetComponent<PlayerProjectileSettings>().SetOwnerProjectile(gameObject);
                                NetworkServer.Spawn(FiredProjectile, connectionToClient);
                                cooldown3_inGame = cooldown3;
                            }

                            // 
                            else if (charID == 2)
                            {

                            }

                            /*
                            if (isUlti1Projectile)
                            {
                                Debug.Log("Firing Ultimate Atk");
                                var FiredProjectile = Instantiate(Projectile3, SpawnPoint.position, SpawnPoint.rotation);
                                FiredProjectile.GetComponent<PlayerProjectileSettings>().SetOwnerProjectile(gameObject);
                                NetworkServer.Spawn(FiredProjectile, connectionToClient);
                                cooldown3_inGame = cooldown3;
                            }
                            

                            else if (isUlti1Child)
                            {

                            }

                            else if (isUlti1Buff)
                            {

                            } */

                        }

                        //else its 2nd ulti
                        else
                        {
                            // Ultimate 2 for Character 1
                            // Fires a wider piercing laser that starts off slow and gradually accelerates
                            // Is global
                            if (charID == 1)
                            {
                                Debug.Log("Firing Ultimate Atk 2");
                                var FiredProjectile = Instantiate(Projectile3_v2, SpawnPoint.position, SpawnPoint.rotation);
                                FiredProjectile.GetComponent<PlayerProjectileSettings>().SetOwnerProjectile(gameObject);
                                NetworkServer.Spawn(FiredProjectile, connectionToClient);
                                cooldown3_inGame = cooldown3;
                            }

                            // Ultimate 2 for Character 2 
                            // Big MoveSPD boost for a long duration
                            // Maybe reduce dmg taken too next time
                            else if (charID == 2)
                            {
                                Debug.Log("MoveSpd Buff activated");
                                //speed up char movespd by 1.5x for 5s
                                this.gameObject.GetComponent<CharacterMovement>().changeSpeed("speedUp", 5, 1.5f);
                                cooldown3_inGame = cooldown3;
                            }

                            /*
                            if (isUlti2Projectile)
                            {
                                Debug.Log("Firing Ultimate Atk 2");
                                var FiredProjectile = Instantiate(Projectile3_v2, SpawnPoint.position, SpawnPoint.rotation);
                                FiredProjectile.GetComponent<PlayerProjectileSettings>().SetOwnerProjectile(gameObject);
                                NetworkServer.Spawn(FiredProjectile, connectionToClient);
                                cooldown3_inGame = cooldown3;
                            }

                            else if (isUlti2Child)
                            {

                            }

                            else if (isUlti2Buff)
                            {
                                Debug.Log("MoveSpd Buff activated");
                                //speed up char movespd by 1.5x for 5s
                                this.gameObject.GetComponent<CharacterMovement>().changeSpeed("speedUp", 5, 1.5f);
                                cooldown3_inGame = cooldown3;
                            } */

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
                cooldown1Happening = true;
                cooldown1_inGame -= Time.deltaTime;
            }
            else if (cooldown1_inGame <= 0)
            {
                cooldown1Happening = false;
            }
            // --------------------------------------------------------------------//

            if (cooldown1MultiplierActive)
            {
                cooldown1MultiplierTimerInGame -= Time.deltaTime;
            }

            //remove atkspd buff once times up
            if (cooldown1MultiplierTimerInGame <= 0 && cooldown1MultiplierActive)
            {
                resetNormalAtk = true;
            }

            if (resetNormalAtk == true)
            {
                normalAtkNormalize();
                resetNormalAtk = false;
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

        public void normalAtkNormalize()
        {
            cooldown1MultiplierActive = false;
            cooldown1 = (cooldown1 - 0.05f) / cooldown1Multiplier;
            Debug.Log("Atk Spd Buff ended");
        }

        [ClientRpc]
        void TargetActivateChild1_Clients()
        {
            SkillChild.GetComponent<MeshRenderer>().enabled = true;
            SkillChild.GetComponent<BoxCollider>().enabled = true;
            SkillChild.GetComponent<PlayerChild>().refreshDuration();

        }

        void TargetActivateChild1_Server()
        {
            SkillChild.GetComponent<MeshRenderer>().enabled = true;
            SkillChild.GetComponent<BoxCollider>().enabled = true;
            SkillChild.GetComponent<PlayerChild>().refreshDuration();

        }
    }
}
