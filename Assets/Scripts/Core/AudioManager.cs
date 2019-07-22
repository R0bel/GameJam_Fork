using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum MixerGroup
{
    MASTER,
    SFX,
    ATMO,
    MUSIC
}

[RequireComponent(typeof(AudioListener))]
public class AudioManager : MonoBehaviour, IManagedBehaviour
{
    GameManager gameManager;

    [SerializeField]
    private AudioMixer masterMixer;
    [SerializeField]
    private AudioSource sfxSource;
    [SerializeField]
    private AudioSource atmoSource;
    [SerializeField]
    private AudioSource musicSource;
    [SerializeField]
    private float lowPitchRange = 0.95f;
    [SerializeField]
    private float highPitchRange = 1.05f;

    private Coroutine fadeInMusicCoroutine;
    private Coroutine fadeOutMusicCoroutine;
    private Coroutine fadeInAtmoCoroutine;
    private Coroutine fadeOutAtmoCoroutine;

    // called after GameManagers Start function has been called
    public void OnStart(GameManager _manager)
    {
        gameManager = _manager;
    }

    public AudioSource SfxSource
    {
        get
        {
            return sfxSource;
        }
        private set
        {
            sfxSource = value;
        }
    }

    public AudioSource AtmoSource
    {
        get
        {
            return atmoSource;
        }
        private set
        {
            atmoSource = value;
        }
    }

    public AudioSource MusicSource
    {
        get
        {
            return musicSource;
        }
        private set
        {
            musicSource = value;
        }
    }

    /// <summary>
    /// Volume of AudioMixer masterGroup. Method can't be used on Awake or Init
    /// </summary>
    public float MasterVolume
    {
        get
        {
            float volume = 0f;
            masterMixer.GetFloat("Master", out volume);
            return volume;
        }
        set
        {
            masterMixer.SetFloat("Master", value);
            if (gameManager != null)
            {
                gameManager.Events.OnMixerGroupVolumeChanged(MixerGroup.MASTER, value);
            }            
        }
    }

    /// <summary>
    /// Volume of AudioMixer effectsGroup. Method can't be used on Awake or Init
    /// </summary>
    public float SfxVolume
    {
        get
        {
            float volume = 0f;
            masterMixer.GetFloat("Effects", out volume);
            return volume;
        }
        set
        {
            masterMixer.SetFloat("Effects", value);
            if (gameManager != null)
            {
                gameManager.Events.OnMixerGroupVolumeChanged(MixerGroup.SFX, value);
            }
        }
    }

    /// <summary>
    /// Volume of AudioMixer musicGroup. Method can't be used on Awake or Init
    /// </summary>
    public float MusicVolume
    {
        get
        {
            float volume = 0f;
            masterMixer.GetFloat("Music", out volume);
            return volume;
        }
        set
        {
            masterMixer.SetFloat("Music", value);
            if (gameManager != null)
            {
                gameManager.Events.OnMixerGroupVolumeChanged(MixerGroup.MUSIC, value);
            }
        }
    }

    /// <summary>
    /// Volume of AudioMixer atmoGroup. Method can't be used on Awake or Init
    /// </summary>
    public float AtmoVolume
    {
        get
        {
            float volume = 0f;
            masterMixer.GetFloat("Atmo", out volume);
            return volume;
        }
        set
        {
            masterMixer.SetFloat("Atmo", value);
            if (gameManager != null)
            {
                gameManager.Events.OnMixerGroupVolumeChanged(MixerGroup.ATMO, value);
            }
        }
    }

    /// <summary>
    /// Play single sfx audioClip
    /// </summary>
    /// <param name="_clip"></param>
    /// <param name="_pitch"></param>
    public void PlaySingle(AudioClip _clip, float _pitch = 1f)
    {
        sfxSource.clip = _clip;
        sfxSource.pitch = _pitch;
        sfxSource.Play();
    }

