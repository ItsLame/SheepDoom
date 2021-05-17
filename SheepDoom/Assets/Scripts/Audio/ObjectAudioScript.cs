using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

//https://answers.unity.com/questions/1320031/having-multiple-audio-sources-in-a-single-object.html
public class ObjectAudioScript : MonoBehaviour
{
    public AudioClip Sound1;
    public AudioClip Sound2;
    public AudioClip Sound3;
    public AudioSource object_sound;

    public float soundInterval1;
    public float soundCD;
    public void playSound1()
    {
        object_sound.clip = Sound1;
        object_sound.PlayOneShot(object_sound.clip, object_sound.volume);
    }

    public void playSound2()
    {
        object_sound.clip = Sound2;
        object_sound.PlayOneShot(object_sound.clip, object_sound.volume);
    }

    //for reoccuring sound
    public void playSound3()
    {
        soundCD += Time.deltaTime;

        if (soundCD >= soundInterval1)
        {
            object_sound.clip = Sound3;
            object_sound.PlayOneShot(object_sound.clip, object_sound.volume);
            soundCD = 0;
        }
    }
    
    //stop
    public void playStop()
    {
        object_sound.Stop();
    }
}