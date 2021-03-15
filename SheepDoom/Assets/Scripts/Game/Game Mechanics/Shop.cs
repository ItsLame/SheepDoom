using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //look at player when in range
    private void OnTriggerStay(Collider other)
    {
        //if (isLocalPlayer)
        if (other.tag == "Player")
        {
            transform.LookAt(other.transform);
        }

    }
}
