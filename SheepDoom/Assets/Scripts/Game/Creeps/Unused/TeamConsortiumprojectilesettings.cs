using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/*namespace SheepDoom
{
    public class TeamConsortiumprojectilesettings : NetworkBehaviour
    {
        public GameObject owner;
        public int damage;

        public float m_Speed = 10f;   // default speed of projectile
        public float m_Lifespan = 3f; // Lifespan per second

        private Rigidbody m_Rigidbody;

        void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }
        void Start()
        {
 //           m_Rigidbody.AddForce(transform.forward * m_Speed);
            Destroy(gameObject, m_Lifespan);
        }

        public void setOwner(GameObject firer)
        {
            owner = firer;
        }

        [Server]
        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.layer == 8 && col.gameObject.CompareTag("Player"))
            {
                //dont hit dead ppl
                if (col.gameObject.GetComponent<PlayerHealth>().isPlayerDead()) return;

                col.gameObject.GetComponent<PlayerHealth>().modifyinghealth(-damage);
                Destroy(this.gameObject);

                if (col.gameObject.GetComponent<PlayerHealth>().getHealth() <= 0)
                {
                    Debug.Log("Minion killed player");
                    col.gameObject.GetComponent<PlayerHealth>().SetPlayerDead();

                    //set minion target to null
                    owner.gameObject.GetComponent<LeftMinionBehaviour>().goBackToTravelling();
                }


            }

            else if (col.gameObject.CompareTag("BaseMinion") && col.gameObject.layer == 8)
            {
                col.transform.parent.gameObject.GetComponent<LeftMinionBehaviour>().TakeDamage(-damage);
                Destroy(this.gameObject);
                //  Debug.Log("health: baseMinion hit by " + m_Rigidbody); 
            }
        }


        // Update is called once per frame
        void Update()
        {
            //basic forward movement
            transform.Translate(Vector3.forward * m_Speed * Time.deltaTime);
        }
    }
}*/