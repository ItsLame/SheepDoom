using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class HealActivateScript : NetworkBehaviour
    {
        [Header("Amount to heal")]
        public float healAmount;
        private bool healActivated = false;
        private float teamID;

        [Header("If damages enemy over time, tick")]
        [SerializeField] private bool damagesEnemy;
        public void setTeamID(float ID)
        {
            teamID = ID;
        }

        [Server]
        public void activateHeal()
        {
            this.gameObject.GetComponent<BoxCollider>().enabled = true;
            healActivated = true;
        }

        [ServerCallback]
        private void OnTriggerStay(Collider other)
        {
            if (healActivated)
            {
                if (other.gameObject.CompareTag("Player"))
                {
                    //heal ally in range
                    if (other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex() == teamID)
                        other.gameObject.GetComponent<PlayerHealth>().modifyinghealth(healAmount * Time.deltaTime);

                    if (damagesEnemy)
                    {
                        //damage enemy in range
                        if (other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex() != teamID)
                        {
                            // increase killer's kill count if target is killed
                            // non lethal dmg
                            if (other.gameObject.GetComponent<PlayerHealth>().getHealth() < 5) return;

                            other.gameObject.GetComponent<PlayerHealth>().modifyinghealth(-healAmount * Time.deltaTime);
                        }
                    }
                }
            }
        }
    }
}

