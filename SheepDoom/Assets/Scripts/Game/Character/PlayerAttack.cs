using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private bool attacking = false;
    private bool ulti = false;
    private bool specialskill = false;

    //Skill projectiles
    public GameObject Projectile;
    public GameObject Projectile2;
    public GameObject Projectile3;

    //skill launch point
    public Transform SpawnPoint;

    //skillcd basetrackers
    public float cooldown1;
    public float cooldown2;
    public float cooldown3;

    //skillcd values to be used n manipulated in game
    public float cooldown1_inGame;
    public float cooldown2_inGame;
    public float cooldown3_inGame;

    // Start is called before the first frame update
    void Start()
    {
        cooldown1_inGame = cooldown1;
        cooldown2_inGame = cooldown2;
        cooldown3_inGame = cooldown3;
    }

    public void AttackClick()
    {
        attacking = true;
    }

    public void UltiClick()
    {
        ulti = true;
    }

    public void SpecialSkillClick()
    {
        specialskill = true;
    }

    // Update is called once per frame
    void Update()
    {
        //reduce cd per second
        cooldown1_inGame -= Time.deltaTime;
        cooldown2_inGame -= Time.deltaTime;
        cooldown3_inGame -= Time.deltaTime;

        if (attacking)
        {
            //if off cd
            if (cooldown1_inGame <= 0)
            {
                Instantiate(Projectile, SpawnPoint.position, SpawnPoint.rotation);
                //resetcd
                cooldown1_inGame = cooldown1; 
                attacking = false;
            }

        }

        if (specialskill)
        {
            if (cooldown2_inGame <= 0)
            {
                Instantiate(Projectile2, SpawnPoint.position, SpawnPoint.rotation);
                cooldown2_inGame = cooldown2;
                specialskill = false;
            }

        }

        if (ulti)
        {
            if (cooldown3_inGame <=0)
            {
                Instantiate(Projectile3, SpawnPoint.position, SpawnPoint.rotation);
                cooldown3_inGame = cooldown3;
                ulti = false;
            }

        }
    }
}
