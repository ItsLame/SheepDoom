using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SheepDoom
{
    public abstract class HealthBar : MonoBehaviour
    {
        private Image foregroundImage;
        private float updateSpeedSeconds;

        #region Properties

        protected Image P_foregroundImage
        {
            get{return foregroundImage;}
            set{foregroundImage = value;}
        }

        protected float P_updateSpeedSeconds
        {
            get{return updateSpeedSeconds;}
            set{updateSpeedSeconds = value;}
        }

        #endregion

        // Start is called before the first frame update
        protected virtual void Awake()
        {
            InitHealthBar();
        }

        protected abstract void InitHealthBar();

        protected void HandleHealthChanged(float pct)
        {
            if(P_foregroundImage)
                StartCoroutine(ChangedToPct(pct));
        }

        private IEnumerator ChangedToPct(float pct)
        {
            float preChangedPct = P_foregroundImage.fillAmount;
            float elasped1 = 0.0f;

            while (elasped1 < P_updateSpeedSeconds)
            {
                elasped1 += Time.deltaTime;
                P_foregroundImage.fillAmount = Mathf.Lerp(preChangedPct, pct, elasped1 / P_updateSpeedSeconds);
                yield return null;
            }

            P_foregroundImage.fillAmount = pct;
        }

        // Update is called once per frame
        private void LateUpdate()
        {
            if (Camera.main)
            {
                transform.LookAt(Camera.main.transform);
                transform.Rotate(0, 180, 0);
            }
        }
    }
}

