using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{
    public class Luigi : Hero
    {
        [SerializeField] private Image heroIcon;

        [Client]
        protected override void InitHeroInfo()
        {
            P_heroName = "Luigi";
            P_heroDesc = "A green hat plumber";
            P_heroIcon = heroIcon.sprite;
        }

        [Client]
        public override void OnClickHero()
        {
            base.OnClickHero();
        }
    }
}