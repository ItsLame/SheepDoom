using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SheepDoom
{
    public class TowerHealthBar : HealthBar
    {
        [SerializeField] private GameObject Tower;
        [SerializeField] private Image foregroundimage;
        [SerializeField] private float updatespeedseconds = 0.5f;

        protected override void Awake()
        {
            base.Awake();
            GetComponentInParent<CapturePointScript>().OnHealthPctChangedTower += HandleHealthChanged;
        }

        protected override void InitHealthBar()
        {
            P_foregroundImage = foregroundimage;
            P_updateSpeedSeconds = updatespeedseconds;
        }
    }
}
