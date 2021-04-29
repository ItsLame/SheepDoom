using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MegaBossSpawner : NetworkBehaviour
{
    private float nextspawntime;

    [SerializeField]
    private GameObject Minionmelee;

    void Start()
    {
        StartCoroutine(TimeUntilSpawn());

    }
    IEnumerator TimeUntilSpawn()
    {
        yield return new WaitForSeconds(20);
        //Instantiate(Minionmelee, transform.position, transform.rotation);
        GameObject spawnMega = Instantiate(Minionmelee, transform);
        spawnMega.transform.SetParent(null, false);
        spawnMega.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
    }

}
