using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{
    public class Astharoth : Hero
    {
        [SerializeField] private Image heroImage;
        [SerializeField] private GameObject SkillsPanel;

        [ClientCallback]
        protected override void InitHeroInfo()
        {
            P_heroName = "Astharoth Schwarz";
            P_heroDesc = "A Devil in Disguise";
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