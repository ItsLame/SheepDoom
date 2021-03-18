using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
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

    //Melee Bool false
    public bool ismeeleeattack = false;

    //Melee
    public Animator animator;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int meleedamage = 50;
    public LayerMask enemyLayers;

    // Start is called before the first frame update    
    void Start()
    {
        cooldown1_inGame = cooldown1;
        cooldown2_inGame = cooldown2;
        cooldown3_inGame = cooldown3;

        SpecialButton.GetComponent<Button>().interactable = false;
        UltiButton.GetComponent<Button>().interactable = false;
    }

    //if atk button is pressed
    public void AttackClick()
    {
        //if off cd
        if (cooldown1_inGame <= 0 && ismeeleeattack == false)
        {
            Instantiate(Projectile, SpawnPoint.position, SpawnPoint.rotation);
            //resetcd
            cooldown1_inGame = cooldown1;
        }
        else if (cooldown1_inGame <= 0 && ismeeleeattack == true)
        {
            animator.SetTrigger("Attack");
            animator.SetTrigger("AttackToIdle");
            Collider[] hitenmies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

            foreach (Collider enemy in hitenmies)
            {
                enemy.GetComponent<PlayerHealth>().modifyinghealth(-meleedamage);

            }
        }
    }

    //if ulti button is pressed
    public void UltiClick()
    {
        if (hasPurchasedUlti)
        {
            if (cooldown3_inGame <= 0)
            {
                Instantiate(Projectile3, SpawnPoint.position, SpawnPoint.rotation);
                cooldown3_inGame = cooldown3;
            }
        }
        else
        {
            Debug.Log("Player hasn't purchased ultimate skill");
        }
    }

    //if special skill is pressed
    public void SpecialSkillClick()
    {
        //only available if special skill is purchased
        if (hasPurchasedSpecial)
        {
            if (cooldown2_inGame <= 0)
            {
                Instantiate(Projectile2, SpawnPoint.position, SpawnPoint.rotation);
                cooldown2_inGame = cooldown2;
            }
        }
        else
        {
            Debug.Log("Player hasn't purchased special skill");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if not 0, reduce cd per second
        if (cooldown1_inGame >= 0)
        {
            cooldown1_inGame -= Time.deltaTime;
        }

        if (cooldown2_inGame >= 0)
        {
            cooldown2_inGame -= Time.deltaTime;
        }

        if (cooldown3_inGame >= 0)
        {
            cooldown3_inGame -= Time.deltaTime;
        }
    }
}
