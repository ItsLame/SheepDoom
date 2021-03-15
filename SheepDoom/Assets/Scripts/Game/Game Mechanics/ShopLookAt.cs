using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopLookAt : MonoBehaviour
{
    //look at player when in range
    private void OnTriggerStay(Collider other)
    {
        //if (isLocalPlayer)
        if (other.CompareTag("Player"))
        {
            transform.LookAt(other.transform);
        }

    }

}
