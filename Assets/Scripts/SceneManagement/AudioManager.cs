using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip intro1Audio;
    [SerializeField] private AudioClip intro2Audio;

    [Header("Death Music")]
    [SerializeField] private AudioSource deathMusicSource;
    [SerializeField] private AudioClip deathMusic;

    [Header("SFX")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip dashSFX;
    [SerializeField] private AudioClip castSFX;
    [SerializeField] private AudioClip deathSFX;
    [SerializeField] private AudioClip enemyDeathSFX;

    private void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopBackgroundMusic()
    {
        musicSource.Stop();
    }

    public void PlayDeathMusic()
    {
        deathMusicSource.clip = deathMusic;
        deathMusicSource.loop = true;
        deathMusicSource.Play();
    }

    public void StopDeathMusic()
    {
        deathMusicSource.Stop();
    }

    public void PlayIntro1()
    {
        sfxSource.PlayOneShot(intro1Audio);
    }

    public void PlayIntro2()
    {
        sfxSource.PlayOneShot(intro2Audio);
    }

    public void PlayDash() => sfxSource.PlayOneShot(dashSFX);
    public void PlayCast() => sfxSource.PlayOneShot(castSFX);
    public void PlayDeathSFX() => sfxSource.PlayOneShot(deathSFX);

    public void PlayEnemyDeath()
    {
        sfxSource.PlayOneShot(enemyDeathSFX);
    }
}