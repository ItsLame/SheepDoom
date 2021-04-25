using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class OnTouchHealth : NetworkBehaviour
    {
        [Header("Amount of health to change on contact")]
        public float healthChangeAmount;

        [Header("If object will be destroyed on contact")]
        public bool destroyOnContact;

        [Header("What does it interact with")]
        public bool willContactPlayer;
        public bool willContactMinion;

        [Header("When to interact with bool")]
        public bool hitboxActive;

        [Header("If is child, parent's details here")]
        public bool hasParent;
        public GameObject parent;
        public float parentTeamID;

        //when collide with player
        [Server]
        private void OnTriggerEnter(Collider col)
        {
            if (hitboxActive)
            {
   //             Debug.Log("Contacted With " + col.gameObject.name);
                if (willContactPlayer)
                {
                    //if hit other player
                    if (col.gameObject.CompareTag("Player"))
                    {
                        Debug.Log("Player Hit");
                        //change the hit player's HP
                        col.gameObject.GetComponent<PlayerHealth>().modifyinghealth(healthChangeAmount);

                        //kill target if target hp <= 0
                        if (col.gameObject.GetComponent<PlayerHealth>().getHealth() <= 0)
                        {
                            //set hit target to dead
                            col.gameObject.GetComponent<PlayerHealth>().SetPlayerDead();

                            //get the brick's parent (the attacker)
                            GameObject target = this.gameObject.GetComponent<GetParents>().getParent();
                            //increase the attacker's score
                            target.GetComponent<PlayerAdmin>().IncreaseCount(false, true, false);

                            //give announcer info
                            col.gameObject.GetComponent<GameEvent>().whoKilled = target.gameObject.GetComponent<PlayerObj>().GetPlayerName();
                        }
                        if (destroyOnContact)
                        {
                            Destroy(gameObject);
                        }

                    }
                }

                if (willContactMinion)
                {
                    if (col.gameObject.CompareTag("BaseMinion"))
                    {
                        Debug.Log("Base Minion Hit");
                        
                        if (parentTeamID == 2)
                        {
                            if (col.gameObject.layer == 8)
                            {
                                Debug.Log("Coalation Minion Hit");
                                GameObject target = col.gameObject.GetComponent<GetParents>().getParent();
                                target.gameObject.GetComponent<LeftMinionBehaviour>().TakeDamage(healthChangeAmount);

                                if (target.gameObject.GetComponent<LeftMinionBehaviour>().getHealth() <= 0)
                                {
                                    GameObject parent = this.gameObject.GetComponent<GetParents>().getParent();
                                    parent.gameObject.GetComponent<CharacterGold>().CmdVaryGold(5);
                                }
                            }
                        }

                        else if (parentTeamID == 1)
                        {
                            if (col.gameObject.layer == 9)
                            {
                                Debug.Log("Consortium Minion Hit");
                                GameObject target = col.gameObject.GetComponent<GetParents>().getParent();
                                target.gameObject.GetComponent<TeamConsortiumLeftMinionBehaviour>().TakeDamage(healthChangeAmount);
                                if (target.gameObject.GetComponent<TeamConsortiumLeftMinionBehaviour>().getHealth() <= 0)
                                {
                                    GameObject parent = this.gameObject.GetComponent<GetParents>().getParent();
                                    parent.gameObject.GetComponent<CharacterGold>().CmdVaryGold(5);
                                }
                            }
                        }


                        if (destroyOnContact)
                        {
                            Object.Destroy(this.gameObject);
                        }

                    }
                }

                //used to test gold for now
                if (col.gameObject.CompareTag("NeutralMinion"))
                {
                    GameObject parent = this.gameObject.GetComponent<GetParents>().getParent();
                    parent.gameObject.GetComponent<CharacterGold>().CmdVaryGold(5);

                    if (destroyOnContact)
                    {
                        Object.Destroy(this.gameObject);
                    }

                }
            }




        }
        private void Start()
        {
            if (hasParent)
            {
                parentTeamID = parent.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
            }
        }
    }
}





