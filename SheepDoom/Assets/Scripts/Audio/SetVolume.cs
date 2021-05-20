using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SetVolume : MonoBehaviour
{
    public AudioMixer SDMixer;

    public void SetVolumeLevel(float _value)
    {
        SDMixer.SetFloat("SDVol", Mathf.Log10(_value) * 20);
    }

    public void SetBGMLevel(float _value)
    {
        SDMixer.SetFloat("SDBgm", Mathf.Log10(_value) * 20);
    }

    public void SetSFXLevel(float _value)
    {
        SDMixer.SetFloat("SDSfx", Mathf.Log10(_value) * 20);
    }
}
