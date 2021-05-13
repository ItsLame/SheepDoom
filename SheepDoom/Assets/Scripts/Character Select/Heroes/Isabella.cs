using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{
    public class Isabella : Hero
    {
        [SerializeField] private Image heroImage;
        [SerializeField] private GameObject SkillsPanel;

        [ClientCallback]
        protected override void InitHeroInfo()
        {
            P_heroName = "Isabella Licht";
            P_heroDesc = "Holy Saint ";
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