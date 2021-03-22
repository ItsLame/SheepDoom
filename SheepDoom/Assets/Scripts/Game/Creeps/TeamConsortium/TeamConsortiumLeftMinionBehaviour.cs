using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
public class TeamConsortiumLeftMinionBehaviour : MonoBehaviour
{
    [Space(15)]
    // Waypoint System
    public Transform[] waypoints;
    public int StartIndex = 0;
    private int currentPoint = 0;
    private Vector3 target;
    private Vector3 direction;
    [Space(15)]

    private NavMeshAgent agent;
    [Space(15)]
    public bool ismeleeattack = false;
    Animator charAnim;

    //Aggro
    private float speed = 15.0f;
    private Vector3 wayPointPos;
    // public float howclose;
    // private float dist;
    public float CreepMoveSpeed = 2.0f;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyattacked;

    //states 
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public bool TeamCoalition;

    //layermask
    public LayerMask whatisplayer;
    [Space(15)]
    //Ranged Projectile
    public GameObject projectile;
    [Space(15)]

    //animator
    public Animator animator;
    [Space(15)]
    //melee attackpoint
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public int meleedamage = 50;
    [Space(15)]
    //Health
    [SerializeField]
    private int maxHealth = 50;
    private int currenthealth;
    public float goldValue;

    [Space(15)]
    GameObject targetObject;
    private Transform targetTransf;
    private bool isLockedOn;
   // public bool isplayer = false;


    public event Action<float> OnHealthPctChanged = delegate { };

    void Start()
    {

        targetObject = null;
        targetTransf = null;
        isLockedOn = false;
        charAnim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        currenthealth = maxHealth;
        currentPoint = StartIndex;
        StartMovingToWayPoint();
        agent.autoBraking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            if (!isLockedOn)
            {
                targetObject = other.gameObject;
                targetTransf = targetObject.transform;
                isLockedOn = true;
            }    

        }

        /*
        else if (other.gameObject.layer == 8 && other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Spotted By " + this.gameObject.name);
            player = other.gameObject;
            playerTransf = player.transform;
            isplayer = true;
        } */
    }

    /*
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player " + other.gameObject.name + " has left minion zone");
            player = null;
            playerTransf = null;
        }
    } */

    void Update()
    {

        // dist = Vector3.Distance(playerTransf.position, transform.position);
        //Check if Player in sightrange
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatisplayer);

        //Check if Player in attackrange
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatisplayer);

        if (target == null)
        {
            targetObject = null;
            targetTransf = null;
            isLockedOn = false;
            StartMovingToWayPoint();
            return;
        }

        if (!playerInSightRange && !playerInAttackRange)
        {
            StartMovingToWayPoint();
        }

        if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }

        if (playerInAttackRange && playerInSightRange && ismeleeattack == false)
        {
            RangedAttackPlayer();
        }
        if (playerInAttackRange && playerInSightRange && ismeleeattack == true)
        {
            MeleeAttackPlayer();
        }

        if (currenthealth <= 0)
        {
            Debug.Log(this.gameObject.name + "has died");

            //if targetobject still around
            if (targetObject == true)
            {
                currenthealth = 0;
                if (targetObject.CompareTag("Player"))
                {
                    targetObject.GetComponent<CharacterGold>().varyGold(goldValue);
                }

                Destroyy();

            }

            Destroyy();

            /*
            else
            {
                currenthealth = 0;
                Destroyy();
            }*/

        }

    }

    void ChasePlayer()
    {

        agent.SetDestination(targetTransf.position);

    }
    void MeleeAttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(targetTransf);

        if (!alreadyattacked)
        {
            //Meele attack for dog
            transform.LookAt(targetTransf);
            animator.SetTrigger("Attack");
            animator.SetTrigger("AttackToIdle");
            Collider[] hitenmies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

            foreach (Collider enemy in hitenmies)
            {
                enemy.GetComponent<PlayerHealth>().modifyinghealth(-meleedamage);

            }
            Debug.Log("MeleeAttack!");
            alreadyattacked = true;
            Invoke("ResetAttack", timeBetweenAttacks);
        }
    }

    void RangedAttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(targetTransf);

        if (!alreadyattacked)
        {
            transform.LookAt(targetTransf);
            //Attack
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();

            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
        //    rb.AddForce(transform.up * 8, ForceMode.Impulse);

            alreadyattacked = true;
            Invoke("ResetAttack", timeBetweenAttacks);
        }
    }

    void ResetAttack()
    {
        alreadyattacked = false;
    }

    void StartMovingToWayPoint()
    {
        if (currentPoint < waypoints.Length)
        {
            target = waypoints[currentPoint].position;
            direction = target - transform.position;
            if (direction.magnitude < 5)
                currentPoint++;
        }
        else if (currentPoint == waypoints.Length)
        {
            agent.autoBraking = true;
        }
        transform.LookAt(target);
        agent.SetDestination(target);
        agent.speed = CreepMoveSpeed;

    }
    public void TakeDamage(int damage)
    {
        currenthealth += damage;

        float currenthealthPct = (float)currenthealth / (float)maxHealth;
        OnHealthPctChanged(currenthealthPct);
    }

    private void Destroyy()
    {
        Destroy(this.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
