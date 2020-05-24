using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;

    public static AudioManager instance;
    double referenceTime = 0;
    List<double> batchedTimes = new List<double>(); // batched sounds playing times

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
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
        Debug.Log("Playing " + name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " not found");
        }
        if (s.loop && s.source.isPlaying)
        {
            // pass
        }
        else
        {
            s.source.Play();
        }
    }

    public void PlayTheme(string name, double align = 4, bool batch = false)
    {
        double playTime;
        if (referenceTime == 0)
        {
            referenceTime = AudioSettings.dspTime;
            playTime = referenceTime;
            Debug.Log("Setting audio reference time to " + referenceTime);
        }
        else
        {
            double diff = AudioSettings.dspTime - referenceTime;
            double wait = align - (diff % align);
            playTime = AudioSettings.dspTime + wait;
        }
        if (batch)
        {
            playTime = BatchTime(playTime);
            batchedTimes.Add(playTime);
            Debug.Log("Batched " + name + " to " + playTime + " s");
        }
        else
        {
            Debug.Log("Playing " + name);
        }

        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " not found");
        }
        if (s.loop && s.source.isPlaying)
        {
            // pass
        }
        else
        {
            s.source.PlayScheduled(playTime);
        }
    }

    double BatchTime(double playTime)
    {
        for (int i = batchedTimes.Count - 1; i >= 0; --i)
        {
            if (batchedTimes[i] == playTime)
            {
                playTime += 1f;
                playTime = BatchTime(playTime); // go through the batch again
            }
            else if (batchedTimes[i] < playTime)
            {
                return playTime; // done
            }
        }

        return playTime;
    }
}
