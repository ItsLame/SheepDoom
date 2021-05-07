using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SheepDoom
{
    public class NeutralCreepHealthBar : HealthBar
    {
        [SerializeField] private Image foregroundimage;

        protected override void Awake()
        {
            base.Awake();
 //           GetComponentInParent<NeutralCreepScript>().OnHealthPctChanged += HandleHealthChanged;
        }

        protected override void InitHealthBar()
        {
   //         P_foregroundImage = foregroundimage;
   //         P_updateSpeedSeconds = updatespeedseconds;
        }
    }
}
