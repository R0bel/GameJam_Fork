using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum UIView
{
    CREATE_ROOM,
    INSIDE_ROOM,
    IN_GAME
}

public class ARScene : SceneMonoBehaviour
{
    private GameManager gameManager;
    private Character activeChar;
    private ARLevel currentLevel;

    [Header("Network Controls")]
    [SerializeField]
    private GameObject networkViewObj;
    [SerializeField]
    private GameObject networkCreateRoomViewObj;

    [Space(10f)]
    [SerializeField]
    private InputField playerNameInput;
    [SerializeField]
    private Button joinCreateRoomBtn;
    [SerializeField]
    private Text statusText;

    [Space(5f)]
    [SerializeField]
    private GameObject networkRoomViewObj;
    [SerializeField]
    private Text roomNameText;
    [SerializeField]
    private Text playerListText;

    [Space(5f)]
    [SerializeField]
    private GameObject inGameView;

    [Header("Player Controls")]
    [Space(10f)]
    [SerializeField]
    private Joystick joystick;
    [SerializeField]
    private float deadPoint;
    private bool inCrouch = false;

    public override void OnStart(GameManager _manager)
    {
        Debug.Log($"{this.GetType().Name} initialized");
        gameManager = _manager;
        activeChar = gameManager.Char.ActiveCharacter;

        joinCreateRoomBtn.interactable = false;

        gameManager.Events.CharacterChanged += OnCharacterUpdate;
        gameManager.Events.LevelStarted += OnLevelStarted;
        gameManager.Events.ConnectedToMaster += OnConnectedToMasterServer;
        gameManager.Events.JoinedRoom += OnJoinedRoom;
    }

    private void OnLevelStarted(ARLevel _level)
    {
        currentLevel = _level;
        if (gameManager.Network.IsConnectedAndReady && gameManager.Network.InRoom)
        {
            ActivateUIView(UIView.INSIDE_ROOM);
        } else
        {
            ActivateUIView(UIView.CREATE_ROOM);
        }        
    }

    private void OnConnectedToMasterServer()
    {
        statusText.text = "Connected";
        joinCreateRoomBtn.interactable = true;
    }

    private void OnJoinedRoom(Room _room)
    {
        statusText.text = "Joined room: " + _room.Name;
        // ActivateUIView(UIView.INSIDE_ROOM);
        roomNameText.text = _room.Name;

        foreach(Player player in gameManager.Network.RoomPlayers)
        {
            playerListText.text = "- " + player.NickName + "\n";
        }

        gameManager.Events.RoomCustomPropertiesChanged += OnRoomPropertiesChanged;


        ActivateUIView(UIView.IN_GAME);
        if (gameManager.Network.IsMasterClient)
        {
            if (currentLevel != null) currentLevel.SpawnCharacter("CowPlayer");
        }
        else
        {
            if (currentLevel != null) currentLevel.SpawnCharacter("PigPlayer");
        }

    }

    private void OnRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable _changedProps)
    {
        
    }

    #region ButtonCallbacks
    public void OnJoinCreateBtn()
    {
        if (playerNameInput.text != string.Empty && gameManager.Network.IsConnectedAndReady)
        {
            // set players nickname
            gameManager.Network.Nickname = playerNameInput.text;

            // start new room or join one
            string roomName = "defaultLevelRoom";
            if (currentLevel != null) roomName = currentLevel.ImageName;
            gameManager.Network.JoinOrCreateRoom(roomName);
        }
    }
    #endregion

    public void StartGame()
    {
        if (gameManager.Network.InRoom
            && gameManager.Network.IsMasterClient
            // && gameManager.Network.RoomPlayers.Length == PhotonNetwork.CurrentRoom.MaxPlayers
            )
        {
            // Set room gameStarted property
            PhotonNetwork.CurrentRoom.CustomProperties["GameStarted"] = true;
            ActivateUIView(UIView.IN_GAME);

            if (gameManager.Network.IsMasterClient)
            {
                if (currentLevel != null) currentLevel.SpawnCharacter("CowPlayer");
            } else
            {
                if (currentLevel != null) currentLevel.SpawnCharacter("PigPlayer");
            }            
        }
    }

    /// <summary>
    /// Activate UI View
    /// </summary>
    /// <param name="_type"></param>
    public void ActivateUIView(UIView _type)
    {
        switch(_type)
        {
            case UIView.CREATE_ROOM:
                networkViewObj.SetActive(true);
                networkCreateRoomViewObj.SetActive(true);
                networkRoomViewObj.SetActive(false);
                inGameView.SetActive(false);
                break;
            case UIView.INSIDE_ROOM:
                networkViewObj.SetActive(true);
                networkCreateRoomViewObj.SetActive(false);
                networkRoomViewObj.SetActive(true);
                inGameView.SetActive(false);
                break;
            case UIView.IN_GAME:
                networkViewObj.SetActive(false);
                networkCreateRoomViewObj.SetActive(false);
                networkRoomViewObj.SetActive(false);
                inGameView.SetActive(true);
                break;
        }
    }

    public void OnRunBtnClicked()
    {
        if (activeChar != null) activeChar.IsRunning = !activeChar.IsRunning;
    }

    public void OnCrouchTriggered()
    {
        inCrouch = !inCrouch;
    }

    private void OnCharacterUpdate(Character _char)
    {
        Debug.Log("Character updated!");
        if (gameManager != null) activeChar = _char;
    }

    private void Update()
    {
        if (activeChar != null && activeChar.isActiveAndEnabled)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift)) activeChar.IsRunning = !activeChar.IsRunning;

            activeChar.CrouchInput = inCrouch ? -1f : 0f;
            bool horizontalActive = (joystick.Horizontal > deadPoint || joystick.Horizontal < -deadPoint);
            bool verticalActive = (joystick.Vertical > deadPoint || joystick.Vertical < -deadPoint);

            bool outOfDeadPoint = horizontalActive || verticalActive;
            float horizontalInput = outOfDeadPoint ? joystick.Horizontal : 0.0f;
            float verticalInput = outOfDeadPoint ? joystick.Vertical : 0.0f;
            
            bool inputActive = outOfDeadPoint;

            activeChar.SetMoveInput(verticalInput, horizontalInput, inputActive);
        }
    }
}
