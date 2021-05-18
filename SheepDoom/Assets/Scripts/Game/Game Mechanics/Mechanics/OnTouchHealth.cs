using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class OnTouchHealth : MonoBehaviour
    {
        [Header("Amount of health to change on contact")]
        [SerializeField]
        private float healthChangeAmount;

        [Header("If object will be destroyed on contact")]
        public bool destroyOnContact;

        //when collide with player
        [ServerCallback]
        private void OnTriggerEnter(Collider col)
        {
            //if (hitboxActive)
            //{
   //             Debug.Log("Contacted With " + col.gameObject.name);
                //if (willContactPlayer)
                //{
            //if hit other player
            if (col.CompareTag("Player"))
            {
                //change the hit player's HP
                col.GetComponent<PlayerHealth>().modifyinghealth(healthChangeAmount);

                if (destroyOnContact)
                    Destroyy();
            }
                //} 
            //}
        }

        [Server]
        private void Destroyy()
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}


//kill target if target hp <= 0
/*
 * //[Header("What does it interact with")]
        //public bool willContactPlayer;
        //public bool willContactMinion;

        //[Header("When to interact with bool")]
        //[SerializeField]
        //private bool hitboxActive;

        //[Header("If is child, parent's details here")]
        //[SerializeField]
        //private bool hasParent;
        //public GameObject parent;
        //[SerializeField]
        //private float parentTeamID;
 * if (col.GetComponent<PlayerHealth>().getHealth() <= 0)
{
    //set hit target to dead
    col.GetComponent<PlayerHealth>().SetPlayerDead();

    //get the brick's parent (the attacker)
    GameObject target = this.gameObject.GetComponent<GetParents>().getParent();
    //increase the attacker's score
    target.GetComponent<PlayerAdmin>().IncreaseCount(false, true, false);

    //give announcer info
    col.GetComponent<GameEvent>().whoKilled = target.GetComponent<PlayerObj>().GetPlayerName();
}
if (willContactMinion)
                {
                    if (col.gameObject.CompareTag("BaseMinion"))
                    {
                    //    Debug.Log("Base Minion Hit");
                        
                        if (parentTeamID == 2)
                        {
                            if (col.gameObject.layer == 8)
                            {
                    //            Debug.Log("Coalation Minion Hit");
                                GameObject target = col.gameObject.GetComponent<GetParents>().getParent();
                                target.GetComponent<LeftMinionBehaviour>().TakeDamage(healthChangeAmount);

                                if (target.GetComponent<LeftMinionBehaviour>().getHealth() <= 0)
                                {
                                    GameObject parent = this.gameObject.GetComponent<GetParents>().getParent();
                                    parent.GetComponent<CharacterGold>().CmdVaryGold(5);
                                }
                            }
                        }
                        else if (parentTeamID == 1)
                        {
                            if (col.gameObject.layer == 9)
                            {
                  //              Debug.Log("Consortium Minion Hit");
                                GameObject target = col.gameObject.GetComponent<GetParents>().getParent();
                                target.GetComponent<LeftMinionBehaviour>().TakeDamage(healthChangeAmount);
                                if (target.GetComponent<LeftMinionBehaviour>().getHealth() <= 0)
                                {
                                    GameObject parent = this.gameObject.GetComponent<GetParents>().getParent();
                                    parent.GetComponent<CharacterGold>().CmdVaryGold(5);
                                }
                            }
                        }

                        if (destroyOnContact)
                            Destroyy();
                    }
                }
void Update()
        {   
            if(hasParent)
            {
                if (parentTeamID == 0)
                    parentTeamID = parent.GetComponent<PlayerAdmin>().getTeamIndex();
            }
        }*/


