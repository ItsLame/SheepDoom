using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{
    public class Yoshi : Hero
    {
        [SerializeField] private Image heroImage;

        [Client]
        protected override void InitHeroInfo()
        {
            P_heroName = "Yoshi";
            P_heroDesc = "A dragon.. right?";
            P_heroIcon = heroImage.sprite;
        }

        [Client]
        public override void OnClickHero()
        {
            base.OnClickHero();
        }

        [Client]
        public override void SetTaken(bool _isTaken)
        {
            P_isTaken = _isTaken;
        }
    }
}