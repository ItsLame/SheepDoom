using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{
    public class Luigi : Hero
    {
        [SerializeField] private Image heroImage;

        [ClientCallback]
        protected override void InitHeroInfo()
        {
            P_heroName = "Luigi";
            P_heroDesc = "A green hat plumber";
            P_heroIcon = heroImage.sprite;
        }

        [ClientCallback]
        public override void OnClickHero()
        {
            base.OnClickHero();
        }

        [ClientCallback]
        public override void SetTaken(bool _isTaken)
        {
            P_isTaken = _isTaken;
        }
    }
}