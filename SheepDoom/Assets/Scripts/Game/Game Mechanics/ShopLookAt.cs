using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            playerInView = true;
            playerWhoIsLookedAt = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInView = false;
            playerWhoIsLookedAt = null;
        }
    }

}
