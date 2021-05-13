using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{
    public class Alma : Hero
    {
        [SerializeField] private Image heroImage;
        [SerializeField] private GameObject SkillsPanel;

        [ClientCallback]
        protected override void InitHeroInfo()
        {
            P_heroName = "Alma Blanc";
            P_heroDesc = "Error 305";
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