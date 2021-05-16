using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class Character2 : NetworkBehaviour
    {
        [SerializeField]
        private GameObject normalAtkMelee, normalAtkMelee2, normalSpecial, normalUlti, AltSpecialBuff, AltUlti;
        private GameObject firedProjectile;




        [SerializeField]
        private Transform spawnPoint, meleeAtkSpawn1, meleeAtkSpawn2, buffspawnpoint, UltiAltSpawnpoint;


        [SerializeField]
        private float meleeAttackDuration1, meleeAttackSpeedMultiplier;
        private int meleeCombo = 1;
        [SerializeField] public NetworkAnimator networkAnimator;

        [Client]
        public void normalAtk(bool _multiplier)
        {
            if (meleeCombo == 1)
            {
                networkAnimator.SetTrigger("AstarothAttack");
            }
            else if (meleeCombo == 2)
            {
                networkAnimator.SetTrigger("AstarothAttack2");

            }
            StartCoroutine(Character2Attack());
            CmdNormalAtk(_multiplier);
        }
        IEnumerator Character2Attack()
        {
            yield return new WaitForSeconds(0.2f);
            if (meleeCombo == 1)
            {
                meleeCombo = 2;
            }
            else if (meleeCombo == 2)
            {
                meleeCombo = 1;
            }
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
                    Debug.Log("Firing melee from right to left");
                    firedProjectile = Instantiate(normalAtkMelee, meleeAtkSpawn1);
                    firedProjectile.GetComponent<PlayerProjectileSettings>().SetOwnerProjectile(gameObject);
                    firedProjectile.transform.SetParent(null, false);
                    firedProjectile.transform.SetPositionAndRotation(meleeAtkSpawn1.position, meleeAtkSpawn1.rotation);

                    //set direction
                    //                 setProjectileDirection(firedProjectile, "right");
                    firedProjectile.GetComponent<PlayerProjectileSettings>().isMovingLeft = true;

                    NetworkServer.Spawn(firedProjectile, connectionToClient);
                    //                   comp.SetMoveSpeed(120);
                    //                   comp.move(meleeAttackDuration1, "right");
                }
                else if (_multiplier)
                {
                    Debug.Log("Firing melee from right to left v2");
                    firedProjectile = Instantiate(normalAtkMelee, meleeAtkSpawn1);
                    firedProjectile.GetComponent<PlayerProjectileSettings>().SetOwnerProjectile(gameObject);

                    //change speed and lifetime
                    firedProjectile.GetComponent<PlayerProjectileSettings>().m_Lifespan /= meleeAttackSpeedMultiplier;
                    firedProjectile.GetComponent<PlayerProjectileSettings>().m_Speed *= meleeAttackSpeedMultiplier;

                    //set direction
                    firedProjectile.GetComponent<PlayerProjectileSettings>().isMovingLeft = true;
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
                    Debug.Log("Firing melee from left to right");
                    firedProjectile = Instantiate(normalAtkMelee, meleeAtkSpawn1);
                    firedProjectile.GetComponent<PlayerProjectileSettings>().SetOwnerProjectile(gameObject);
                    firedProjectile.transform.SetParent(null, false);
                    firedProjectile.transform.SetPositionAndRotation(meleeAtkSpawn1.position, meleeAtkSpawn1.rotation);

                    //set direction
                    //                    setProjectileDirection(firedProjectile, "left");
                    firedProjectile.GetComponent<PlayerProjectileSettings>().isMovingRight = true;

                    NetworkServer.Spawn(firedProjectile, connectionToClient);
                    //                   comp.GetComponent<ObjectMovementScript>().SetMoveSpeed(120);
                    //                   comp.GetComponent<ObjectMovementScript>().move(meleeAttackDuration1, "left");
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
                    firedProjectile.GetComponent<PlayerProjectileSettings>().isMovingRight = true;
                    //                    firedProjectile.GetComponent<PlayerProjectileSettings>().setDirection("left");

                    firedProjectile.transform.SetParent(null, false);
                    firedProjectile.transform.SetPositionAndRotation(meleeAtkSpawn1.position, meleeAtkSpawn1.rotation);

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
        public void SpecialAtk(bool _isAltSpecial)
        {
            CmdSpecialAtk(_isAltSpecial);
        }

        [Command]
        void CmdSpecialAtk(bool _isAltSpecial)
        {
            if (!_isAltSpecial)
            {

                firedProjectile = Instantiate(normalSpecial, transform);
                firedProjectile.transform.SetParent(null, false);
                firedProjectile.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

                //set owner
                firedProjectile.GetComponent<ObjectFollowScript>().owner = this.gameObject;

                NetworkServer.Spawn(firedProjectile, connectionToClient);
            }
            if (_isAltSpecial)
            {
                //For Buff Effect Only
                firedProjectile = Instantiate(AltSpecialBuff, transform);
                firedProjectile.transform.SetParent(null, false);
                firedProjectile.transform.SetPositionAndRotation(buffspawnpoint.position, buffspawnpoint.rotation);

                //set owner
                firedProjectile.GetComponent<BuffFollowScript>().owner = this.gameObject;

                NetworkServer.Spawn(firedProjectile, connectionToClient);
            }
        }

        [Client]
        public void UltiAtk(bool _isAltUlti)
        {
            /*Vector3 additionalDistance = new Vector3(0, 40, 0);
            additionalDistance += (transform.forward * 30);*/
            if (!_isAltUlti)
            {
                networkAnimator.SetTrigger("AstarothUlti1");
            }
            CmdUltiAtk(_isAltUlti);
        }

        [Command]
        void CmdUltiAtk(bool _isAltUlti)
        {
            if (!_isAltUlti)
            {
                //firedProjectile = Instantiate(normalUlti, spawnPoint.position + _distance, spawnPoint.rotation);
                firedProjectile = Instantiate(normalUlti, transform);
                firedProjectile.GetComponent<PlayerProjectileSettings>().SetOwnerProjectile(gameObject);
                firedProjectile.transform.SetParent(null, false);
                firedProjectile.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

                NetworkServer.Spawn(firedProjectile, connectionToClient);
            }
            else if (_isAltUlti)
            {
                firedProjectile = Instantiate(AltUlti, transform);
                firedProjectile.GetComponent<SelfBuffScript>().owner = this.gameObject;
                firedProjectile.transform.SetParent(null, false);
                firedProjectile.transform.SetPositionAndRotation(UltiAltSpawnpoint.position, UltiAltSpawnpoint.rotation);

                GetComponent<CharacterMovement>().changeSpeed("speedUp", 5, 1.5f);
                NetworkServer.Spawn(firedProjectile, connectionToClient);
            }
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
