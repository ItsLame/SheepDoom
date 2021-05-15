using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NeutralCreepScript : MonoBehaviour
{
    //boolean to check if under attack
    [SerializeField]
    private bool isAttacked = false;
    //gameobject to know who attacked it
    public GameObject Attacker;
    //transform of player who attacked it
    Transform CharacterWhoAttacked_Transform;

    [Space(15)]
    //creature starting location
    public GameObject StartingLocation;
    //attacking zone, by getting its child's collider 
   // public GameObject NeutralLeashZone;

    //stats
    [Space(15)]

    //chasing range
    public float AttackRange;
    //movement speed
    public float MovementSpeed;
    //neutral creep HP
    public float NeutralHP;
    public float NeutralHPInGame;
    //how much gold its worth
    public float goldValue;

//    public event Action<float> OnHealthPctChanged = delegate { };

    //function to change the attacked bool
    public void isUnderAttack(/*GameObject playerWhoAttacked*/)
    {
        isAttacked = true;
       // Attacker = playerWhoAttacked;
   //     Debug.Log("Creature " + gameObject.name + " is under attack by " + Attacker.gameObject.name);
    }

    public void neutralTakeDamage(float damageAmount)
    {
        NeutralHPInGame += damageAmount;

     //   float currenthealthPct = (float)NeutralHPInGame / (float)NeutralHP;
   //     OnHealthPctChanged(currenthealthPct);
    }

    //leash function, called by the attached zone's collider exit 
    public void BackToLocation()
    {

        //snap back for now
   //     this.gameObject.transform.position = StartingLocation.transform.position;
        //reset aggro
        isAttacked = false;
        //debug
    //    Debug.Log(gameObject.name + " stopped chasing " + Attacker.name);
        //ResetHP
        NeutralHPInGame = NeutralHP;
     //   float currenthealthPct = (float)NeutralHPInGame / (float)NeutralHP;
    //    OnHealthPctChanged(currenthealthPct);


    }

    private void Start()
    {
        //set hp at start
        NeutralHPInGame = NeutralHP;
    }

    // Update is called once per frame
    void Update()
    {
        //if dead
        if (NeutralHPInGame <= 0)
        {
            //inform the spawner that it died
 //           StartingLocation.GetComponent<NeutralCreepSpawner>().neutralIsKilled();
          //  Attacker.GetComponent<CharacterGold>().varyGold(goldValue);
            Destroy(this.gameObject);
        }

        //if is under attack, chase player
        if (isAttacked)
        {
            /*
           if (Vector3.Distance(Attacker.transform.position, transform.position) <= AttackRange)
              {
                     Debug.Log("Moving Towards " + CharacterWhoAttacked_Transform);
                     CharacterWhoAttacked_Transform = Attacker.transform;
                     transform.position = Vector3.MoveTowards(transform.position, CharacterWhoAttacked_Transform.position, MovementSpeed * Time.deltaTime);
            }*/

           // Debug.Log(Vector3.Distance(Attacker.transform.position, transform.position));
            //stop at a certain distance from the player
            if (Vector3.Distance(Attacker.transform.position, transform.position) >= AttackRange)
            {
                this.gameObject.transform.position += transform.forward * 30 * Time.deltaTime;
                transform.LookAt(Attacker.transform);
            }


        }    



    }
}
