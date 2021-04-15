using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SheepDoom
{
    public class Yoshi : Hero
    {
        [SerializeField] private Image yoshiIcon;

        protected override void InitHeroInfo()
        {
            Debug.Log("OVERRIDE INIT HERO INFO");

            P_heroName = "Yoshi";
            P_heroDesc = "A dragon.. right?";
            P_heroIcon = yoshiIcon.sprite;
        }

        public override void OnClickHero()
        {
            base.OnClickHero();
        }
    }
}