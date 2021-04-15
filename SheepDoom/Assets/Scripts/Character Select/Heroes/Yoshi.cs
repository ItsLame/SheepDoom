using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{
    public class Yoshi : Hero
    {
        [SerializeField] private Image yoshiIcon;

        [Client]
        protected override void InitHeroInfo()
        {
            Debug.Log("OVERRIDE INIT HERO INFO");

            P_heroName = "Yoshi";
            P_heroDesc = "A dragon.. right?";
            P_heroIcon = yoshiIcon.sprite;
        }

        [Client]
        public override void OnClickHero()
        {
            base.OnClickHero();
        }
    }
}