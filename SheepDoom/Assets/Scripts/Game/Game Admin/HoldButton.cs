using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Mirror;

namespace SheepDoom
{
    public class HoldButton : MonoBehaviour
    {

        private bool pointerdown;
        public float requiredholdtime;
        private float pointerdowntimer;
        private bool checkifclick;
        [Header("UI Attack Buttons")]
        [Space(15)]

        [HideInInspector]
        public bool playerattackcheck;
        [HideInInspector]
        public bool rangeindicatorchecker;
        public static HoldButton GivingToOtherScript;
        private PlayerAttachToButtons playerattachedbutton;

        public void CheckPointer()
        {
            checkifclick = true;
        }
        public void CheckPointerUp()
        {
            if (pointerdowntimer < requiredholdtime && pointerdowntimer != 0)
            {
                Debug.Log("fireTimer is" + pointerdowntimer);
                playerattackcheck = true;
                GivingToOtherScript = this;
                StartCoroutine(ExampleCoroutine());
            }
            if(pointerdowntimer == 0)
            {
                rangeindicatorchecker = false;
                GivingToOtherScript = this;
            }
        }
        IEnumerator ExampleCoroutine()
        {
            yield return new WaitForSeconds(0.0001f);
            playerattackcheck = false;
            GivingToOtherScript = this;
            reset();

        }
        public void Update()
        {
            if (checkifclick)
            {
                pointerdowntimer += Time.deltaTime;

                if (pointerdowntimer >= requiredholdtime)
                {
                    rangeindicatorchecker = true;
                    GivingToOtherScript = this;

                    reset();
                }
            }
        }
        public void reset()
        {
            checkifclick = false;
            pointerdowntimer = 0;
            playerattackcheck = false;
            GivingToOtherScript = this;
        }

    }
}

