using UnityEngine;
using UnityEngine.Audio;
using System;


// This just controls the volume of game music and sound effects.
// That we will have.
public class VolumeControl : MonoBehaviour
{
    public AudioMixer AudioMixer;
    public AudioMixerGroup Music, SFX;

    public AudioClip MenuMusic, GameMusic;

    public static float musicVolume = 0f, sfxVolume = 0f;
    public static bool musicMuted, sfxMuted;


    void Start()
    {
        AudioMixer.GetFloat("MusicVolume", out musicVolume);
        AudioMixer.GetFloat("SFXVolume", out sfxVolume);
    }


    // Make sure we are playing the correct music for the right scene.
    // Game music for game level
    // Mike's creepy singing for the menu otherwise.
    void OnLevelWasLoaded(int level)
    {
        AudioSource audio = GetComponent<AudioSource>();
        AudioClip music = (level == 2 || level == 3) ? GameMusic : MenuMusic;
        if (audio.clip != music)
        {
            audio.clip = music;
            audio.Play();
        }
    }

    // Pass a float to change the MUSIC volume; pass a bool to mute/unmute.
    public void SetMusicVolume (bool muted)
    {
        musicMuted = muted;
        AudioMixer.SetFloat("MusicVolume", muted ? -80 : musicVolume);
    }

    public void SetMusicVolume (float volume)
    {
        SetMusicVolume(volume, musicMuted);
    }

    public void SetMusicVolume(float volume, bool muted)
    {
        musicVolume = volume;
        SetMusicVolume(muted);
    }


    //Pass a float to change the SFX volume; pass a bool to mute/unmute.
    public void SetSFXVolume(bool muted)
    {
        sfxMuted = muted;
        AudioMixer.SetFloat("SFXVolume", muted ? -80 : sfxVolume);
    }

    public void SetSFXVolume(float volume)
    {
        SetSFXVolume(volume, sfxMuted);
    }

    public void SetSFXVolume(float volume, bool muted)
    {
        sfxVolume = volume;
        SetSFXVolume(muted);
    }
}
