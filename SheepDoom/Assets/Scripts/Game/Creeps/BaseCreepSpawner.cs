using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SheepDoom
{ 
    public class BaseCreepSpawner : MonoBehaviour
    {
        private float nextspawntime;

        [SerializeField]
        private GameObject Minionmelee;
        [SerializeField]
        private float spawndelay = 10;
        private int noofenemies;

        private void Update()
        {
            if (ShouldSpawn())
            {
                spawn();
            }
        }

        private void spawn()
        {
            nextspawntime = Time.time + spawndelay;
            Instantiate(Minionmelee, transform.position, transform.rotation);
        }

        private bool ShouldSpawn()
        {
            return Time.time >= nextspawntime;
        }
    }
}