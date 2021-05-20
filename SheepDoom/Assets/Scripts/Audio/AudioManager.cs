using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup BGMMixer;
    [SerializeField] private AudioMixerGroup SFXMixer;
    public bool isBGMPlaying = false;
    public string SceneName;
    public bool listenerEnabled;
    public Sound[] sounds;

    //singleton
    public static AudioManager instance;

    private void Awake()
    {

        if (instance == null)
            instance = this;

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

    private void Start()
    {
        if (AudioManager.instance.isBGMPlaying == false)
        {
            Play("BGMLobby");
            AudioManager.instance.isBGMPlaying = true;
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

    public void enableAudioListener(string scenename)
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(scenename))
        {
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Game Networking Scene 1.0")) return;
            this.gameObject.GetComponent<AudioListener>().enabled = true;
            listenerEnabled = true;
        }
    }

    public void disableAudioListener(string scenename)
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName(scenename))
        {
            this.gameObject.GetComponent<AudioListener>().enabled = false;
            listenerEnabled = false;
        }
    }

    public void sendToGame()
    {
        StopPlay("BGMLobby");
        AudioManager.instance.gameObject.GetComponent<AudioListener>().enabled = false;
        listenerEnabled = false;
    }


    public void sendToLobby()
    {
        Play("BGMLobby");
        AudioManager.instance.gameObject.GetComponent<AudioListener>().enabled = true;
        listenerEnabled = true;
    }

    private void Update()
    {
        if (!listenerEnabled)
            enableAudioListener(SceneName);
        else if (listenerEnabled)
            disableAudioListener(SceneName);
    }
}
