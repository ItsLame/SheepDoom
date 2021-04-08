using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SheepDoom
{
    public class CreeprojectileSettings : MonoBehaviour
    {
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
            m_Rigidbody.AddForce(transform.forward * m_Speed);
            Destroy(gameObject, m_Lifespan);
        }
        void OnTriggerEnter(Collider col)
        {
            if (col.tag == "Player")
            {
                col.gameObject.GetComponent<PlayerHealth>().modifyinghealth(-damage);
                Debug.Log("health: hit by " + m_Rigidbody);
                Object.Destroy(this.gameObject);
            }
        }


        // Update is called once per frame
        void Update()
        {

        }
    }
}