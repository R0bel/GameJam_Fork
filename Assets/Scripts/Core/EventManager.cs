using Photon.Realtime;
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
    public delegate void CharacterHandler(Character _char);
    public delegate void CharacterRegisterHandler(GameObject _character);

    public delegate void NetworkHandler();
    public delegate void NetworkDisconnectHandler(DisconnectCause _cause);
    public delegate void NetworkRoomHandler(Room _room);
    public delegate void NetworkPlayerHandler(Player _player);
    public delegate void NetworkPlayerPropertiesHandler(Player _target, ExitGames.Client.Photon.Hashtable _changedProps);
    public delegate void NetworkRoomPropertiesHandler(ExitGames.Client.Photon.Hashtable _changedProps);
    public delegate void NetworkErrorHandler(short _code, string _msg);

    public delegate void PointHandler(int _points);

    // Events
    public event GameStartupHandler StartupFinished;
    public event AudioMixerHandler MixerGroupVolumeChanged;
    public event ARHandler TrackedImagesChanged;
    public event ARLevelHandler LevelStarted;
    public event ARLevelHandler LevelStopped;
    public event CharacterHandler CharacterChanged;
    public event CharacterRegisterHandler CharacterSpawned;
    public event CharacterRegisterHandler CharacterDespawned;

    public event NetworkHandler ConnectedToMaster;
    public event NetworkRoomHandler CreatedRoom;
    public event NetworkRoomHandler JoinedRoom;
    public event NetworkRoomHandler LeftRoom;
    public event NetworkDisconnectHandler Disconnected;
    public event NetworkPlayerHandler PlayerJoinedRoom;
    public event NetworkPlayerHandler PlayerLeftRoom;
    public event NetworkPlayerHandler MasterClientSwitched;
    public event NetworkPlayerPropertiesHandler PlayerCustomPropertiesChanged;
    public event NetworkRoomPropertiesHandler RoomCustomPropertiesChanged;
    public event NetworkErrorHandler RoomJoinFailed;
    public event NetworkErrorHandler RoomCreateFailed;

    public event PointHandler PointsChanged;

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

    public void OnARLevelStopped(ARLevel _stoppedLevel)
    {
        LevelStopped?.Invoke(_stoppedLevel);
    }

    public void OnCharacterChanged(Character _char)
    {
        CharacterChanged?.Invoke(_char);
    }

    public void OnCharacterSpawned(GameObject _char)
    {
        CharacterSpawned?.Invoke(_char);
    }

    public void OnCharacterDespawned(GameObject _char)
    {
        CharacterDespawned?.Invoke(_char);
    }

    public void OnConnectedToMasterServer()
    {
        ConnectedToMaster?.Invoke();
    }

    public void OnCreatedRoom(Room _room)
    {
        CreatedRoom?.Invoke(_room);
    }

    public void OnClientJoinedRoom(Room _room)
    {
        JoinedRoom?.Invoke(_room);
    }

    public void OnClientLeftRoom(Room _room)
    {
        LeftRoom?.Invoke(_room);
    }

    public void OnClientDisconnected(DisconnectCause _cause)
    {
        Disconnected?.Invoke(_cause);
    }

    public void OnPlayerJoinedRoom(Player _player)
    {
        PlayerJoinedRoom?.Invoke(_player);
    }

    public void OnPlayerLeftRoom(Player _player)
    {
        PlayerLeftRoom?.Invoke(_player);
    }

    public void OnMasterClientSwitched(Player _newMasterClient)
    {
        MasterClientSwitched?.Invoke(_newMasterClient);
    }

    public void OnPlayerCustomPropertiesChanged(Player _target, ExitGames.Client.Photon.Hashtable _changedProps)
    {
        PlayerCustomPropertiesChanged?.Invoke(_target, _changedProps);
    }

    public void OnRoomCustomPropertiesChanged(ExitGames.Client.Photon.Hashtable _changedProps)
    {
        RoomCustomPropertiesChanged?.Invoke(_changedProps);
    }

    public void OnRoomJoinFailed(short _errorCode, string _msg)
    {
        RoomJoinFailed?.Invoke(_errorCode, _msg);
    }

    public void OnRoomCreateFailed(short _errorCode, string _msg)
    {
        RoomCreateFailed?.Invoke(_errorCode, _msg);
    }

    public void OnPointsChanged(int _points)
    {
        PointsChanged?.Invoke(_points);
    }
}
