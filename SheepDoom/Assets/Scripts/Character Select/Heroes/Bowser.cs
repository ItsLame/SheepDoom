using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{
    public class Bowser : Hero
    {
        [SerializeField] private Image heroIcon;

        [Client]
        protected override void InitHeroInfo()
        {
            P_heroName = "Bowser";
            P_heroDesc = "Plumber's sworn enemy";
            P_heroIcon = heroIcon.sprite;
        }

        [Client]
        public override void OnClickHero()
        {
            base.OnClickHero();
        }
    }
}