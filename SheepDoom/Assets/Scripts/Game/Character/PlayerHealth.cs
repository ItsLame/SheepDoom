using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

namespace SheepDoom
{
    public class PlayerHealth : NetworkBehaviour
    {
        [SerializeField]
        private float maxHealth = 100.0f;

        [SyncVar(hook = nameof(healthUpdate))]
        public float currenthealth;

        public event Action<float> OnHealthPctChanged = delegate { };

        public bool isFullHealth;

        //for calling death function
        bool deathCounterCalled = false;

        //is character dead?
        [SyncVar]
        bool playerDead = false;

        private void Start()
        {
            currenthealth = maxHealth;
        }

        //for syncvar to sync player health
        private void healthUpdate(float oldHealth, float newHealth)
        {
            if(hasAuthority)
                Debug.Log("Old HP: " + oldHealth + " New HP: " + newHealth);
        }

        public void modifyinghealth(float amount)
        {
            currenthealth += amount;

            float currenthealthPct = currenthealth / maxHealth;
            OnHealthPctChanged(currenthealthPct);
        }
        // Update is called once per frame
        void Update()
        {
            if (currenthealth <= 0)
            {
                currenthealth = 0;


                //increase death by 1
                if (deathCounterCalled == false)
                {
                    this.gameObject.GetComponent<PlayerAdmin>().increasePlayerDeathCount();
                    deathCounterCalled = true;
                }

                GameOver();
            }   
            if (currenthealth > maxHealth)
            {
                currenthealth = maxHealth;
                isFullHealth = true;
            }

            if (currenthealth < maxHealth)
            {
                isFullHealth = false;
            }
        }

        void GameOver()
        {
            //added respawn
            this.gameObject.GetComponent<PlayerRespawn>().isDead = true;
            playerDead = true;

            //StartCoroutine(TimeBeforeDeath());
            // Debug.Log("health: ded");
        }

        IEnumerator TimeBeforeDeath()
        {
            // should be sync with death animation
            yield return new WaitForSeconds(2);
            Destroy(gameObject);
        }

        public void RefillHealth()
        {
            currenthealth = maxHealth;
            deathCounterCalled = false;
        }

        public float getHealth()
        {
            return currenthealth;
        }

        //to set playerdead to false when player is respawned
        public void revivePlayer()
        {
            playerDead = false;
        }

        //accessor method for playerdead
        public bool isPlayerDead()
        {
            return playerDead;
        }
    }
}
