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
        private GameObject normalAtkProjectile, normalSpecial, altSpecial;
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
            firedProjectile = Instantiate(normalAtkProjectile, spawnPoint.position, spawnPoint.rotation);
            firedProjectile.GetComponent<PlayerProjectileSettings>().SetOwnerProjectile(gameObject);
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
                firedProjectile = Instantiate(normalSpecial, spawnPoint.position, spawnPoint.rotation);
            else if(_isAltSpecial)
                firedProjectile = Instantiate(altSpecial, spawnPoint.position, spawnPoint.rotation);
            firedProjectile.GetComponent<PlayerProjectileSettings>().SetOwnerProjectile(gameObject);
            NetworkServer.Spawn(firedProjectile, connectionToClient);
        }
    }
}
