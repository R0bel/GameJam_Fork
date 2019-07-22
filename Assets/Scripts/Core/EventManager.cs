using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    // Event Handlers
    public delegate void GameStartupHandler();
    public delegate void AudioMixerHandler(MixerGroup _mixerGroup, float _volume);

    // Events
    public event GameStartupHandler StartupFinished;
    public event AudioMixerHandler MixerGroupVolumeChanged;

    // Trigger
    public void OnGameStartupFinished()
    {
        StartupFinished?.Invoke();
    }

    public void OnMixerGroupVolumeChanged(MixerGroup _mixerGroup, float _volume)
    {
        MixerGroupVolumeChanged?.Invoke(_mixerGroup, _volume);
    }
}
