using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SheepDoom
{
    public class PlayerHealthBar : HealthBar
    {
        [SerializeField] private Image foregroundimage;
        [SerializeField] private float updatespeedseconds = 0.1f;

        protected override void Awake()
        {
            base.Awake();
            GetComponentInParent<PlayerHealth>().OnHealthPctChanged += HandleHealthChanged;
        }

        protected override void InitHealthBar()
        {
            P_foregroundImage = foregroundimage;
            P_updateSpeedSeconds = updatespeedseconds;
        }
    }
}