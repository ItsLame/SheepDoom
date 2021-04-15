﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SheepDoom
{
    public class Bowser : Hero
    {
        [SerializeField] private Image bowserIcon;

        protected override void InitHeroInfo()
        {
            Debug.Log("OVERRIDE INIT HERO INFO");

            P_heroName = "Bowser";
            P_heroDesc = "Plumber's sworn enemy";
            P_heroIcon = bowserIcon.sprite;
        }

        public override void OnClickHero()
        {
            base.OnClickHero();
        }
    }
}