using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{

    public class NeutralAOEAttackScript : NetworkBehaviour
    {
        [Header("Rate of AOE Growth")]
        public float scale_speed;

        [Header("AOE duration")]
        public float AOE_Duration;

        [Header("Will AOE destroy on contact")]
        public bool destroyOnContact;

        [Header("How much damage it does")]
        public float damage;

        // Start is called before the first frame update
        void Start()
        {
            //Destroy after set time
            Destroy(gameObject, AOE_Duration);
        }

        // Update is called once per frame
        void Update()
        {
            //expand over time
            this.gameObject.transform.localScale = this.gameObject.transform.localScale * scale_speed;
        }

        [Server]
        void OnTriggerEnter(Collider col)
        {
            //if hit player
            if (col.gameObject.CompareTag("Player"))
            {

                //reduce HP of hit target
                col.gameObject.GetComponent<PlayerHealth>().modifyinghealth(-damage);

                //increase killer's kill count if target is killed
                if (col.gameObject.GetComponent<PlayerHealth>().getHealth() <= 0)
                {
                    col.gameObject.GetComponent<PlayerHealth>().SetPlayerDead();
                }

                if (destroyOnContact)
                {
                    Object.Destroy(this.gameObject);
                }

            }
        }

    }

}