using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class RangedCreepProjectilesettings : NetworkBehaviour
    {
        private GameObject owner;
        [SerializeField]
        private float damage;
        [SerializeField]
        private float m_Speed; // default speed of projectile
        [SerializeField]
        private float m_Lifespan; // Lifespan per second
        private float m_startTime;

        [SerializeField]
        private Rigidbody m_Rigidbody;

        public void setOwner(GameObject firer)
        {
            owner = firer;
        }

        [ServerCallback]
        void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag("Other"))
            {
                Destroyy();
            }

            if (col.CompareTag("Shield"))
            {
                Destroyy();
            }

            if (owner != null)
            {
                if (owner.CompareTag("TeamCoalitionRangeCreep"))
                {
                    if (col.CompareTag("Player") && !col.GetComponent<PlayerHealth>().isPlayerDead())
                    {
                        if (col.gameObject.GetComponent<PlayerAdmin>().getTeamIndex() == 2) // consortium
                        {
                            col.GetComponent<PlayerHealth>().modifyinghealth(-damage);
                            Destroyy();
                        }

                        if (col.GetComponent<PlayerHealth>().getHealth() <= 0)
                        {
                            col.GetComponent<PlayerHealth>().SetPlayerDead();
                            col.gameObject.GetComponent<GameEvent>().isMinion = true;
                            owner.GetComponent<LeftMinionBehaviour>().goBackToTravelling();
                        }
                    }
                    else if (col.CompareTag("BaseMinion") && col.gameObject.layer == 9)
                    {
                        col.transform.parent.GetComponent<LeftMinionBehaviour>().TakeDamage(-damage);
                        Destroyy();
                    }

                    else if (col.gameObject.CompareTag("Shield"))
                    {
                        if (col.gameObject.GetComponent<ObjectFollowScript>().teamID == 2)
                        {
                                Destroyy();
                        }
                    }
                }
                else if (owner.CompareTag("TeamConsortiumRangeCreep"))
                {
                    if (col.CompareTag("Player") && !col.GetComponent<PlayerHealth>().isPlayerDead())
                    {
                        if (col.gameObject.GetComponent<PlayerAdmin>().getTeamIndex() == 1) // coalition
                        {
                            col.GetComponent<PlayerHealth>().modifyinghealth(-damage);
                            Destroyy();
                        }

                        if (col.GetComponent<PlayerHealth>().getHealth() <= 0)
                        {
                            col.GetComponent<PlayerHealth>().SetPlayerDead();
                            col.gameObject.GetComponent<GameEvent>().isMinion = true;
                            owner.GetComponent<LeftMinionBehaviour>().goBackToTravelling();
                        }
                    }
                    else if (col.CompareTag("BaseMinion") && col.gameObject.layer == 8)
                    {
                        col.transform.parent.GetComponent<LeftMinionBehaviour>().TakeDamage(-damage);
                        Destroyy();
                    }

                    else if (col.gameObject.CompareTag("Shield"))
                    {
                        if (col.gameObject.GetComponent<ObjectFollowScript>().teamID == 1)
                        {
                            Destroyy();
                        }
                    }
                }
            }
        }

        private void Destroyy()
        {
            NetworkServer.Destroy(gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            //basic forward movement
            if (isServer)
            {
                m_startTime += Time.deltaTime;
                transform.Translate(Vector3.forward * m_Speed * Time.deltaTime);
                if (m_startTime > m_Lifespan)
                    Destroyy();
            }
        }
    }
}