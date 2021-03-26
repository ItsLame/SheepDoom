using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaBossSpawner : MonoBehaviour
{
    private float nextspawntime;

    [SerializeField]
    private GameObject Minionmelee;

    void Start()
    {
        Instantiate(Minionmelee, transform.position, transform.rotation);
    }

}
