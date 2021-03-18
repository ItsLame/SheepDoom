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
        {
            transform.LookAt(playerWhoIsLookedAt.transform);
        }

    }

    /*
    //look at player when in range
    private void OnTriggerStay(Collider other)
    {
        //if (isLocalPlayer)
        if (other.CompareTag("Player"))
        {
            transform.LookAt(other.transform);
        }

    } */

    private void OnTriggerEnter(Collider other)
    {
        //if (isLocalPlayer)
        if (other.CompareTag("Player")) 
        {
            playerInView = true;
            playerWhoIsLookedAt = other.gameObject;
     //       transform.LookAt(other.transform);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInView = false;
            playerWhoIsLookedAt = null;
            //       transform.LookAt(other.transform);
        }
    }

}
