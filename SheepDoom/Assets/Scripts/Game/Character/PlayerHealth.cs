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

    private void OnEnable()
    {
        currenthealth = maxHealth;

    }
    private void modifyinghealth(int amount)
    {
        currenthealth += amount;

        float currenthealthPct = (float)currenthealth / (float)maxHealth;
        OnHealthPctChanged(currenthealthPct);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            modifyinghealth(-10);
        
    }
}
