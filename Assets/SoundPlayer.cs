using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class SoundClip
{
    public AudioClip clip;
    public float volume;
    public bool loop;
}

public static class SoundPlayer
{
    public static void PlaySound(AudioSource source, SoundClip soundClip, bool canOverrideCurrentSound)
    {
        if (!canOverrideCurrentSound)
        {
            if (source.isPlaying) return;
        }

        source.clip = soundClip.clip;
        source.volume = soundClip.volume;
        source.loop = soundClip.loop;
        source.time = 0;
        source.Play();
    }

    public static void PlaySound(AudioSource source, SoundClip soundClip, float startTime, bool canOverrideCurrentSound)
    {
        if (!canOverrideCurrentSound)
        {
            if (source.isPlaying) return;
        }


        source.clip = soundClip.clip;
        source.volume = soundClip.volume;
        source.loop = soundClip.loop;
        source.time = startTime;
        source.Play();
    }
    public static void PlaySound(AudioSource source, AudioClip clip, float volume, bool loop, bool canOverrideCurrentSound)
    {
        if (!canOverrideCurrentSound)
        {
            if (source.isPlaying) return;
        }

        source.clip = clip;
        source.volume = volume;
        source.loop = loop;
        source.time = 0;
        source.Play();
    }

    public static void StopSound(AudioSource source)
    {
        source.Stop();
        source.clip = null;
        source.volume = 1;
        source.loop = false;

    }
}
