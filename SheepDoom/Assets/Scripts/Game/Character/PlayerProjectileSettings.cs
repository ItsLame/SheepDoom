using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SheepDoom
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerProjectileSettings : MonoBehaviour
    {
        //projectileOwner
        public GameObject owner;

        [Space(15)]
        //rotation controls
        public float x_rotaspeed;
        public float y_rotaspeed;
        public float z_rotaspeed;

        [Space(15)]
        public int damage;
        public float m_Speed = 10f;   // default speed of projectile
        public float m_Lifespan = 3f; // Lifespan per second
        public bool destroyOnContact; //if projectile ill stop on first contact

        private Rigidbody m_Rigidbody;

        //bool for calling kill counter increase once
        bool killCounterIncreaseCalled = false;

        void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }
        void Start()
        {
            m_Rigidbody.AddForce(transform.forward * m_Speed);
            Destroy(gameObject, m_Lifespan);
        }


        void OnTriggerEnter(Collider col)
        {
            //if hit player
            if (col.gameObject.CompareTag("Player"))
            {
                //dont hurt the owner of the projectile
                if (col.gameObject != owner)
                {
                    //reduce HP of hit target
                    col.gameObject.GetComponent<PlayerHealth>().modifyinghealth(-damage);
                    string playerName = col.gameObject.GetComponent<PlayerUI>().getPlayerName().text;
                    //Debug.Log(playerName + "'s health: " + col.gameObject.GetComponent<PlayerHealth>().getHealth());

                    //increase killer's kill count if target is killed
                    if (col.gameObject.GetComponent<PlayerHealth>().getHealth() <= 0)
                    {
                        //dont increase score if hitting dead player
                        if (col.gameObject.GetComponent<PlayerHealth>().isPlayerDead()) return;
                        owner.gameObject.GetComponent<PlayerAdmin>().increasePlayerKillCount();
                    }

                    if (destroyOnContact)
                    {
                        Object.Destroy(this.gameObject);
                    }
                }


            }

            else if (col.gameObject.CompareTag("Tower"))
            {
                col.transform.parent.gameObject.GetComponent<CapturePointScript>().modifyinghealth(-damage);
                //  Debug.Log("health: tower hit by " + m_Rigidbody);
                Object.Destroy(this.gameObject);
            }

            else if (col.gameObject.CompareTag("NeutralMinion"))
            {
                //  Debug.Log("NeutralMinion hit by " + player.gameObject.name);



                //take damage
                col.gameObject.GetComponent<NeutralCreepScript>().Attacker = owner;
                col.gameObject.GetComponent<NeutralCreepScript>().neutralTakeDamage(-damage);
                //inform that its under atk
                col.gameObject.GetComponent<NeutralCreepScript>().isUnderAttack();

                if (destroyOnContact)
                {
                    Object.Destroy(this.gameObject);
                }

            }

            else if (col.gameObject.CompareTag("BaseMinion") && col.gameObject.layer == 9)
            {
                col.transform.parent.gameObject.GetComponent<TeamConsortiumLeftMinionBehaviour>().TakeDamage(-damage);
                col.transform.parent.gameObject.GetComponent<TeamConsortiumLeftMinionBehaviour>().Murderer = owner;
                //  Debug.Log("health: baseMinion hit by " + m_Rigidbody);

                if (destroyOnContact)
                {
                    Object.Destroy(this.gameObject);

                }

            }
            else if (col.gameObject.CompareTag("MegaBoss"))
            {
                col.transform.parent.gameObject.GetComponent<MegaBossBehaviour>().TakeDamage(-damage);
                col.transform.parent.gameObject.GetComponent<MegaBossBehaviour>().Murderer = owner;
                //  Debug.Log("health: baseMinion hit by " + m_Rigidbody);

                if (destroyOnContact)
                {
                    Object.Destroy(this.gameObject);

                }

            }
            /*
            else if (col.tag == "Base")
            {
                col.transform.parent.gameObject.GetComponent<CaptureBaseScript>().modifyinghealth(-damage);
                Debug.Log("health: base hit by " + m_Rigidbody);
                Object.Destroy(this.gameObject);
            }
            */
        }

        void Update()
        {
            transform.Rotate(1.0f * x_rotaspeed, 1.0f * y_rotaspeed, 1.0f * z_rotaspeed);
        }
    }
}
