using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*namespace SheepDoom
{
    public class BaseConsortiumHPBar : HealthBar
    {
        [SerializeField] private Image foregroundimage;
        [SerializeField] private float updatespeedseconds = 0.5f;

        protected override void Awake()
        {
            base.Awake();
            GetComponentInParent<LeftMinionBehaviour>().OnHealthPctChanged += HandleHealthChanged;
        }

        void OnDestroy()
        {
            GetComponentInParent<LeftMinionBehaviour>().OnHealthPctChanged -= HandleHealthChanged;
        }

        protected override void InitHealthBar()
        {
            P_foregroundImage = foregroundimage;
            P_updateSpeedSeconds = updatespeedseconds;
        }
    }  
}*/