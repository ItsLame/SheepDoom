using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class BaseCreepSpawner : NetworkBehaviour
    {
        [SerializeField]private float nextspawntime = 30;

        [SerializeField] private bool firstSpawn = false;

        [SerializeField] private float teamID;
        [SerializeField] private GameObject Minion;
        [SerializeField] private GameObject ConvertedBoss;
        [SerializeField] private float spawndelay;
        [SerializeField] private GameObject SpawnPos;


        private void Update()
        {
            if(isServer)
            {
                if (firstSpawn)
                {
                    if (ShouldSpawn())
                        spawn();
                }

            }
        }

        public void setSpawn()
        {
            firstSpawn = true;
        }

        [Server]
        private void spawn()
        {
            nextspawntime = Time.time + spawndelay;
            GameObject creep = Instantiate(Minion, transform);
            creep.transform.SetParent(null, false);
            creep.transform.SetPositionAndRotation(SpawnPos.transform.position, SpawnPos.transform.rotation);
            NetworkServer.Spawn(creep);
        }

        [Server]
        public void spawnConvertedBoss()
        {
            GameObject boss = Instantiate(ConvertedBoss, SpawnPos.transform);
            boss.transform.SetParent(null, false);
            boss.transform.SetPositionAndRotation(SpawnPos.transform.position, SpawnPos.transform.rotation);
            NetworkServer.Spawn(boss);
        }

        private bool ShouldSpawn()
        {
            return Time.time >= nextspawntime;
        }
    }
}