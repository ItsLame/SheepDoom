using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralAggroScript : MonoBehaviour
{
    public GameObject Attacker;
    public bool LockedOn;
    public float moveSpd;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player" + other.gameObject.name + " spotted");
            Attacker = other.gameObject;
            LockedOn = true;

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && LockedOn)
        {
            if (other.gameObject != Attacker.gameObject) return;

            Debug.Log("Moving towards " + Attacker.gameObject.name);
            this.gameObject.transform.position += transform.forward * moveSpd * Time.deltaTime;
            transform.LookAt(Attacker.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && LockedOn)
        {
            Debug.Log("Stopped following " + Attacker.gameObject.name);
            Attacker = null;
            LockedOn = false;
        }
    }
}
