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

        public void setTeamID(float ID)
        {
            teamID = ID;
        }

        
        public void activateHeal()
        {
            Debug.Log("BoxCollider Activated?: " + this.gameObject.GetComponent<BoxCollider>().enabled);
            this.gameObject.GetComponent<BoxCollider>().enabled = true;
            Debug.Log("BoxCollider Activated?: " + this.gameObject.GetComponent<BoxCollider>().enabled);
            healActivated = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (healActivated)
            {
                if (other.gameObject.CompareTag("Player"))
                {
                    Debug.Log("Player " + other.gameObject.name + " found");
                    Debug.Log("Heal Activated");

                    if (other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex() == teamID)
                    {
                        Debug.Log("Healing Player " + other.gameObject.name + " with teamID of " + teamID);
                        other.gameObject.GetComponent<PlayerHealth>().modifyinghealth(healAmount);
                    }

//                    healActivated = false;
                }



            }
        }
    }
}

