using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{
    public class Peach : Hero
    {
        [SerializeField] private Image heroIcon;

        [Client]
        protected override void InitHeroInfo()
        {
            P_heroName = "Peach";
            P_heroDesc = "A princess";
            P_heroIcon = heroIcon.sprite;
        }

        [Client]
        public override void OnClickHero()
        {
            base.OnClickHero();
        }
    }
}