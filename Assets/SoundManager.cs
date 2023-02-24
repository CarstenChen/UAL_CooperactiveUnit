using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance { get { return instance; } private set { } }

    public PlayerController player;
    public SoundClip bgmClip;
    public SoundClip horrorClip;
    public SoundClip[] screamClip;
    public SoundClip sanClip;
    public SoundClip bodyChangeClip;
    public SoundClip mainStoryClip;
    public SoundClip mainGateClip;
    public SoundClip monsterClip;
    public AudioSource bgmAudioSource;
    public AudioSource playerAudioSource;
    public AudioSource collectibleAudioSource;
    public AudioSource monsterAudioSource;

    protected bool previousOnCatchingState;
    protected bool bgmChanged = true;

    protected float previousbgmTime = 0f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void Start()
    {
        PlayBgm(bgmAudioSource);

    }
    // Update is called once per frame
    void Update()
    {
        bgmChanged = AIDirector.Instance.onCatchingState == previousOnCatchingState ? false : transform;

        PlayBgm(bgmAudioSource);
        PlayPlayerSound(playerAudioSource);


        previousOnCatchingState = AIDirector.Instance.onCatchingState;
    }

    void PlayBgm(AudioSource audio)
    {
        if (!bgmChanged) return;
        if (AIDirector.Instance.onCatchingState)
        {
            previousbgmTime = bgmAudioSource.time;
            SoundPlayer.PlaySound(audio, horrorClip, true);
        }
        else
        {
            SoundPlayer.PlaySound(audio, bgmClip, previousbgmTime, true);
        }
    }
    public void PlayMonsterSound()
    {
        SoundPlayer.PlaySound(monsterAudioSource, monsterClip, true);
    }

    public void StopMonsterSound()
    {
        SoundPlayer.StopSound(monsterAudioSource);
    }

    public void PlayBodyChangeSound()
    {
        SoundPlayer.PlaySound(playerAudioSource, bodyChangeClip, true);
    }

    public void PlayMainGateSound()
    {
        SoundPlayer.PlaySound(collectibleAudioSource, mainGateClip, true);
    }
    void PlayPlayerSound(AudioSource audio)
    {
        if (AIDirector.Instance.playerScreamOnce)
        {
            SoundPlayer.PlaySound(audio, screamClip[Random.Range(0, screamClip.Length)], 0, true);
        }
    }

    public void PlaySanCollectSound()
    {
        SoundPlayer.PlaySound(collectibleAudioSource, sanClip, true);
    }

    public void PlayMainStoryTriggerSound()
    {
        SoundPlayer.PlaySound(collectibleAudioSource, mainStoryClip, true);
    }
}


