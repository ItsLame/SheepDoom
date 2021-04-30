using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class Character2 : NetworkBehaviour
    {
        [SerializeField]
        private GameObject normalAtkMelee, normalAtkMelee2, normalSpecial, normalUlti;
        private GameObject firedProjectile;




        [SerializeField]
        private Transform spawnPoint, meleeAtkSpawn1, meleeAtkSpawn2;


        [SerializeField]
        private float meleeAttackDuration1, meleeAttackSpeedMultiplier;
        private int meleeCombo = 1;

        [Client]
        public void normalAtk(bool _multiplier)
        {
            CmdNormalAtk(_multiplier);
        }

        [Client]
        public void setProjectileDirection(GameObject projectile, string direction)
        {
            projectile.GetComponent<PlayerProjectileSettings>().setDirection(direction);
        }

        [Command]
        void CmdNormalAtk(bool _multiplier)
        {
            if (meleeCombo == 1)
            {
                if (!_multiplier)
                {
                    Debug.Log("Firing melee from left to right");
                    firedProjectile = Instantiate(normalAtkMelee, meleeAtkSpawn1);
                    firedProjectile.GetComponent<PlayerProjectileSettings>().SetOwnerProjectile(gameObject);
                    firedProjectile.transform.SetParent(null, false);
                    firedProjectile.transform.SetPositionAndRotation(meleeAtkSpawn1.position, meleeAtkSpawn1.rotation);

                    //set direction
                    //                 setProjectileDirection(firedProjectile, "right");
                    firedProjectile.GetComponent<PlayerProjectileSettings>().isMovingRight = true;

                    NetworkServer.Spawn(firedProjectile, connectionToClient);
                    //                   comp.SetMoveSpeed(120);
                    //                   comp.move(meleeAttackDuration1, "right");
                }
                else if (_multiplier)
                {
                    Debug.Log("Firing melee from left to right v2");
                    firedProjectile = Instantiate(normalAtkMelee, meleeAtkSpawn1);
                    firedProjectile.GetComponent<PlayerProjectileSettings>().SetOwnerProjectile(gameObject);

                    //change speed and lifetime
                    firedProjectile.GetComponent<PlayerProjectileSettings>().m_Lifespan /= meleeAttackSpeedMultiplier;
                    firedProjectile.GetComponent<PlayerProjectileSettings>().m_Speed *= meleeAttackSpeedMultiplier;

                    //set direction
                    setProjectileDirection(firedProjectile, "right");
                    //                    firedProjectile.GetComponent<PlayerProjectileSettings>().setDirection("right");

                    firedProjectile.transform.SetParent(null, false);
                    firedProjectile.transform.SetPositionAndRotation(meleeAtkSpawn1.position, meleeAtkSpawn1.rotation);

                    NetworkServer.Spawn(firedProjectile, connectionToClient);

                    //                   comp.SetMoveSpeed(120 * meleeAttackSpeedMultiplier);
                    //                   comp.move((meleeAttackDuration1 / meleeAttackSpeedMultiplier), "right");
                }
                meleeCombo = 2;
            }
            else if (meleeCombo == 2)
            {
                if (!_multiplier)
                {
                    Debug.Log("Firing melee from right to left");
                    firedProjectile = Instantiate(normalAtkMelee, meleeAtkSpawn2);
                    firedProjectile.GetComponent<PlayerProjectileSettings>().SetOwnerProjectile(gameObject);
                    firedProjectile.transform.SetParent(null, false);
                    firedProjectile.transform.SetPositionAndRotation(meleeAtkSpawn2.position, meleeAtkSpawn2.rotation);

                    //set direction
                    //                    setProjectileDirection(firedProjectile, "left");
                    firedProjectile.GetComponent<PlayerProjectileSettings>().isMovingLeft = true;

                    NetworkServer.Spawn(firedProjectile, connectionToClient);
                    //                   comp.GetComponent<ObjectMovementScript>().SetMoveSpeed(120);
                    //                   comp.GetComponent<ObjectMovementScript>().move(meleeAttackDuration1, "left");
                }
                else if (_multiplier)
                {
                    Debug.Log("Firing melee from right to left v2");
                    firedProjectile = Instantiate(normalAtkMelee, meleeAtkSpawn2);
                    firedProjectile.GetComponent<PlayerProjectileSettings>().SetOwnerProjectile(gameObject);

                    //change speed and lifetime
                    firedProjectile.GetComponent<PlayerProjectileSettings>().m_Lifespan /= meleeAttackSpeedMultiplier;
                    firedProjectile.GetComponent<PlayerProjectileSettings>().m_Speed *= meleeAttackSpeedMultiplier;

                    //set direction
                    setProjectileDirection(firedProjectile, "left");
                    //                    firedProjectile.GetComponent<PlayerProjectileSettings>().setDirection("left");

                    firedProjectile.transform.SetParent(null, false);
                    firedProjectile.transform.SetPositionAndRotation(meleeAtkSpawn2.position, meleeAtkSpawn2.rotation);

                    NetworkServer.Spawn(firedProjectile, connectionToClient);
                    //                   comp.SetMoveSpeed(120 * meleeAttackSpeedMultiplier);
                    //                   comp.move((meleeAttackDuration1 / meleeAttackSpeedMultiplier), "left");
                }
                meleeCombo = 1;
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
            normalAtkMelee.GetComponent<OnTouchHealth>().SetHitBox(_hitBox);
        }

        [Client]
        public void SpecialAtk()
        {
            CmdSpecialAtk();
        }

        [Command]
        void CmdSpecialAtk()
        {
            /* 
                             firedProjectile = Instantiate(altUlti, transform);
                firedProjectile.GetComponent<ProjectileHealScript>().SetOwnerProjectile(gameObject);
                firedProjectile.transform.SetParent(null, false);
                firedProjectile.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

                //setting channeling conditions (if owner moves, destroy)
                firedProjectile.GetComponent<ChannelingScript>().setOwner(this.gameObject);

                //setting healing over time conditions (heal allies, damage enemies)
                firedProjectile.GetComponent<HealActivateScript>().setTeamID(this.gameObject.GetComponent<PlayerAdmin>().getTeamIndex());
                firedProjectile.GetComponent<HealActivateScript>().activateHeal();


                NetworkServer.Spawn(firedProjectile, connectionToClient);
             */

            firedProjectile = Instantiate(normalSpecial, transform);
            firedProjectile.transform.SetParent(null, false);
            firedProjectile.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

            //set owner
            firedProjectile.GetComponent<ObjectFollowScript>().owner = this.gameObject;

            NetworkServer.Spawn(firedProjectile, connectionToClient);
        }

        [Client]
        public void UltiAtk(bool _isAltUlti)
        {
            Vector3 additionalDistance = new Vector3(0, 40, 0);
            additionalDistance += (transform.forward * 30);
            CmdUltiAtk(additionalDistance, _isAltUlti);
        }

        [Command]
        void CmdUltiAtk(Vector3 _distance, bool _isAltUlti)
        {
            if (!_isAltUlti)
            {
                //firedProjectile = Instantiate(normalUlti, spawnPoint.position + _distance, spawnPoint.rotation);
                firedProjectile = Instantiate(normalUlti, transform);
                firedProjectile.GetComponent<PlayerProjectileSettings>().SetOwnerProjectile(gameObject);
                firedProjectile.transform.SetParent(null, false);
                firedProjectile.transform.SetPositionAndRotation(spawnPoint.position + _distance, spawnPoint.rotation);

                NetworkServer.Spawn(firedProjectile, connectionToClient);
            }
            else if (_isAltUlti)
                GetComponent<CharacterMovement>().changeSpeed("speedUp", 5, 1.5f);
        }

        /*
        [ClientRpc]
        void RpcEnableSkill()
        {
            if (normalSpecial != null)
            {
                normalSpecial.GetComponent<MeshRenderer>().enabled = true;
                normalSpecial.GetComponent<BoxCollider>().enabled = true;
                normalSpecial.GetComponent<PlayerChild>().refreshDuration();
            }
        } */
        }
    }
