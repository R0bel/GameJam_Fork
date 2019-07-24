using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks, IManagedBehaviour
{
    private GameManager gameManager;
    private EventManager events;

    [SerializeField]
    private string gameVersion;
    [SerializeField]
    private byte maxPlayersPerRoom;
    private RoomOptions roomOptions;

    private Player[] roomPlayers;
    private ExitGames.Client.Photon.Hashtable initialRoomProperties;

    public void OnStart(GameManager _manager)
    {
        gameManager = _manager;
        events = gameManager.Events;
    }    

    private void Awake()
    {
        // setup photon settings
        roomOptions = new RoomOptions
        {
            MaxPlayers = maxPlayersPerRoom,
            IsVisible = false
        };

        initialRoomProperties = new ExitGames.Client.Photon.Hashtable();
        initialRoomProperties.Add("GameStarted", false);

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #region Properties

    public bool IsConnectedAndReady
    {
        get { return PhotonNetwork.IsConnectedAndReady; }
    }

    public bool IsConnected
    {
        get { return PhotonNetwork.IsConnected; }
    }

    public bool InRoom
    {
        get { return PhotonNetwork.InRoom; }
    }

    public bool IsMasterClient
    {
        get { return PhotonNetwork.IsMasterClient; }
    }

    public string Nickname
    {
        get
        {
            return PhotonNetwork.NickName;
        }
        set
        {
            PhotonNetwork.NickName = value;
        }
    }

    public Player[] RoomPlayers
    {
        get
        {
            return PhotonNetwork.PlayerList;
        }
    }
    #endregion Properties

    #region PhotonMethods
    public void ConnectToMasterserver()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void CreateRoom(string _roomName)
    {
        if (!InRoom)
        {
            PhotonNetwork.CreateRoom(_roomName, roomOptions);
        }
    }

    public void JoinRoom(string _roomName)
    {
        if (!InRoom)
        {
            PhotonNetwork.JoinRoom(_roomName);
        }
    }

    public void JoinOrCreateRoom(string _roomName)
    {
        if (!InRoom)
        {
            PhotonNetwork.JoinOrCreateRoom(_roomName, roomOptions, TypedLobby.Default);
        }
    }    

    public void LeaveRoom()
    {
        if (InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }
    #endregion

    #region PhotonCallbacks
    public override void OnConnectedToMaster()
    {
        Debug.Log($"Connected to Masterserver in Region: {PhotonNetwork.CloudRegion}");
        events.OnConnectedToMasterServer();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined to Room!: {PhotonNetwork.CurrentRoom.Name}");
        events.OnClientJoinedRoom(PhotonNetwork.CurrentRoom);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left Room!");
        events.OnClientLeftRoom(PhotonNetwork.CurrentRoom);
    }

    public override void OnDisconnected(DisconnectCause _cause)
    {
        Debug.LogWarningFormat($"Disconnected: {_cause}");
        events.OnClientDisconnected(_cause);
    }

    public override void OnPlayerEnteredRoom(Player _newPlayer)
    {
        Debug.LogWarningFormat($"{_newPlayer.NickName} joined Room");
        events.OnPlayerJoinedRoom(_newPlayer);
    }

    public override void OnPlayerLeftRoom(Player _otherPlayer)
    {
        Debug.LogWarningFormat($"{_otherPlayer.NickName} left Room");
        events.OnPlayerLeftRoom(_otherPlayer);
    }

    public override void OnCreatedRoom()
    {
        events.OnCreatedRoom(PhotonNetwork.CurrentRoom);

        // set inital room properties
        PhotonNetwork.CurrentRoom.SetCustomProperties(initialRoomProperties);
    }

    public override void OnMasterClientSwitched(Player _newMasterClient)
    {
        Debug.Log($"New Masterclient is {(_newMasterClient.IsLocal ? "local" : "remote")}");
        events.OnMasterClientSwitched(_newMasterClient);
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable _propertiesThatChanged)
    {
        events.OnRoomCustomPropertiesChanged(_propertiesThatChanged);
    }

    public override void OnPlayerPropertiesUpdate(Player _target, ExitGames.Client.Photon.Hashtable _changedProps)
    {
        events.OnPlayerCustomPropertiesChanged(_target, _changedProps);
    }

    // Error catching
    public override void OnJoinRoomFailed(short _returnCode, string _message)
    {
        events.OnRoomJoinFailed(_returnCode, _message);
    }

    public override void OnCreateRoomFailed(short _returnCode, string _message)
    {
        events.OnRoomCreateFailed(_returnCode, _message);
    }

    #endregion

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
    }

}
