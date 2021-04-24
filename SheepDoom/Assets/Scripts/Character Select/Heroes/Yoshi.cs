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

        /*
        public Yoshi(Yoshi comp, bool _isTaken, Image heroImg)
        {
            // for object
            heroImage = heroImg;
            InitHeroInfo();
            // for game object
            comp.InitHeroInfo();
            comp.SetTaken(_isTaken);
            comp.OnClickHero();
        }
        */

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