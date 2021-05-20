using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class AudioManagerGame : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup BGMMixer;
    [SerializeField] private AudioMixerGroup SFXMixer;
    public Sound[] sounds;
    
    //singleton
    public static AudioManagerGame instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(this.gameObject);
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            
            if(s.name == "BGMLobby" || s.name == "BGMGame")
                s.source.outputAudioMixerGroup = BGMMixer;
            else
                s.source.outputAudioMixerGroup = SFXMixer;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        else
            s.source.PlayOneShot(s.clip, s.volume);
    }

    public void StopPlay(string name)
    {
        Sound s = Array.Find(sounds, item => item.name == name);
        if (s == null)
            return;
        else
            s.source.Stop();
    }
}
