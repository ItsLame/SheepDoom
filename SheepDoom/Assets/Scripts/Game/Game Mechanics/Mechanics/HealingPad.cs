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

        [Server]
        private void OnTriggerStay(Collider other)
        {
            //Debug.Log("inside healing pad");
            if (other.CompareTag("Player") && !other.gameObject.GetComponent<PlayerHealth>().GetHealthStatus())
            {
                if(gameObject.CompareTag("TeamCoaHealPad"))
                {
                    //if not full health
                    if (other.gameObject.layer == 8)
                        other.gameObject.GetComponent<PlayerHealth>().modifyinghealth(HealRate);
                }
                else if(gameObject.CompareTag("TeamConHealPad"))
                {
                    if (other.gameObject.layer == 9)
                        other.gameObject.GetComponent<PlayerHealth>().modifyinghealth(HealRate);
                }  
            }
        }
    }
}