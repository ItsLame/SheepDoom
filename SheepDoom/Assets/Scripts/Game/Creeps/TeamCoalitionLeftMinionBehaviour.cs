using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class TeamCoalitionLeftMinionBehaviour : MonoBehaviour
{
    // Waypoint System
    public Transform[] waypoints;
    public int StartIndex = 0;
    private int currentPoint = 0;
    private Vector3 target;
    private Vector3 direction;
    private Transform playerTransf;
    private NavMeshAgent agent;
    public bool ismeleeattack = false;
    Animator charAnim;

    //Aggro
    private float speed = 15.0f;
    private Vector3 wayPointPos;
    // public float howclose;
    private float dist;
    public float CreepMoveSpeed = 2.0f;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyattacked;

    //states 
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    //layermask
    public LayerMask whatisplayer;

    //Ranged Projectile
    public GameObject projectile;


    //animator
    public Animator animator;

    //melee attackpoint
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public int meleedamage = 50;

    //Health
    [SerializeField]
    private int maxHealth = 50;
    private int currenthealth;

    public event Action<float> OnHealthPctChanged = delegate { };

    void Start()
    {

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransf = player.GetComponent<Transform>();

        charAnim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        currenthealth = maxHealth;
        currentPoint = StartIndex;
        StartMovingToWayPoint();
        agent.autoBraking = false;
    }

    void Update()
    {

        dist = Vector3.Distance(playerTransf.position, transform.position);
        //Check if Player in sightrange
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatisplayer);

        //Check if Player in attackrange
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatisplayer);

        if (target == null)
        {
            StartMovingToWayPoint();
            return;
        }
        if (!playerInSightRange && !playerInAttackRange) StartMovingToWayPoint();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
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
            currenthealth = 0;
            Destroyy();
        }

    }

    void ChasePlayer()
    {

        agent.SetDestination(playerTransf.position);

    }
    void MeleeAttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(playerTransf);

        if (!alreadyattacked)
        {
            //Meele attack for dog
            transform.LookAt(playerTransf);
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

        transform.LookAt(playerTransf);

        if (!alreadyattacked)
        {
            transform.LookAt(playerTransf);
            //Attack
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();

            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8, ForceMode.Impulse);

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
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}

