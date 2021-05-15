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

        public override void OnStartServer()
        {
            //Destroy after set time
            Destroy(gameObject, AOE_Duration);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            //expand over time
            if (isServer)
                this.gameObject.transform.localScale = this.gameObject.transform.localScale * scale_speed;
        }

        [ServerCallback]
        private void OnTriggerStay(Collider col)
        {
            //if hit anything???
      //      Debug.Log(col.gameObject.name + " hit");

            //if hit player
            if (col.gameObject.CompareTag("Player"))
            {
      //          Debug.Log(col.gameObject.name + " player hit");
                //reduce HP of hit target
                col.gameObject.GetComponent<PlayerHealth>().modifyinghealth(-(damage * Time.deltaTime));

                //increase killer's kill count if target is killed
                if (col.gameObject.GetComponent<PlayerHealth>().getHealth() <= 0)
                {
                    col.gameObject.GetComponent<PlayerHealth>().SetPlayerDead();
                    col.gameObject.GetComponent<GameEvent>().isNeutral = true;
                }

                if (destroyOnContact)
                {
                    Object.Destroy(this.gameObject);
                }

            }
        }

    }

}