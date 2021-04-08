using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField]
        private float maxHealth = 100.0f;

        public float currenthealth;

        //public event Action<float> OnHealthPctChanged = delegate { };

        public bool isFullHealth;

        private void Start()
        {
            currenthealth = maxHealth;
        }

        public void modifyinghealth(float amount)
        {
            currenthealth += amount;

        //    float currenthealthPct = currenthealth / maxHealth;
        //    OnHealthPctChanged(currenthealthPct);
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
            //this.gameObject.GetComponent<PlayerRespawn>().isDead = true;

            //StartCoroutine(TimeBeforeDeath());
            Debug.Log("health: ded");
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
        }

        public float getHealth()
        {
            return currenthealth;
        }
    }
