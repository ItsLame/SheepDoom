using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class OnTouchHealth : NetworkBehaviour
    {
        [Header("Amount of health to change on contact")]
        public float healthChangeAmount;

        [Header("If object will be destroyed on contact")]
        public bool destroyOnContact;

        //when collide with player
        private void OnTriggerEnter(Collider other)
        {
            if (isServer)
            {
                if (other.gameObject.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<PlayerHealth>().modifyinghealth(healthChangeAmount);
                    if (destroyOnContact)
                    {
                        Destroy(gameObject);
                    }

                }
            }

        }

        private void Start()
        {
            
        }
    }
}

