using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{
    public class Bowser : Hero
    {
        [SerializeField] private Image heroImage;

        /*
        public Bowser(Bowser comp, bool _isTaken, Image heroImg)
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
            P_heroName = "Bowser";
            P_heroDesc = "Plumber's sworn enemy";
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