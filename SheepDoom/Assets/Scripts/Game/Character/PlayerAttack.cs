using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    //for activating attacks when buttons r clicked
    private bool attacking = false;
    private bool ulti = false;
    private bool specialskill = false;

    [Space(15)]
    //for checking if player purchased special and ulti
    public bool hasPurchasedSpecial = false;
    public bool hasPurchasedUlti = false;

    public GameObject SpecialButton;
    public GameObject UltiButton;

    [Space(15)]
    //Skill projectiles
    public GameObject Projectile;
    public GameObject Projectile2;
    public GameObject Projectile3;

    [Space(15)]
    //skill launch point
    public Transform SpawnPoint;

    //skillcd basetrackers
    [SerializeField]
    public float cooldown1, cooldown2, cooldown3;

    //skillcd values to be used n manipulated in game
    [SerializeField]
    public float cooldown1_inGame, cooldown2_inGame, cooldown3_inGame;

    // Start is called before the first frame update    
    void Start()
    {
        cooldown1_inGame = cooldown1;
        cooldown2_inGame = cooldown2;
        cooldown3_inGame = cooldown3;

        SpecialButton.GetComponent<Button>().interactable = false;
        UltiButton.GetComponent<Button>().interactable = false;
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

        //if atk button is pressed
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

        //if special skill is pressed
        if (specialskill)
        {
            //only available if special skill is purchased
            if (hasPurchasedSpecial)
            {
                if (cooldown2_inGame <= 0)
                {
                    Instantiate(Projectile2, SpawnPoint.position, SpawnPoint.rotation);
                    cooldown2_inGame = cooldown2;
                    specialskill = false;
                }
            }

            else
            {
                Debug.Log("Player hasn't purchased special skill");
            }

        }

        //if ulti button is pressed
        if (ulti)
        {
            if (hasPurchasedUlti)
            {
                if (cooldown3_inGame <= 0)
                {
                    Instantiate(Projectile3, SpawnPoint.position, SpawnPoint.rotation);
                    cooldown3_inGame = cooldown3;
                    ulti = false;
                }
            }

            else
            {
                Debug.Log("Player hasn't purchased ultimate skill");
            }

        }
    }
}
