using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Mirror;

namespace SheepDoom
{
    public class RangeIndicator :   NetworkBehaviour
    {
        private HoldButton otherScipt;
        private bool Get;
        public Image indicatorRangeCircle;
        private bool haveauthority;
        // Start is called before the first frame update
        void Start()
        {
            if (!hasAuthority) return;
            else if(hasAuthority)
            {
                haveauthority = true;
            }

        }

        // Update is called once per frame
        [Client]
        void Update()
        {
            if(haveauthority == true)
            {
                Get = HoldButton.GivingToOtherScript.rangeindicatorchecker;

                if (Get == true)
                {
                    indicatorRangeCircle.GetComponent<Image>().enabled = true;
                }
                else if (Get == false)
                {
                    indicatorRangeCircle.GetComponent<Image>().enabled = false;
                }
            }
        }
    }
}

