using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private bool attacking = false;
    //private float attackTimer = 0;
    //private float attackCd = 0.3f;
    //public Collider2D attacktrigger;
    private Animation anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animation>();
        //attacktrigger.enabled = false;
    }

    public void AttackClick()
    {
        attacking = true;
        //attackTimer = attackCd;
        //attacktrigger.enabled = true;

    }
    // Update is called once per frame
    void Update()
    {
        if (attacking)
        {
            Debug.Log("attack TRUE");
            anim.Play("attack");
        }
        /*
        if (attacking)
        {
            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            }else
            {
                attacking = false;
                //attacktrigger.enabled = false;
            }
        }
        anim.Play("attack");
        */
    }
}
