using System;
using UnityEngine;
using UnityEngine.Serialization;
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [FormerlySerializedAs("musicSounds")]
    [SerializeField] private Sound[] _musicSounds;
    [FormerlySerializedAs("sfxSounds")]
    [SerializeField] private Sound[] _sfxSounds;
    [Header("Audio Sources")]
    [FormerlySerializedAs("musicSource")]
    [SerializeField] private AudioSource _musicSource;
    [FormerlySerializedAs("sfxSource")]
    [SerializeField] private AudioSource _sfxSource;
    private void Awake()
    {
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
        if (_musicSource == null)
        {
            _musicSource = gameObject.AddComponent<AudioSource>();
        }
        _musicSource.loop = true; 
        _musicSource.playOnAwake = false;
        if (_sfxSource == null)
        {
            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.loop = false;
            _sfxSource.playOnAwake = false;
        }
    }
    public void PlayMusic(string name)
    {
        Sound soundToPlay = Array.Find(_musicSounds, sound => sound.name == name);
        if (soundToPlay == null)
        {
            return;
        }
        if (_musicSource.clip == soundToPlay.clip)
        {
            if (!_musicSource.isPlaying)
            {
                _musicSource.Play();
            }
            return;
        }
        _musicSource.clip = soundToPlay.clip;
        _musicSource.Play();
    }
    public void PlaySFX(string name)
    {
        Sound soundToPlay = Array.Find(_sfxSounds, sound => sound.name == name);
        if (soundToPlay == null)
        {
            return;
        }
        _sfxSource.PlayOneShot(soundToPlay.clip);
    }
    public void SetMusicVolume(float volume)
    {
        _musicSource.volume = volume;
    }
    public void SetSFXVolume(float volume)
    {
        _sfxSource.volume = volume;
    }
    public float GetMusicVolume()
    {
        return _musicSource.volume;
    }
    public float GetSFXVolume()
    {
        return _sfxSource.volume;
    }
}
