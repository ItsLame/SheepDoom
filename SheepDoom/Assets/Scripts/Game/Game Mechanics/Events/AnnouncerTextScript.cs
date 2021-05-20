using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SheepDoom
{
    public class AnnouncerTextScript : MonoBehaviour
    {
        //public Text AnnouncerText;
        [SerializeField] TextMeshProUGUI AnnouncerText;
        public float ClearTimer;

        private void Update()
        {
            if (ClearTimer >= 0)
            {
                ClearTimer -= Time.deltaTime;
            }

            if (ClearTimer <= 0)
            {
                AnnouncerText.text = null;
            }
        }
        public void ResetText(float seconds)
        {
            ClearTimer = seconds;
        }
    }
}
