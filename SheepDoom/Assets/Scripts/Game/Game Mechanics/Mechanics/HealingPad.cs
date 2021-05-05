using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class HealingPad : MonoBehaviour
    {
        //Rate of healing
        [SerializeField]
        private float HealRate = 0.2f;

        [ServerCallback]
        private void OnTriggerStay(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                if(gameObject.CompareTag("TeamCoaHealPad"))
                {
                    if((other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex() == 1) && !other.GetComponent<PlayerHealth>().GetHealthStatus()) // heal ally
                        other.GetComponent<PlayerHealth>().modifyinghealth(HealRate);

                    else if((other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex() == 2))
                    {
                        other.GetComponent<PlayerHealth>().modifyinghealth(-(HealRate * 2)); // damage enemy
                        if(other.GetComponent<PlayerHealth>().getHealth() <= 0)
                            other.GetComponent<PlayerHealth>().SetPlayerDead();
                    }   
                }
                else if (gameObject.CompareTag("TeamConHealPad"))
                {
                    if ((other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex() == 2) && !other.GetComponent<PlayerHealth>().GetHealthStatus())
                        other.GetComponent<PlayerHealth>().modifyinghealth(HealRate);

                    else if ((other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex() == 1))
                    {
                        other.GetComponent<PlayerHealth>().modifyinghealth(-(HealRate * 2));
                        if (other.GetComponent<PlayerHealth>().getHealth() <= 0)
                            other.GetComponent<PlayerHealth>().SetPlayerDead();
                    }  
                }
            }
            else if(other.CompareTag("BaseMinion"))
            {
                if (gameObject.CompareTag("TeamCoaHealPad") && other.gameObject.layer == 9) // damage enemy minion
                    other.GetComponent<LeftMinionBehaviour>().TakeDamage(-(HealRate * 2));
                else if (gameObject.CompareTag("TeamConHealPad") && other.gameObject.layer == 8)
                    other.GetComponent<LeftMinionBehaviour>().TakeDamage(-(HealRate * 2));
            }
        }
    }
}