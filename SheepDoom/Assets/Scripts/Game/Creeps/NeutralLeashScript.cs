using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script used to leash the attached object back to original location
public class NeutralLeashScript : MonoBehaviour
{
    //the attached gameobject
   // public GameObject CreatureToBeAssigned;

    //leash function, using the attached zone's collider exit 
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NeutralMinion"))
        {
            Debug.Log("Neutral Creep " + other.name + " has exited zone");
            other.gameObject.GetComponent<NeutralCreepScript>().BackToLocation();
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player " + other.name + " has exited zone");
        }
    }
}
