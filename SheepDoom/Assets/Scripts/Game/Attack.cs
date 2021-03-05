using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private bool attacking = false;
    public GameObject Projectile;
    public Transform SpawnPoint;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void AttackClick()
    {
        attacking = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (attacking)
        {
            Instantiate(Projectile, SpawnPoint.position, SpawnPoint.rotation);
            attacking = false;
        }
    }
}
