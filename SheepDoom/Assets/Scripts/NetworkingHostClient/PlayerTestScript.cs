using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerTestScript : MonoBehaviour
{
    public int speed = 1;
    private bool attacking = false;
    private bool ulti = false;
    private bool specialskill = false;

    //Skill projectiles
    public GameObject Projectile;
    public GameObject Projectile2;
    public GameObject Projectile3;
    //skill launch point
    public Transform SpawnPoint;


    void Movement()
    {
        Vector3 pos = transform.position;
        if (Input.GetKey(KeyCode.D))
            pos.z += speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
            pos.z -= speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S))
            pos.x += speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.W))
            pos.x -= speed * Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.I))
            attacking = true;
        if (Input.GetKeyDown(KeyCode.O))
            specialskill = true;
        if (Input.GetKeyDown(KeyCode.P))
            ulti = true;

        transform.position = pos;
    }

    void Update()
    {
        Movement();
        if (attacking)
        {
            Instantiate(Projectile, SpawnPoint.position, SpawnPoint.rotation);
            attacking = false;
        }

        if (specialskill)
        {
            Instantiate(Projectile2, SpawnPoint.position, SpawnPoint.rotation);
            specialskill = false;
        }

        if (ulti)
        {
            Instantiate(Projectile3, SpawnPoint.position, SpawnPoint.rotation);
            ulti = false;
        }
    }
}
