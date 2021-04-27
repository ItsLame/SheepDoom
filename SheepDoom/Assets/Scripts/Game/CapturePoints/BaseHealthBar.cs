using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SheepDoom
{
    public class BaseHealthBar : HealthBar
    {
        [SerializeField] private Image foregroundimage;
        [SerializeField] private float updatespeedseconds = 0.5f;

        protected override void Awake()
        {
            base.Awake();
            GetComponentInParent<CaptureBaseScript>().OnHealthPctChangedTower += HandleHealthChanged;
        }

        protected override void InitHealthBar()
        {
            P_foregroundImage = foregroundimage;
            P_updateSpeedSeconds = updatespeedseconds;
        }
    }
}
