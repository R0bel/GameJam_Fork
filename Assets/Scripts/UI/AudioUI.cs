using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioUI : MonoBehaviour, IManagedBehaviour
{
    private GameManager gameManager;

    [SerializeField]
    private Slider masterVolumeSlider;
    [SerializeField]
    private Text masterVolumeText;

    [Space(10f)]
    [SerializeField]
    private Slider sfxVolumeSlider;
    [SerializeField]
    private Text sfxVolumeText;

    [Space(10f)]
    [SerializeField]
    private Slider atmoVolumeSlider;
    [SerializeField]
    private Text atmoVolumeText;

    [Space(10f)]
    [SerializeField]
    private Slider musicVolumeSlider;
    [SerializeField]
    private Text musicVolumeText;

    [Space(10f)]
    [SerializeField]
    private AudioClip musicClip;
    [SerializeField]
    private AudioClip sfxClip;
    [SerializeField]
    private AudioClip atmoClip;

    // called after GameManagers Start function has been called
    public void OnStart(GameManager _manager)
    {
        Debug.Log("Init AudioUI");
        gameManager = _manager;
        // OR
        // gameManager = GameManager.Instance;
        gameManager.Events.MixerGroupVolumeChanged += OnMixerGroupValue;

        // start music and atmo sounds
        gameManager.Audio.PlayMusic(musicClip, 5f, 1f, true);
        gameManager.Audio.PlayAtmo(atmoClip, 5f, 1f, true);
        gameManager.Audio.AtmoVolume = -20;
    }

    public void TriggerSfx()
    {
        if (!gameManager.Audio.SfxSource.isPlaying)
        {
            gameManager.Audio.PlaySingle(sfxClip);
        }        
    }

    private void OnMixerGroupValue(MixerGroup _mixerGroup, float _volume)
    {
        int vol = (int)_volume;
        switch (_mixerGroup)
        {
            case MixerGroup.MASTER:
                masterVolumeText.text = $"{(vol + 80)} %";
                masterVolumeSlider.value = _volume;
                break;
            case MixerGroup.SFX:
                sfxVolumeText.text = $"{(vol + 80)} %";
                sfxVolumeSlider.value = _volume;
                break;
            case MixerGroup.ATMO:
                atmoVolumeText.text = $"{(vol + 80)} %";
                atmoVolumeSlider.value = _volume;
                break;
            case MixerGroup.MUSIC:
                musicVolumeText.text = $"{(vol + 80)} %";
                musicVolumeSlider.value = _volume;
                break;
        }
    }

    public void OnMasterVolume(float _value)
    {
        if (gameManager != null)
        {
            gameManager.Audio.MasterVolume = _value;
        }
    }

    public void OnEffectsVolume(float _value)
    {
        if (gameManager != null)
        {
            gameManager.Audio.SfxVolume = _value;
        }
    }

    public void OnAtmoVolume(float _value)
    {
        if (gameManager != null)
        {
            gameManager.Audio.AtmoVolume = _value;
        }
    }

    public void OnMusicVolume(float _value)
    {
        if (gameManager != null)
        {
            gameManager.Audio.MusicVolume = _value;
        }
    }

    private void OnDestroy()
    {
        gameManager.Events.MixerGroupVolumeChanged -= OnMixerGroupValue;
    }
}
