using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralCreepSpawner : MonoBehaviour
{
    //respawn time, + unit u want to spawn
    public float spawnTimer;
    public float spawnTimerInGame;
    public GameObject NeutralUnitToSpawn;
    //to check if neutral is killed
    public bool isNeutralKilled;


    // Start is called before the first frame update
    void Start()
    {
        //spawn, so false
        isNeutralKilled = false;
        //spawn once
        spawnNeutral();
        //reset timer
        spawnTimerInGame = spawnTimer;   
    }

    // Update is called once per frame
    void Update()
    {
        //countdown only if neutral is killed
        if (isNeutralKilled)
        {
            spawnTimerInGame -= Time.deltaTime;
        }


        //if respawn time is up and neutral is killed
        if (spawnTimerInGame <= 0 && isNeutralKilled)
        {
   //         Debug.Log("Neutral respawning");
            spawnNeutral();
            spawnTimerInGame = spawnTimer;
            isNeutralKilled = false;
        }
    }

    //spawn function
    public void spawnNeutral()
    {
        //Instantiate(NeutralUnitToSpawn, transform.position, transform.rotation);
        GameObject spawnNeut = Instantiate(NeutralUnitToSpawn, transform);
        spawnNeut.transform.SetParent(null, false);
        spawnNeut.transform.SetPositionAndRotation(transform.position, transform.rotation);
    }

    //notify that neutral is killed function
    public void neutralIsKilled()
    {
        isNeutralKilled = true;
    }
}
