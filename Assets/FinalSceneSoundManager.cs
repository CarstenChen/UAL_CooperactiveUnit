using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalSceneSoundManager : MonoBehaviour
{
    private static FinalSceneSoundManager instance;
    public static FinalSceneSoundManager Instance { get { return instance; } private set { } }

    public PlayerController player;
    public SoundClip bgmClip;
    public AudioSource bgmAudioSource;
    public AudioSource playerAudioSource;
    public SoundClip[] screamClip;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        PlayBgm(bgmAudioSource);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlayBgm(AudioSource audio)
    {
        SoundPlayer.PlaySound(audio, bgmClip, true);
    }

    public void PlayPlayerSound()
    {
        SoundPlayer.PlaySound(playerAudioSource, screamClip[Random.Range(0, screamClip.Length)], 0, true);
    }
}
