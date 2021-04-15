using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SheepDoom
{
    public class Mario : Hero
    {
        [SerializeField] private Image marioIcon;

        public override void InitHeroInfo()
        {
            Debug.Log("OVERRIDE INIT HERO INFO");

            P_heroName = "Mario";
            P_heroDesc = "A balanced hero that uses melee attacks to defeat enemies";
            P_heroIcon = marioIcon.sprite;
        }

        public override void OnClickHero()
        {
            base.OnClickHero();
        }
    }
}