    /// <summary>
    /// Play random sfx audioClip with random pitch
    /// </summary>
    /// <param name="_clip"></param>
    /// <param name="_pitch"></param>
    public void PlayRandomSfx(params AudioClip[] _clips)
    {
        int randomIndex = Random.Range(0, _clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        sfxSource.pitch = randomPitch;
        sfxSource.clip = _clips[randomIndex];
        sfxSource.Play();
    }

    /// <summary>
    /// Play atmo audioClip
    /// </summary>
    /// <param name="_clip"></param>
    /// <param name="_isLooping"></param>
    public void PlayAtmo(AudioClip _clip, float _pitch = 1f, bool _isLooping = false)
    {
        atmoSource.clip = _clip;
        atmoSource.loop = _isLooping;
        atmoSource.pitch = _pitch;
        atmoSource.Play();
    }

    /// <summary>
    /// Play atmo audioClip with fade in
    /// </summary>
    /// <param name="_clip"></param>
    /// <param name="_fadeTime"></param>
    /// <param name="_pitch"></param>
    /// <param name="_isLooping"></param>
    public void PlayAtmo(AudioClip _clip, float _fadeTime, float _pitch = 1f, bool _isLooping = false)
    {
        if (fadeInAtmoCoroutine != null) StopCoroutine(fadeInAtmoCoroutine);
        if (fadeOutAtmoCoroutine != null) StopCoroutine(fadeOutAtmoCoroutine);
        atmoSource.clip = _clip;
        atmoSource.loop = _isLooping;
        atmoSource.pitch = _pitch;
        atmoSource.volume = 0;

        fadeInAtmoCoroutine = StartCoroutine(FadeIn(atmoSource, _fadeTime));
    }

    /// <summary>
    /// Play music audioClip
    /// </summary>
    /// <param name="_clip"></param>
    /// <param name="_fadeTime"></param>
    /// <param name="_isLooping"></param>
    public void PlayMusic(AudioClip _clip, float _pitch = 1f, bool _isLooping = false)
    {
        if (fadeInMusicCoroutine != null) StopCoroutine(fadeInMusicCoroutine);
        if (fadeOutMusicCoroutine != null) StopCoroutine(fadeOutMusicCoroutine);
        musicSource.clip = _clip;
        musicSource.loop = _isLooping;
        musicSource.pitch = _pitch;
        musicSource.Play();
    }

    /// <summary>
    /// Play music audioClip with fade in
    /// </summary>
    /// <param name="_clip"></param>
    /// <param name="_fadeTime"></param>
    /// <param name="_isLooping"></param>
    public void PlayMusic(AudioClip _clip, float _fadeTime, float _pitch = 1f, bool _isLooping = false)
    {
        if (fadeInMusicCoroutine != null) StopCoroutine(fadeInMusicCoroutine);
        if (fadeOutMusicCoroutine != null) StopCoroutine(fadeOutMusicCoroutine);
        musicSource.clip = _clip;
        musicSource.loop = _isLooping;
        musicSource.pitch = _pitch;
        musicSource.volume = 0;

        fadeInMusicCoroutine = StartCoroutine(FadeIn(musicSource, _fadeTime));
    }

    /// <summary>
    /// Stop atmo audioClip
    /// </summary>
    public void StopAtmo()
    {
        if (fadeInAtmoCoroutine != null) StopCoroutine(fadeInAtmoCoroutine);
        if (fadeOutAtmoCoroutine != null) StopCoroutine(fadeOutAtmoCoroutine);
        if (atmoSource.isPlaying)
            atmoSource.Stop();
    }

    /// <summary>
    /// Stop atmo audioClip with fade out
    /// </summary>
    /// <param name="_fadeTime"></param>
    public void StopAtmo(float _fadeTime)
    {
        if (fadeInAtmoCoroutine != null) StopCoroutine(fadeInAtmoCoroutine);
        if (fadeOutAtmoCoroutine != null) StopCoroutine(fadeOutAtmoCoroutine);
        if (atmoSource.isPlaying)
        {
            fadeOutAtmoCoroutine = StartCoroutine(FadeOut(atmoSource, _fadeTime));
        }
    }

    /// <summary>
    /// Stop music audioClip
    /// </summary>
    public void StopMusic()
    {
        if (fadeInMusicCoroutine != null) StopCoroutine(fadeInMusicCoroutine);
        if (fadeOutMusicCoroutine != null) StopCoroutine(fadeOutMusicCoroutine);
        if (musicSource.isPlaying)
            musicSource.Stop();
    }

    /// <summary>
    /// Stop music audioClip with fade out
    /// </summary>
    /// <param name="_fadeTime"></param>
    public void StopMusic(float _fadeTime)
    {
        if (fadeInMusicCoroutine != null) StopCoroutine(fadeInMusicCoroutine);
        if (fadeOutMusicCoroutine != null) StopCoroutine(fadeOutMusicCoroutine);
        if (musicSource.isPlaying)
        {
            fadeOutMusicCoroutine = StartCoroutine(FadeOut(musicSource, _fadeTime));
        }
    }

    private IEnumerator FadeIn(AudioSource _audioSource, float _fadeTime)
    {
        float startVolume = _audioSource.volume + 1;
        _audioSource.Play();

        while (_audioSource.volume < 1)
        {
            _audioSource.volume += startVolume * Time.deltaTime / _fadeTime;

            yield return null;
        }
    }

    private IEnumerator FadeOut(AudioSource _audioSource, float _fadeTime)
    {
        float startVolume = _audioSource.volume;

        while (_audioSource.volume > 0)
        {
            _audioSource.volume -= startVolume * Time.deltaTime / _fadeTime;

            yield return null;
        }

        _audioSource.Stop();
        _audioSource.volume = startVolume;
    }
}
