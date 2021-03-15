using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileSettings : MonoBehaviour
{
    //rotation controls
    public float x_rotaspeed;
    public float y_rotaspeed;
    public float z_rotaspeed;

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
            Debug.Log("health: player hit by " + m_Rigidbody);
            Object.Destroy(this.gameObject);
        } else if (col.tag == "Tower")
        {
            col.transform.parent.gameObject.GetComponent<CapturePointScript>().modifyinghealth(-damage);
            Debug.Log("health: tower hit by " + m_Rigidbody);
            Object.Destroy(this.gameObject);
        }
    }

    void Update()
    {
        transform.Rotate(1.0f * x_rotaspeed, 1.0f * y_rotaspeed, 1.0f * z_rotaspeed);
    }
}