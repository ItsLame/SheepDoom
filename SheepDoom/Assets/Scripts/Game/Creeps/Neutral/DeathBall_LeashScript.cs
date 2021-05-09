using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class DeathBall_LeashScript : NetworkBehaviour
    {
        //transform of where the original spot is to be 
        //    [SerializeField] private Transform oriSpot;

        [ServerCallback]
        private void OnTriggerExit(Collider other)
        {
            //      Debug.Log(other.gameObject.name + (" has left the zone"));
            if (other.gameObject.CompareTag("DeathBall"))
            {
                other.gameObject.GetComponent<leashScript>().backToStart();
                Debug.Log("Death Ball is going back to original place");
            }
        }
    }
}

