using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Manager<SoundManager>
{
    public enum BackgroundMusic { Typing, TestArea, Speed1, Speed2, Speed3, Speed4 };
    public List<AudioClip> backgroundMusic = new List<AudioClip>();
    public BackgroundMusic currentMusic;
    public enum SoundEffect { Jump, Collect, PlayerDie, GameOver, Win, Lose, Ready, Go, TypeWriterReturn, Shoot, Explosion};
    public List<AudioClip> soundEffects = new List<AudioClip>();
    public bool soundOn = true;

    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void ChangeBackgroundMusic(BackgroundMusic music)
    {
        if (soundOn)
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
            audioSource.Stop();
            audioSource.clip = backgroundMusic[(int)music];
            audioSource.loop = true;
            audioSource.Play();
            currentMusic = music;
        }
    }

    public void PlaySoundEffect(SoundEffect effect)
    {
        if (soundOn)
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
            audioSource.PlayOneShot(soundEffects[(int)effect]);
        }
    }

    public void StopMusic()
    {
        if (soundOn)
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
            audioSource.Stop();
        }
    }
}
