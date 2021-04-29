using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class Character1 : NetworkBehaviour
    {
        // Attack/skill related prefabs
        [SerializeField]
        private GameObject normalAtkProjectile, normalSpecial, altSpecial, normalUlti, altUlti;
        private GameObject firedProjectile;

        [SerializeField]
        private Transform spawnPoint;


        [Client]
        public void normalAtk()
        {
            CmdNormalAtk();
        }

        [Command]
        void CmdNormalAtk()
        {
            //firedProjectile = Instantiate(normalAtkProjectile, spawnPoint.position, spawnPoint.rotation);
            //firedProjectile.GetComponent<PlayerProjectileSettings>().SetOwnerProjectile(gameObject);
            
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
            if(!_isAltSpecial)
                //firedProjectile = Instantiate(normalSpecial, spawnPoint.position, spawnPoint.rotation);
                firedProjectile = Instantiate(normalSpecial, transform);
            else if(_isAltSpecial)
                //firedProjectile = Instantiate(altSpecial, spawnPoint.position, spawnPoint.rotation);
                firedProjectile = Instantiate(altSpecial, transform);
            
            firedProjectile.GetComponent<PlayerProjectileSettings>().SetOwnerProjectile(gameObject);
            firedProjectile.transform.SetParent(null, false);
            firedProjectile.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            
            NetworkServer.Spawn(firedProjectile, connectionToClient);
        }

        [Client]
        public void UltiAtk(bool _isAltUlti)
        {
            CmdUltiAtk(_isAltUlti);
        }

        [Command]
        void CmdUltiAtk(bool _isAltUlti)
        {
            if(!_isAltUlti)
                //firedProjectile = Instantiate(normalUlti, spawnPoint.position, spawnPoint.rotation);
                firedProjectile = Instantiate(normalUlti, transform);
            else if(_isAltUlti)
                //firedProjectile = Instantiate(altUlti, spawnPoint.position, spawnPoint.rotation);
                firedProjectile = Instantiate(altUlti, transform);
            
            firedProjectile.GetComponent<PlayerProjectileSettings>().SetOwnerProjectile(gameObject);
            firedProjectile.transform.SetParent(null, false);
            firedProjectile.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            
            NetworkServer.Spawn(firedProjectile, connectionToClient);
        }
    }
}
