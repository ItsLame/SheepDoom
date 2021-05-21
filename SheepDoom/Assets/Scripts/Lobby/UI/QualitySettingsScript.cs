using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualitySettingsScript : MonoBehaviour
{
    public GameObject PostProcessor = null;
    public GameObject NormalLighting = null;
    public GameObject LowQLighting = null;

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);

        if (PostProcessor == null) return;

        if (qualityIndex == 0)
        {
            PostProcessor.SetActive(false);
            LowQLighting.SetActive(true);
            NormalLighting.SetActive(false);
        }

        else if (qualityIndex == 1)
        {
            PostProcessor.SetActive(false);
            LowQLighting.SetActive(false);
            NormalLighting.SetActive(true);
        }

        else if (qualityIndex == 2)
        {
            PostProcessor.SetActive(true);
            LowQLighting.SetActive(false);
            NormalLighting.SetActive(true);
        }

    }

    /*
    public void PostProcessorSetting(int index)
    {
        if (PostProcessor != null)
        {
            if (index == 1 || index == 0)
            {
                PostProcessor.SetActive(false);
            }

            else if (index == 2)    
            {
                PostProcessor.SetActive(true);
            }
        }
    }

    public void LightingSetting(int index)
    {
        if (NormalLighting == null) return;
        if (index == 0)
        {
            LowQLighting.SetActive(true);
            NormalLighting.SetActive(false);
        }

        else if (index > 1)
        {
            LowQLighting.SetActive(false);
            NormalLighting.SetActive(true);
        }
    }*/
}
