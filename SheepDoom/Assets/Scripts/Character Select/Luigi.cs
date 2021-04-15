using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{
    public class Luigi : Hero
    {
        [SerializeField] private Image luigiIcon;

        [Client]
        protected override void InitHeroInfo()
        {
            Debug.Log("OVERRIDE INIT HERO INFO");

            P_heroName = "Luigi";
            P_heroDesc = "A green hat plumber";
            P_heroIcon = luigiIcon.sprite;
        }

        [Client]
        public override void OnClickHero()
        {
            base.OnClickHero();
        }
    }
}