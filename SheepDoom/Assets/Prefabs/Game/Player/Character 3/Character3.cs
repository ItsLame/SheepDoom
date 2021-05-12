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
        private Transform spawnPoint;

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


        [Client]
        public void normalAtk()
        {
            CmdNormalAtk();
        }

        [Command]
        void CmdNormalAtk()
        {
            //firedProjectile = Instantiate(normalAtkProjectile, spawnPoint.position, spawnPoint.rotation);

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
                //firedProjectile = Instantiate(normalSpecial, spawnPoint.position, spawnPoint.rotation);
                firedProjectile = Instantiate(normalSpecial, transform);
            else if (_isAltSpecial)
                //firedProjectile = Instantiate(altSpecial, spawnPoint.position, spawnPoint.rotation);
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
            Debug.Log("Start Casting");
            castTimeInGame = castTime;

            //set transform
            lastPos = transform.position;
        }

        [Client]
        void startChanneling()
        {
            isChanneling = true;
            CmdUltiAtk(true);
            Debug.Log("Start Channeling");

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
                Debug.Log("isCastingComplete 1: " + isCastingComplete);

                /*
                if (isCastingComplete)
                {
                    Debug.Log("isCastingComplete 2: " + isCastingComplete);
                    CmdUltiAtk(_isAltUlti);
                    isCastingComplete = false;
                }*/
            }
            else if (_isAltUlti)
                startChanneling();


        }

        [Command]
        void CmdUltiAtk(bool _isAltUlti)
        {
            Debug.Log("CmdUltiAtk-ing");
            if (!_isAltUlti)
            {
                firedProjectile = Instantiate(normalUlti, transform);
                firedProjectile.GetComponent<ProjectileHealScript>().SetOwnerProjectile(gameObject);
                firedProjectile.transform.SetParent(null, false);
                firedProjectile.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

                NetworkServer.Spawn(firedProjectile, connectionToClient);
            }

            else if (_isAltUlti)
            {
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
                    Debug.Log("Casting......");
                    castTimeInGame -= Time.deltaTime;
                    Debug.Log("Cast time left: " + castTimeInGame);

                    //check for movement
                    if (dist > 5)
                    {
                        Debug.Log("Player Moved, stopping incantation");
                        isCasting = false;
                    }

                }

                //when casting is complete
                if (isCasting && castTimeInGame <= 0)
                {
                    Debug.Log("Casting complete");
                    isCastingComplete = true;
                    isCasting = false;
                }


                if (isCastingComplete)
                {
                    Debug.Log("isCastingComplete 2: " + isCastingComplete);
                    CmdUltiAtk(false);
                    isCastingComplete = false;
                }

                //if interrupted by cc (stun)
                if (isCasting && gameObject.GetComponent<CharacterMovement>().isStopped)
                {
                    Debug.Log("Casting failed");
                    isCasting = false;
                }

                //if interrupted by cc (sleep)
                if (isCasting && gameObject.GetComponent<CharacterMovement>().isSleeped)
                {
                    Debug.Log("Casting failed");
                    isCasting = false;
                }

                // ================================ calculation for channeling timers ===================================
                if (isChanneling)
                {
                    dist = Vector3.Distance(lastPos, transform.position);
                    Debug.Log("Channeling ult 2...");
                    channelTimeInGame -= Time.deltaTime;
                    Debug.Log("Channeling time left: " + channelTimeInGame);

                    //check for movement
                    if (dist > 5)
                    {
                        Debug.Log("Player Moved, stopping channeling");
                        isChanneling = false;
                    }
                }

                //end channeling when time is up
                if (isChanneling && channelTimeInGame <= 0)
                {
 //                   Destroyy(firedProjectile);
                    isChanneling = false;
                }

                //if interrupted by cc (stun)
                if (isChanneling && gameObject.GetComponent<CharacterMovement>().isStopped)
                {
                    Debug.Log("Channeling stopped");
                    isChanneling = false;
                }

                //if interrupted by cc (sleep)
                if (isChanneling && gameObject.GetComponent<CharacterMovement>().isSleeped)
                {
                    Debug.Log("Channeling stopped");
                    isChanneling = false;
                }

            }



            //    }
        }

    }

}