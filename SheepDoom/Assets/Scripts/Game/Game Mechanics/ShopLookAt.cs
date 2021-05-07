using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class ShopLookAt : MonoBehaviour
    {
        //bool for playerinview
        bool playerInView;
        public GameObject playerWhoIsLookedAt;

        private void Update()
        {
            if (playerInView)
                transform.LookAt(playerWhoIsLookedAt.transform);
        }

        [ClientCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInView = true;
                playerWhoIsLookedAt = other.gameObject;
            }
        }

        [ClientCallback]
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInView = false;
                playerWhoIsLookedAt = null;
            }
        }
    }
}
