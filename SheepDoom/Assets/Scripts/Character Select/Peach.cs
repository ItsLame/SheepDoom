using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SheepDoom
{
    public class Peach : Hero
    {
        [SerializeField] private Image peachIcon;

        protected override void InitHeroInfo()
        {
            Debug.Log("OVERRIDE INIT HERO INFO");

            P_heroName = "Peach";
            P_heroDesc = "A princess";
            P_heroIcon = peachIcon.sprite;
        }

        public override void OnClickHero()
        {
            base.OnClickHero();
        }
    }
}