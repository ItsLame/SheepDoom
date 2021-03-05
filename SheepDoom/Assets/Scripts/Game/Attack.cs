using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private bool attacking = false;
    private bool ulti = false;
    private bool specialskill = false;
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
        if (attacking)
        {
            Instantiate(Projectile, SpawnPoint.position, SpawnPoint.rotation);
            attacking = false;
        }
        if (ulti)
        {
            Instantiate(Projectile, SpawnPoint.position, SpawnPoint.rotation);
            ulti = false;
        }
        if (specialskill)
        {
            Instantiate(Projectile, SpawnPoint.position, SpawnPoint.rotation);
            specialskill = false;
        }
    }
}
