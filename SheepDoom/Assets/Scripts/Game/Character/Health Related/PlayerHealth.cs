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

        [SyncVar(hook = nameof(HealthBarUpdate))]
        private float currenthealth;

        public event Action<float> OnHealthPctChanged = delegate { };

        private bool isFullHealth = true;

        //for calling death function
        bool deathCounterCalled = false;

        //is character dead?
        [SyncVar]
        bool playerDead = false;

        private void Start()
        {
            currenthealth = maxHealth;
        }

        [Server]    
        public void modifyinghealth(float amount)
        {
            currenthealth += amount;

            float currenthealthPct = currenthealth / maxHealth;
            OnHealthPctChanged(currenthealthPct);

            // wake player up if taking damage
            // health modify is called before debuffing so if the new damage type has a debuff
            // the waking up will occur first, n debuffing immediately after
            if (amount < 0)
            {
                if (this.gameObject.GetComponent<CharacterMovement>().isSleeped)
                {
                    this.gameObject.GetComponent<CharacterMovement>().changeSpeed("stop", 0.1f, 0);
                }
            }

        }

        // Update is called once per frame
        void Update()
        {
            if(isServer)
            {
                if (playerDead)
                {
                    currenthealth = 0;

                    //increase death by 1
                    if (!deathCounterCalled)
                    {
                        GetComponent<PlayerAdmin>().IncreaseCount(false, false, true);
                        deathCounterCalled = true;
                    }
                }

                if (currenthealth >= maxHealth)
                {
                    currenthealth = maxHealth;
                    isFullHealth = true;
                }

                if (currenthealth < maxHealth)
                    isFullHealth = false;
            }
        }

        void HealthBarUpdate(float oldValue, float newValue)
        {
            float currenthealthPct = newValue / maxHealth;
            OnHealthPctChanged(currenthealthPct);
        }

        public void SetPlayerDead()
        {
            playerDead = true;
            GetComponent<GameEvent>().gotKilled = PlayerObj.instance.GetPlayerName();
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

        public bool GetHealthStatus()
        {
            return isFullHealth;
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
