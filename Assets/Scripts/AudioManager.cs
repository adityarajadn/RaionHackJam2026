using System;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] musicSounds;
    public Sound[] sfxSounds;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    private void Awake()
    {
        // Setup Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Jika belum ada AudioSource, tambahkan secara otomatis
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true; // Music defaultnya ngeloop
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);
        if (s == null)
        {
            Debug.LogWarning("Music: " + name + " tidak ditemukan!");
            return;
        }

        // Cegah lagu mengulang dari awal jika lagu yang sama sedang dimainkan
        if (musicSource.clip == s.clip)
        {
            if (!musicSource.isPlaying)
            {
                musicSource.Play();
            }
            return;
        }

        musicSource.clip = s.clip;
        musicSource.Play();
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);
        if (s == null)
        {
            Debug.LogWarning("SFX: " + name + " tidak ditemukan!");
            return;
        }

        sfxSource.PlayOneShot(s.clip);
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    public float GetMusicVolume()
    {
        return musicSource.volume;
    }

    public float GetSFXVolume()
    {
        return sfxSource.volume;
    }
}
