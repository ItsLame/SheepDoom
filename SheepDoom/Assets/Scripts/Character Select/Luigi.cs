using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SheepDoom
{
    public class Luigi : Hero
    {
        [SerializeField] private Image luigiIcon;

        protected override void InitHeroInfo()
        {
            Debug.Log("OVERRIDE INIT HERO INFO");

            P_heroName = "Luigi";
            P_heroDesc = "A green hat plumber";
            P_heroIcon = luigiIcon.sprite;
        }

        public override void OnClickHero()
        {
            base.OnClickHero();
        }
    }
}