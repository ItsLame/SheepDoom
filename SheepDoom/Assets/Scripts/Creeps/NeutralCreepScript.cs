using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralCreepScript : MonoBehaviour
{
    //boolean to check if under attack
    [SerializeField]
    private bool isAttacked = false;
    //gameobject to know who attacked it
    public GameObject player;

    [Space(15)]
    //creature starting location
    public GameObject StartingLocation;
    //attacking zone, by getting its child's collider 
   // public GameObject NeutralLeashZone;

    [Space(15)]
    //chasing range
    public float AttackRange;
    //movement speed
    public float MovementSpeed;

    //function to change the attacked bool
    public void isUnderAttack(GameObject playerWhoAttacked)
    {
        isAttacked = true;
        player = playerWhoAttacked;
        Debug.Log("Creature " + gameObject.name + " is under attack by " + player.gameObject.name);
    }

    //leash function, called by the attached zone's collider exit 
    public void BackToLocation()
    {
        //snap back for now
        this.gameObject.transform.position = StartingLocation.transform.position;
        //reset aggro
        isAttacked = false;
        //debug
        Debug.Log(gameObject.name + " stopped chasing " + player.name);

    }

    private void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if is under attack, chase player
        if (isAttacked)
        {
          //  if (Vector3.Distance(player.transform.position, transform.position) <= AttackRange)
          //  {
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, MovementSpeed * Time.deltaTime);
          //  }


        }    



    }
}
