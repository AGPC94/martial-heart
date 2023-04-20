using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManagerBrackeys : MonoBehaviour
{
    Sound[] UISounds;

    public String currentMusic;

    public Sound[] sounds;

    public static AudioManagerBrackeys instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }    
        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        Debug.Log("Sound: " + name + " is sounding!");

        s.source.Play();
    }
    public void StopPlaying(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        Debug.Log("Sound: " + name + " stop!");

        s.source.Stop();

        //s.source.volume = s.volume * (1f + Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        //s.source.pitch = s.pitch * (1f + Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));


    }

    public void PlayMusic(string name)
    {
        if (currentMusic != name)
            StopPlaying(currentMusic);

        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        Debug.Log("Sound: " + name + " is sounding!");

        if (!s.source.isPlaying)
        {
            currentMusic = name;
            s.source.Play();
        }
    }

    public void StopMusic()
    {
        StopPlaying(currentMusic);
    }

    public void AdjustVolume(string name, float volume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        Debug.Log("Sound: " + name + " is sounding!");

        volume = Mathf.Clamp(volume, 0, 1);

        s.volume = volume;
        s.source.volume = volume;
    }
}
