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
            healActivated = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (healActivated)
            {
                Debug.Log("Heal Activated");
                if (other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex() == teamID)
                {
                    Debug.Log("Healing Player " + other.gameObject.name);
                    other.gameObject.GetComponent<PlayerHealth>().modifyinghealth(healAmount);
                }

                healActivated = false;
            }
        }
    }
}

