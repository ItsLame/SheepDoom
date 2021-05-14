using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SheepDoom
{
    public class ShopSkillsIconScript : MonoBehaviour
    {
        [Header("Identifying player")]
        [SerializeField] private float characterID;
        public bool hasSet;

        [Header("Setting icons based on player")]
        [Space(15)]
        [Header("---- Alma----")]
        [SerializeField] private Image AlmaS1;
        [SerializeField] private Image AlmaS2;
        [SerializeField] private Image AlmaU1;
        [SerializeField] private Image AlmaU2;

        [Header("---- Asta----")]
        [SerializeField] private Image AstaS1;
        [SerializeField] private Image AstaS2;
        [SerializeField] private Image AstaU1;
        [SerializeField] private Image AstaU2;

        [Header("----Isa----")]
        [SerializeField] private Image IsaS1;
        [SerializeField] private Image IsaS2;
        [SerializeField] private Image IsaU1;
        [SerializeField] private Image IsaU2;

        public void setIcons()
        {
            characterID = FindMe.instance.P_MyPlayer.GetComponent<PlayerAdmin>().getCharID();
         //   Debug.Log("Char ID: " + characterID);
            hasSet = true;
        }



    }

}
