using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{
    public class Peach : Hero
    {
        [SerializeField] private Image peachIcon;

        [Client]
        protected override void InitHeroInfo()
        {
            Debug.Log("OVERRIDE INIT HERO INFO");

            P_heroName = "Peach";
            P_heroDesc = "A princess";
            P_heroIcon = peachIcon.sprite;
        }

        [Client]
        public override void OnClickHero()
        {
            base.OnClickHero();
        }
    }
}