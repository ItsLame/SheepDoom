using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public string SceneName;
    public bool listenerEnabled;
    public Sound[] sounds;

    //singleton
 //   public static AudioManager instance;

    private void Awake()
    {
        /*
        if (instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);*/
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        //play bgm
        Play("BGMLobby");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log(name + " not found");
            return;
        }

        else
        {
            Debug.Log("Playing : " + name);
            s.source.PlayOneShot(s.clip, s.volume);
        }

    }

    public void StopPlay(string name)
    {
        Sound s = Array.Find(sounds, item => item.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Stop();
    }

    public void enableAudioListener(string scenename)
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(scenename))
        {
            Debug.Log("Enabling sound listener in ");
            Debug.Log("Current scene: " + SceneManager.GetActiveScene().name + " VS " + scenename);
            this.gameObject.GetComponent<AudioListener>().enabled = true;
            listenerEnabled = true;
        }

    }

    public void disableAudioListener(string scenename)
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName(scenename))
        {
            Debug.Log("Disabling sound listener in ");
            Debug.Log("Current scene: " + SceneManager.GetActiveScene().name + " VS " + scenename);
            this.gameObject.GetComponent<AudioListener>().enabled = false;
            listenerEnabled = false;
        }


    }

    public void checkIfInGame()
    {

    }

    private void Update()
    {
        if (!listenerEnabled)
        {
            enableAudioListener(SceneName);
        }

        else if (listenerEnabled)
        {
            disableAudioListener(SceneName);
        }

        checkIfInGame();

    }
}
