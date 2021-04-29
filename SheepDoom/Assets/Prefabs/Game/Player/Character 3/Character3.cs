using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class Character3 : NetworkBehaviour
    {
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
                CmdUltiAtk(_isAltUlti);

        }

        [Command]
        void CmdUltiAtk(bool _isAltUlti)
        {
            Debug.Log("CmdUltiAtk-ing");
            if (!_isAltUlti)
                firedProjectile = Instantiate(normalUlti, transform);
            else if (_isAltUlti)
                firedProjectile = Instantiate(altUlti, transform);
            
            firedProjectile.GetComponent<ProjectileHealScript>().SetOwnerProjectile(gameObject);
            firedProjectile.transform.SetParent(null, false);
            firedProjectile.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            
            NetworkServer.Spawn(firedProjectile, connectionToClient);

        }

        /*[Command]
        void CmdUltiAtk(bool _isAltUlti)
        {
            Debug.Log("Ulti pressed");
            // Has a casting time before its instantiated
            if (!_isAltUlti)
            {
                startCasting();

                if (isCastingComplete)
                {
                    fireCastedSkill("ulti");
                    isCastingComplete = false;
                }

            }

            else if (_isAltUlti)
            {
                firedProjectile = Instantiate(altUlti, spawnPoint.position, spawnPoint.rotation);
                firedProjectile.GetComponent<ProjectileHealScript>().SetOwnerProjectile(gameObject);
                NetworkServer.Spawn(firedProjectile, connectionToClient);
            }


        }*/

        //        [Command]
        /* void fireCastedSkill(string skillID)
         {
             if (skillID == "ulti1")
             {
                 firedProjectile = Instantiate(normalUlti, spawnPoint.position, spawnPoint.rotation);
                 firedProjectile.GetComponent<ProjectileHealScript>().SetOwnerProjectile(gameObject);
                 NetworkServer.Spawn(firedProjectile, connectionToClient);
                 isCastingComplete = false;
             }

         }*/

        private void Update()
        {
            if (isClient && hasAuthority)
            {
                //start the castimg
                if (isCasting)
                {
                    Debug.Log("Casting......");
                    castTimeInGame -= Time.deltaTime;
                    Debug.Log("Cast time left: " + castTimeInGame);

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
            }



            //    }
        }
    }

}