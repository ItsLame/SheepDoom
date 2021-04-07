using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : NetworkBehaviour
{
    [Space(15)]
    //for checking if player purchased special and ulti
    public bool hasPurchasedSpecial = false;
    public bool hasPurchasedUlti = false;

    public GameObject NormalButton;
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
    private float cooldown1, cooldown2, cooldown3;

    //skillcd values to be used n manipulated in game

    private float cooldown1_inGame, cooldown2_inGame, cooldown3_inGame;
    [Space(15)]
    //Melee Bool false
    public bool ismeeleeattack = false;
    [Space(15)]
    //Melee
    public Animator animator;
    [Space(15)]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int meleedamage = 50;
    [Space(15)]
    public LayerMask enemyLayers;
    [Space(15)]
    public bool isDead;

    // Start is called before the first frame update    
    void Start()
    {
        if (!hasAuthority) return;
        cooldown1_inGame = cooldown1;
        cooldown2_inGame = cooldown2;
        cooldown3_inGame = cooldown3;

        SpecialButton.GetComponent<Button>().interactable = false;
        UltiButton.GetComponent<Button>().interactable = false;
    }

    public void AttackClick()
    {
        if (!hasAuthority) return;
        CmdAttackClick();
    }

    public void SpecialSkillClick()
    {
        if (!hasAuthority) return;
        CmdAttackClick();
    }

    public void UltiClick()
    {
        if (!hasAuthority) return;
        CmdAttackClick();
    }

    [Command]
    //if atk button is pressed
    public void CmdAttackClick()
    {
        //if not dead
        if (!isDead)
        {
            //if off cd
            if (cooldown1_inGame <= 0 && ismeeleeattack == false)
            {
                Debug.Log("Firing Normal Atk");
                var FiredProjectile = Instantiate(Projectile, SpawnPoint.position, SpawnPoint.rotation);
                FiredProjectile.GetComponent<ProjectileSettings>().owner = this.gameObject;
                NetworkServer.Spawn(FiredProjectile);

                //resetcd
                cooldown1_inGame = cooldown1;
            }
            else if (cooldown1_inGame <= 0 && ismeeleeattack == true)
            {
               // animator.SetTrigger("Attack");
               // animator.SetTrigger("AttackToIdle");
                Collider[] hitenmies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

                foreach (Collider enemy in hitenmies)
                {
                    enemy.GetComponent<PlayerHealth>().modifyinghealth(-meleedamage);

                }
            }
        }

    }

    //if special skill is pressed
    [Command]
    public void CmdSpecialSkillClick()
    {
        if (!isDead)
        {
            //only available if special skill is purchased
            if (hasPurchasedSpecial)
            {
                if (cooldown2_inGame <= 0)
                {
                    Debug.Log("Firing Special Atk");
                    var FiredProjectile =  Instantiate(Projectile2, SpawnPoint.position, SpawnPoint.rotation);
                    FiredProjectile.GetComponent<ProjectileSettings>().owner = this.gameObject;
                    cooldown2_inGame = cooldown2;
                }
            }
            else
            {
                Debug.Log("Player hasn't purchased special skill");
            }
        }
    }

    //if ulti button is pressed
    [Command]
    public void CmdUltiClick()
    {
        if (!isDead)
        {
            if (hasPurchasedUlti)
            {
                if (cooldown3_inGame <= 0)
                {
                    Debug.Log("Firing Ultimate Atk");
                    var FiredProjectile = Instantiate(Projectile3, SpawnPoint.position, SpawnPoint.rotation);
                    FiredProjectile.GetComponent<ProjectileSettings>().owner = this.gameObject;
                    cooldown3_inGame = cooldown3;
                }
            }
            else
            {
                Debug.Log("Player hasn't purchased ultimate skill");
            }
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
