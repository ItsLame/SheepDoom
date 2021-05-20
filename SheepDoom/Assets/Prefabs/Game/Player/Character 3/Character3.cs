using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class Character3 : NetworkBehaviour
    {
        //debugging
        public float dist;
        // Attack/skill related prefabs
        [SerializeField]
        private GameObject normalAtkProjectile, normalSpecial, altSpecial, normalUlti, altUlti;
        private GameObject firedProjectile;

        [SerializeField]
        private Transform spawnPoint, aoespawnPoint;

        [Header("Cast timer calculations")]
        [SerializeField]
        private bool isCasting, isCastingComplete;
        [SerializeField]
        private float castTime;
        private float castTimeInGame = 0;

        [Header("Channeling timer calculations")]
        [SerializeField] private bool isChanneling;
        [SerializeField] private bool isChannelingComplete;
        [SerializeField] private float channelTime;
        private float channelTimeInGame = 0;

        [Header("Transform checks for movement for casting & Channeling")]
        [SerializeField] private Vector3 lastPos;

        //Animation
        [SerializeField] public NetworkAnimator networkAnimator;
        [SerializeField] private Animator animator;


        [Client]
        public void normalAtk()
        {
            networkAnimator.SetTrigger("IsabellaAttack");
            CmdNormalAtk();
        }

        [Command]
        void CmdNormalAtk()
        {
            firedProjectile = Instantiate(normalAtkProjectile, transform);
            firedProjectile.GetComponent<PlayerProjectileSettings>().SetOwnerProjectile(gameObject);
            firedProjectile.transform.SetParent(null, false);
            firedProjectile.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

            NetworkServer.Spawn(firedProjectile, connectionToClient);
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
                firedProjectile = Instantiate(normalSpecial, transform);
            else if (_isAltSpecial)
                firedProjectile = Instantiate(altSpecial, transform);

            firedProjectile.GetComponent<ProjectileHealScript>().SetOwnerProjectile(gameObject);
            firedProjectile.transform.SetParent(null, false);
            firedProjectile.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

            NetworkServer.Spawn(firedProjectile, connectionToClient);
        }

        [Client]
        void startCasting()
        {
            isCasting = true;
            networkAnimator.SetTrigger("Channeling");
            castTimeInGame = castTime;

            //set transform
            lastPos = transform.position;
        }

        [Client]
        void startChanneling()
        {
            isChanneling = true;
            CmdUltiAtk(true);
            networkAnimator.SetTrigger("Channeling");

            //set transform
            lastPos = transform.position;

            //set timer
            channelTimeInGame = channelTime;
        }

        [Client]
        public void UltiAtk(bool _isAltUlti)
        {
            if (!_isAltUlti)
            {
                startCasting();
            }
            else if (_isAltUlti)
                startChanneling();


        }

        [Command]
        void CmdUltiAtk(bool _isAltUlti)
        {
            if (!_isAltUlti)
            {
                firedProjectile = Instantiate(normalUlti, transform);
                firedProjectile.GetComponent<ProjectileHealScript>().SetOwnerProjectile(gameObject);
                firedProjectile.transform.SetParent(null, false);
                firedProjectile.transform.SetPositionAndRotation(aoespawnPoint.position, aoespawnPoint.rotation);

                NetworkServer.Spawn(firedProjectile, connectionToClient);
            }

            else if (_isAltUlti)
            {
                firedProjectile = Instantiate(altUlti, transform);
                firedProjectile.GetComponent<ProjectileHealScript>().SetOwnerProjectile(gameObject);
                firedProjectile.transform.SetParent(null, false);
                firedProjectile.transform.SetPositionAndRotation(aoespawnPoint.position, aoespawnPoint.rotation);
                //setting channeling conditions (if owner moves, destroy)
                firedProjectile.GetComponent<ChannelingScript>().setOwner(this.gameObject);

                //setting healing over time conditions (heal allies, damage enemies)
                firedProjectile.GetComponent<HealActivateScript>().setTeamID(this.gameObject.GetComponent<PlayerAdmin>().getTeamIndex());
                firedProjectile.GetComponent<HealActivateScript>().activateHeal();


                NetworkServer.Spawn(firedProjectile, connectionToClient);
            }

        }


        private void Update()
        {
            if (isClient && hasAuthority)
            {
                // ======================================== for casting timer calculations ================================== 
                //start the casting 
                if (isCasting)
                {
                    dist = Vector3.Distance(lastPos, transform.position);
                    castTimeInGame -= Time.deltaTime;

                    if (dist > 1)
                    {
                        networkAnimator.SetTrigger("CastCancelToRun");
                        isCasting = false;

                    }

                }

                //when casting is complete
                if (isCasting && castTimeInGame <= 0)
                {
                    networkAnimator.SetTrigger("CastCancel");
                    isCastingComplete = true;
                    isCasting = false;

                }
                if (isCastingComplete)
                {
                    CmdUltiAtk(false);
                    isCastingComplete = false;
                }

                //if interrupted by cc (stun)
                if (isCasting && gameObject.GetComponent<CharacterMovement>().isStopped)
                {
                    networkAnimator.SetTrigger("CastCancel");
                    isCasting = false;
                }

                //if interrupted by cc (sleep)
                if (isCasting && gameObject.GetComponent<CharacterMovement>().isSleeped)
                {
                    networkAnimator.SetTrigger("CastCancel");
                    isCasting = false;
                }

                // ================================ calculation for channeling timers ===================================
                if (isChanneling)
                {
                    dist = Vector3.Distance(lastPos, transform.position);
                    channelTimeInGame -= Time.deltaTime;
                    if (dist > 5)
                    {
                        networkAnimator.SetTrigger("CastCancelToRun");
                        isChanneling = false;
                    }
                }

                //end channeling when time is up
                if (isChanneling && channelTimeInGame <= 0)
                {
                    //Destroyy(firedProjectile);
                    isChanneling = false;
                    networkAnimator.SetTrigger("CastCancel");
                }

                //if interrupted by cc (stun)
                if (isChanneling && gameObject.GetComponent<CharacterMovement>().isStopped)
                {
                    networkAnimator.SetTrigger("CastCancel");
                    isChanneling = false;
                }

                //if interrupted by cc (sleep)
                if (isChanneling && gameObject.GetComponent<CharacterMovement>().isSleeped)
                {
                    networkAnimator.SetTrigger("CastCancel");
                    isChanneling = false;
                }
            }
        }
    }
}