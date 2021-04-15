using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{
    public class Mario : Hero
    {
        [SerializeField] private Image marioIcon;

        [Client]
        protected override void InitHeroInfo()
        {
            Debug.Log("OVERRIDE INIT HERO INFO");

            P_heroName = "Mario";
            P_heroDesc = "A red hat plumber";
            P_heroIcon = marioIcon.sprite;
        }

        [Client]
        public override void OnClickHero()
        {
            base.OnClickHero();
        }
    }
}