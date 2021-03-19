using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPad : MonoBehaviour
{
    //Rate of healing
    [SerializeField]
    private float HealRate = 0.2f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    //for capture hp reduction when staying in area
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("inside healing pad");
        if (other.CompareTag("Player"))
        {
            //if not full health
            if (other.gameObject.GetComponent<PlayerHealth>().isFullHealth == false)
            {
                other.gameObject.GetComponent<PlayerHealth>().modifyinghealth(HealRate);
                Debug.Log("healing: " + HealRate * Time.deltaTime);
            }
        }
    }
}
