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
            if (other.CompareTag("Player"))
            {
                //if not full health
                if (!other.gameObject.GetComponent<PlayerHealth>().GetHealthStatus())
                    other.gameObject.GetComponent<PlayerHealth>().modifyinghealth(HealRate);
            }
        }
    }
}