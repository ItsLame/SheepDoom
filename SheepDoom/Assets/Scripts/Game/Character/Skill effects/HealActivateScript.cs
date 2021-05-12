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

        
        public void activateHeal()
        {
   //         Debug.Log("BoxCollider Activated?: " + this.gameObject.GetComponent<BoxCollider>().enabled);
            this.gameObject.GetComponent<BoxCollider>().enabled = true;
    //        Debug.Log("BoxCollider Activated?: " + this.gameObject.GetComponent<BoxCollider>().enabled);
            healActivated = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (healActivated)
            {
                if (other.gameObject.CompareTag("Player"))
                {
          //          Debug.Log("Player " + other.gameObject.name + " found");
                    Debug.Log("Heal Activated");

                    //heal ally in range
                    if (other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex() == teamID)
                    {
                        Debug.Log("Healing Player " + other.gameObject.name + " with teamID of " + teamID);
                        other.gameObject.GetComponent<PlayerHealth>().modifyinghealth(healAmount);
                    }

                    if (damagesEnemy)
                    {
                        //damage enemy in range
                        if (other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex() != teamID)
                        {
                            Debug.Log("Damaging player " + other.gameObject.name + " with teamID of " + teamID);

                            // increase killer's kill count if target is killed
                            // non lethal dmg
                            if (other.gameObject.GetComponent<PlayerHealth>().getHealth() < 5) return;
                            other.gameObject.GetComponent<PlayerHealth>().modifyinghealth(-healAmount);



                        }
                    }
 



                    //                    healActivated = false;
                }



            }
        }
    }
}

