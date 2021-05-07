using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{
    public class Mario : Hero
    {
        [SerializeField] private Image heroImage;

        [ClientCallback]
        protected override void InitHeroInfo()
        {
            P_heroName = "Mario";
            P_heroDesc = "A red hat plumber";
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