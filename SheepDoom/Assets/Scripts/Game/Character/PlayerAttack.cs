using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace SheepDoom
{
    public class PlayerAttack : NetworkBehaviour
    {
        [Header("PlayerID")]
        [SerializeField]
        private float charID;

        [Header("For checking if player purchased special and ulti")]
        [Space(15)]
        public bool hasPurchasedSpecial = false;
        public bool hasPurchasedUlti = false;

        [Header("Bools to check which type of skills player chose")]
        [Space(15)]
        public bool AlternateSpecial = false;
        public bool AlternateUlti = false;

        //skillcd basetrackers
        [SerializeField]
        private float cooldown1, cooldown2, cooldown3;

        [Space(15)]
        //cooldown 1 cd multiplier
        [SerializeField]
        private float cooldown1Multiplier;
        [SerializeField]
        private float cooldown1MultiplerTimer;
        private float cooldown1MultiplierTimerInGame;
        private bool cooldown1MultiplierActive;
        private bool resetNormalAtk = false;

        [Header("Skillcd values to be used n manipulated in game")]
 //       [SerializeField]
        private float cooldown1_inGame, cooldown2_inGame, cooldown3_inGame;
        private bool cooldown1Happening;

        [Space(15)]
        //Melee
        [SerializeField] public NetworkAnimator networkAnimator;

        public override void OnStartClient()
        {
            if (!hasAuthority) return;
            cooldown1_inGame = 0;
            cooldown2_inGame = 0;
            cooldown3_inGame = 0;

            cooldown1MultiplierActive = false;
            cooldown1MultiplierTimerInGame = cooldown1MultiplerTimer;

            //get playerID
            charID = this.gameObject.GetComponent<PlayerAdmin>().getCharID();
        }

        public void AttackClick()
        {
            if (!hasAuthority) return;

            //disable attack when sleeped or stopped
            if (this.gameObject.GetComponent<CharacterMovement>().isSleeped || this.gameObject.GetComponent<CharacterMovement>().isStopped)
            {
                return;
            }

            if (cooldown1_inGame <= 0 && !GetComponent<PlayerHealth>().isPlayerDead())
            {
                if (charID == 1)
                {
                    networkAnimator.SetTrigger("Attack");
                    StartCoroutine(Character1Attack());
                }
                else if (charID == 2)
                {
                    networkAnimator.SetTrigger("AstrarothAttack");
                    StartCoroutine(Character2Attack());
                }

                else if (charID == 3)
                {
                    networkAnimator.SetTrigger("IsabellaAttack");
                    StartCoroutine(Character3Attack());
                }
                IEnumerator Character1Attack()
                {
                    yield return new WaitForSeconds(0.1f);
                    Character1 comp = GetComponent<Character1>();
                    comp.normalAtk();
                    cooldown1_inGame = cooldown1 * cooldown1Multiplier;
                }
                IEnumerator Character2Attack()
                {
                    yield return new WaitForSeconds(0.1f);
                    Character2 comp = GetComponent<Character2>();
                    comp.normalAtk(cooldown1MultiplierActive);
                    cooldown1_inGame = cooldown1;
                }
                IEnumerator Character3Attack()
                {
                    yield return new WaitForSeconds(0.1f);
                    Character3 comp = GetComponent<Character3>();
                    comp.normalAtk();
                    cooldown1_inGame = cooldown1;
                }
            }
        }

        public void SpecialSkillClick()
        {
            if (!hasAuthority) return;

            //disable attack when sleeped or stopped
            if (this.gameObject.GetComponent<CharacterMovement>().isSleeped || this.gameObject.GetComponent<CharacterMovement>().isStopped)
            {
                return;
            }

            if (cooldown2_inGame <= 0 && !GetComponent<PlayerHealth>().isPlayerDead())
            {
                if(charID == 1)
                {
                    Character1 comp = GetComponent<Character1>();
                    if (hasPurchasedSpecial)
                        comp.SpecialAtk(AlternateSpecial);
                }

                else if(charID == 2)
                {
                    Character2 comp = GetComponent<Character2>();
                    if(hasPurchasedSpecial)
                    {
                        if (!AlternateSpecial)
                            comp.SpecialAtk();
                        else if (AlternateSpecial && !cooldown1Happening)
                        {
                            cooldown1MultiplierTimerInGame = cooldown1MultiplerTimer;
                            cooldown1MultiplierActive = true;
                            cooldown1 = (cooldown1 * cooldown1Multiplier) + 0.05f;
                        }
                    }
                }

                else if (charID == 3)
                {
                    Character3 comp = GetComponent<Character3>();
                    if (hasPurchasedSpecial)
                        comp.SpecialAtk(AlternateSpecial);
                }
                cooldown2_inGame = cooldown2;
            }
        }

        public void UltiClick()
        {
            if (!hasAuthority) return;


            //disable attack when sleeped or stopped
            if (this.gameObject.GetComponent<CharacterMovement>().isSleeped || this.gameObject.GetComponent<CharacterMovement>().isStopped)
            {
                return;
            }

            if (cooldown3_inGame <= 0 && !GetComponent<PlayerHealth>().isPlayerDead())
            {
                if(charID == 1)
                {
                    Character1 comp = GetComponent<Character1>();
                    if(hasPurchasedUlti)
                        comp.UltiAtk(AlternateUlti);
                }
                else if(charID == 2)
                {
                    Character2 comp = GetComponent<Character2>();
                    if(hasPurchasedUlti)
                        comp.UltiAtk(AlternateUlti);
                }

                else if (charID == 3)
                {
                    Character3 comp = GetComponent<Character3>();
                    if (hasPurchasedUlti)
                        comp.UltiAtk(AlternateUlti);
                }

                cooldown3_inGame = cooldown3;
            }
        }

        [Client]
        public void PlayerHasPurchasedSpecial(bool _isAlt)
        {
            if (!hasAuthority) return;
            hasPurchasedSpecial = true;
            AlternateSpecial = _isAlt;
        }

        [Client]
        public void PlayerHasPurchasedUlti(bool _isAlt)
        {
            if (!hasAuthority) return;
            hasPurchasedUlti = true;
            AlternateUlti = _isAlt;
        }

        // Update is called once per frame
        void Update()
        {
            //if not 0, reduce cd per second
            if (isClient && hasAuthority)
            {
                if (cooldown1_inGame >= 0)
                {
                    cooldown1Happening = true;
                    cooldown1_inGame -= Time.deltaTime;
                }
                else if (cooldown1_inGame <= 0)
                    cooldown1Happening = false;
                // --------------------------------------------------------------------//

                if (cooldown1MultiplierActive)
                    cooldown1MultiplierTimerInGame -= Time.deltaTime;

                //remove atkspd buff once times up
                if (cooldown1MultiplierTimerInGame <= 0 && cooldown1MultiplierActive)
                    resetNormalAtk = true;

                if (resetNormalAtk)
                {
                    normalAtkNormalize();
                    resetNormalAtk = false;
                }

                if (cooldown2_inGame >= 0)
                    cooldown2_inGame -= Time.deltaTime;

                if (cooldown3_inGame >= 0)
                    cooldown3_inGame -= Time.deltaTime;
            }
        }

        public void normalAtkNormalize()
        {
            cooldown1MultiplierActive = false;
            cooldown1 = (cooldown1 - 0.05f) / cooldown1Multiplier;
     //       Debug.Log("Atk Spd Buff ended");
        }
    }
}
