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

        //when collide with player
        [Server]
        private void OnTriggerEnter(Collider col)
        {
            if (hitboxActive)
            {
                if (willContactPlayer)
                {
                    if (col.gameObject.CompareTag("Player"))
                    {
                        col.gameObject.GetComponent<PlayerHealth>().modifyinghealth(healthChangeAmount);
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
                        if (col.gameObject.layer == 8)
                        {
                            Debug.Log("Coalation Minion Hit");
                            GameObject target = col.gameObject.GetComponent<GetParents>().getParent();
                            target.gameObject.GetComponent<TeamCoalitionLeftMinionBehaviour>().TakeDamage(-healthChangeAmount);
                            if (target.gameObject.GetComponent<TeamCoalitionLeftMinionBehaviour>().getHealth() <= 0)
                            {
                                GameObject parent = this.gameObject.GetComponent<GetParents>().getParent();
                                parent.gameObject.GetComponent<CharacterGold>().varyGold(5);
                            }
                        }

                        if (col.gameObject.layer == 9)
                        {
                            Debug.Log("Consortium Minion Hit");
                            GameObject target = col.gameObject.GetComponent<GetParents>().getParent();
                            target.gameObject.GetComponent<TeamConsortiumLeftMinionBehaviour>().TakeDamage(-healthChangeAmount);
                            if (target.gameObject.GetComponent<TeamConsortiumLeftMinionBehaviour>().getHealth() <= 0)
                            {
                                GameObject parent = this.gameObject.GetComponent<GetParents>().getParent();
                                parent.gameObject.GetComponent<CharacterGold>().varyGold(5);
                            }
                        }
                    }
                }

                //used to test gold for now
                if (col.gameObject.CompareTag("NeutralMinion"))
                {
                    GameObject parent = this.gameObject.GetComponent<GetParents>().getParent();
                    parent.gameObject.GetComponent<CharacterGold>().varyGold(5);

                    if (destroyOnContact)
                    {
                        Object.Destroy(this.gameObject);
                    }

                }


                if (destroyOnContact)
                {
                    Object.Destroy(this.gameObject);

                }
            }




        }
        private void Start()
        {

        }
    }
}





