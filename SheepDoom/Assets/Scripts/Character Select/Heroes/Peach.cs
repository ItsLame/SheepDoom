using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{
    public class Peach : Hero
    {
        [SerializeField] private Image heroImage;

        [ClientCallback]
        protected override void InitHeroInfo()
        {
            P_heroName = "Peach";
            P_heroDesc = "A princess";
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