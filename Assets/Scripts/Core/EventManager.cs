using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class EventManager : MonoBehaviour
{
    // Event Handlers
    public delegate void GameStartupHandler();
    public delegate void AudioMixerHandler(MixerGroup _mixerGroup, float _volume);
    public delegate void ARHandler(ARTrackedImagesChangedEventArgs _eventArgs);
    public delegate void ARLevelHandler(ARLevel _level);

    // Events
    public event GameStartupHandler StartupFinished;
    public event AudioMixerHandler MixerGroupVolumeChanged;
    public event ARHandler TrackedImagesChanged;
    public event ARLevelHandler LevelStarted;

    // Trigger
    public void OnGameStartupFinished()
    {
        StartupFinished?.Invoke();
    }

    public void OnMixerGroupVolumeChanged(MixerGroup _mixerGroup, float _volume)
    {
        MixerGroupVolumeChanged?.Invoke(_mixerGroup, _volume);
    }

    public void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs _eventArgs)
    {
        TrackedImagesChanged?.Invoke(_eventArgs);
    }

    public void OnARLevelStarted(ARLevel _startedLevel)
    {
        LevelStarted?.Invoke(_startedLevel);
    }
}
