using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class OnStayHealth : NetworkBehaviour
    {
        [Header("Object name to be used for announcer")]
        [SerializeField] private string objectName;
        [SerializeField] private bool isFallZone;

        [Header("Amount of health to change on contact")]
        [SerializeField] private float healthChangeAmount;

        [Header("What does it interact with")]
        [SerializeField] private bool willContactPlayer;
        [SerializeField] private bool willContactMinion;

        [Header("When to interact with bool")]
        [SerializeField] private bool hitboxActive;


        public void SetHitBox(bool _status)
        {
            hitboxActive = _status;
        }

        //only for insta kill zone
        [ServerCallback]
        private void OnTriggerEnter(Collider col)
        {
            if (isFallZone)
            {
                //if hit other player
                if (col.CompareTag("Player") && !col.GetComponent<PlayerHealth>().isPlayerDead())
                {
   //                 Debug.Log("Player Hit");
                    //change the hit player's HP
                    col.GetComponent<PlayerHealth>().modifyinghealth(healthChangeAmount);

                    col.GetComponent<PlayerHealth>().SetPlayerDead();
                    //give announcer info
                    col.GetComponent<GameEvent>().whoKilled = objectName;
                }
            }
        }

        //when collide with player
        [ServerCallback]
        private void OnTriggerStay(Collider col)
        {
            if (isFallZone) return;
            if (hitboxActive)
            {
                if (willContactPlayer)
                {
                    //if hit other player
                    if (col.CompareTag("Player") && !col.GetComponent<PlayerHealth>().isPlayerDead())
                    {
                        Debug.Log("Player Hit");
                        //change the hit player's HP
                        col.GetComponent<PlayerHealth>().modifyinghealth(healthChangeAmount);

                        //kill target if target hp <= 0
                        if (col.GetComponent<PlayerHealth>().getHealth() <= 0)
                        {
                            //set hit target to dead
                            col.GetComponent<PlayerHealth>().SetPlayerDead();

                            //give announcer info
                            col.GetComponent<GameEvent>().whoKilled = objectName;
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
                            target.GetComponent<LeftMinionBehaviour>().TakeDamage(healthChangeAmount);

                            if (target.GetComponent<LeftMinionBehaviour>().getHealth() <= 0)
                            {
                                GameObject parent = this.gameObject.GetComponent<GetParents>().getParent();
                                parent.GetComponent<CharacterGold>().CmdVaryGold(5);
                            }
                        }


                        if (col.gameObject.layer == 9)
                        {
                            Debug.Log("Consortium Minion Hit");
                            GameObject target = col.gameObject.GetComponent<GetParents>().getParent();
                            target.GetComponent<LeftMinionBehaviour>().TakeDamage(healthChangeAmount);
                            if (target.GetComponent<LeftMinionBehaviour>().getHealth() <= 0)
                            {
                                GameObject parent = this.gameObject.GetComponent<GetParents>().getParent();
                                parent.GetComponent<CharacterGold>().CmdVaryGold(5);
                            }
                        }

                    }
                }

                //used to test gold for now
                if (col.gameObject.CompareTag("NeutralMinion"))
                {
                    GameObject parent = this.gameObject.GetComponent<GetParents>().getParent();
                    parent.gameObject.GetComponent<CharacterGold>().CmdVaryGold(5);

                }
            }
        }
    }
}





