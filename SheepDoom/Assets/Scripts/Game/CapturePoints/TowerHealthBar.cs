using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{
    public class TowerHealthBar : MonoBehaviour
    {
        [SerializeField]
        private Image foregroundimage;
        [SerializeField]
        private float updatespeedseconds = 0.5f;

        // Start is called before the first frame update
        private void Awake()
        {
            GetComponentInParent<CapturePointScript>().OnHealthPctChangedTower += HandleHealthChangedTower;
        }
        private void HandleHealthChangedTower(float pct)
        {
            StartCoroutine(ChangedToPctTower(pct));
        }
        private IEnumerator ChangedToPctTower(float pct)
        {
            float preChangedPct = foregroundimage.fillAmount;
            float elasped1 = 0f;

            while (elasped1 < updatespeedseconds)
            {
                elasped1 += Time.deltaTime;
                foregroundimage.fillAmount = Mathf.Lerp(preChangedPct, pct, elasped1 / updatespeedseconds);
                yield return null;
            }
            foregroundimage.fillAmount = pct;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if(Camera.main)
                transform.LookAt(Camera.main.transform);
            
            transform.Rotate(0, 180, 0);
        }
    }
}
