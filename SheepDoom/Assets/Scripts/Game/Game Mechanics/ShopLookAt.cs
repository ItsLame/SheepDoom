using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

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

    [Client]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            playerInView = true;
            playerWhoIsLookedAt = other.gameObject;
        }
    }

    [Client]
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInView = false;
            playerWhoIsLookedAt = null;
        }
    }

}
