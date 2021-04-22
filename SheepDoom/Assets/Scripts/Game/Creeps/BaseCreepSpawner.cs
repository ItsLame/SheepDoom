using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class BaseCreepSpawner : NetworkBehaviour
    {
        private float nextspawntime;

        [SerializeField]
        private GameObject Minion;
        [SerializeField]
        private float spawndelay = 10;

        private void Update()
        {
            if (ShouldSpawn())
            {
                spawn();
            }
        }
        [Server]
        private void spawn()
        {
            nextspawntime = Time.time + spawndelay;
            GameObject creep = (GameObject)Instantiate(Minion, transform.position, transform.rotation);

            NetworkServer.Spawn(creep);
        }

        private bool ShouldSpawn()
        {
            return Time.time >= nextspawntime;
        }
    }
}