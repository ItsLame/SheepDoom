using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{
    public class Yoshi : Hero
    {
        [SerializeField] private Image heroIcon;

        [Client]
        protected override void InitHeroInfo()
        {
            P_heroName = "Yoshi";
            P_heroDesc = "A dragon.. right?";
            P_heroIcon = heroIcon.sprite;
        }

        [Client]
        public override void OnClickHero()
        {
            base.OnClickHero();
        }
    }
}