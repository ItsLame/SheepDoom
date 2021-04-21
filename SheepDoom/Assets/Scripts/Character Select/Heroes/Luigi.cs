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
        public Luigi(Luigi comp, bool _isTaken, Image heroImg)
        {
            // for object
            heroImage = heroImg;
            InitHeroInfo();
            // for game object
            comp.InitHeroInfo();
            comp.SetTaken(_isTaken);
            comp.OnClickHero();
        }
        [Client]
        protected override void InitHeroInfo()
        {
            P_heroName = "Luigi";
            P_heroDesc = "A green hat plumber";
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