using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 100;

    private int currenthealth;

    public event Action<float> OnHealthPctChanged = delegate { };

    private void Start()
    {
        currenthealth = maxHealth;
    }

    public void modifyinghealth(int amount)
    {
        currenthealth += amount;

        float currenthealthPct = (float)currenthealth / (float)maxHealth;
        OnHealthPctChanged(currenthealthPct);
    }
    // Update is called once per frame
    void Update()
    {
        if (currenthealth <= 0)
        {
            currenthealth = 0;
            GameOver();
        }
        if (currenthealth > maxHealth)
        {
            currenthealth = maxHealth;
        }
    }

    void GameOver()
    {
        //added respawn
        this.gameObject.GetComponent<PlayerRespawn>().isDead = true;

        //StartCoroutine(TimeBeforeDeath());
        Debug.Log("health: ded");
    }

    IEnumerator TimeBeforeDeath()
    {
        // should be sync with death animation
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
