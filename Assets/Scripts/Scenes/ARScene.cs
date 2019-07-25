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

    [Header("Audio")]
    [SerializeField]
    private AudioClip gameStartSound;
    [SerializeField]
    private AudioClip infectionSound;
    [SerializeField]
    private AudioClip uiStartSound;
    [SerializeField]
    private AudioClip uiSelectSound;

    [Header("Network Controls")]
    [SerializeField]
    private GameObject networkViewObj;
    [SerializeField]
    private GameObject networkCreateRoomViewObj;

    [Space(10f)]
    [SerializeField]
    private Button joinCreateRoomBtn;
    [SerializeField]
    private Button audioToggleBtn;
    [SerializeField]
    private Text statusText;

    [Space(5f)]
    [SerializeField]
    private GameObject inGameView;

    [Header("Player Controls")]
    [Space(10f)]
    [SerializeField]
    private Text pointsText;
    [SerializeField]
    private Image runBtnImg;
    [SerializeField]
    private Sprite runSprite;
    [SerializeField]
    private Sprite runSpriteActive;
    [Space(5f)]
    [SerializeField]
    private Joystick joystick;
    [SerializeField]
    private float deadPoint;
    private bool inCrouch = false;
    private int points = 0; // currently hold in UI TODO: Holded in GameManager or Player (Photon Syn!?)

    public override void OnStart(GameManager _manager)
    {
        Debug.Log($"{this.GetType().Name} initialized");
        gameManager = _manager;
        activeChar = gameManager.Char.ActiveCharacter;
        points = 0;

        joinCreateRoomBtn.interactable = false;
        statusText.color = Color.red;

        gameManager.Events.CharacterChanged += OnCharacterUpdate;
        gameManager.Events.LevelStarted += OnLevelStarted;
        gameManager.Events.ConnectedToMaster += OnConnectedToMasterServer;
        gameManager.Events.JoinedRoom += OnJoinedRoom;
        gameManager.Events.LeftRoom += OnLeftRoom;
        gameManager.Events.PointsChanged += OnPointsChanged;
    }

    private void OnLevelStarted(ARLevel _level)
    {
        currentLevel = _level;
        pointsText.text = points.ToString();
        ActivateUIView(UIView.CREATE_ROOM);
        // gameManager.Audio.PlaySingle(uiStartSound);
    }

    private void OnConnectedToMasterServer()
    {
        statusText.text = "CONNECTED";
        statusText.color = Color.green;
        joinCreateRoomBtn.interactable = true;
    }

    private void OnJoinedRoom(Room _room)
    {
        // ActivateUIView(UIView.INSIDE_ROOM);
        gameManager.Events.RoomCustomPropertiesChanged += OnRoomPropertiesChanged;

        // play game start audio
        gameManager.Audio.PlaySingle(gameStartSound);
        
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

    private void OnLeftRoom(Room _room)
    {
        ActivateUIView(UIView.CREATE_ROOM);
    }

    private void OnRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable _changedProps)
    {
        
    }

    private void OnPointsChanged(int _points)
    {
        gameManager.Audio.PlaySingle(infectionSound);
        points += _points;
        pointsText.text = points.ToString();
    }

    #region ButtonCallbacks
    public void OnJoinCreateBtn()
    {
        // play game start audio
        gameManager.Audio.PlaySingle(uiSelectSound);
        if (gameManager.Network.IsConnectedAndReady)
        {
            // set players nickname
            // gameManager.Network.Nickname = playerNameInput.text;

            // start new room or join one
            string roomName = "defaultLevelRoom";
            if (currentLevel != null) roomName = currentLevel.ImageName;
            gameManager.Network.JoinOrCreateRoom(roomName);
        }
    }

    public void OnToggleAudioBtn()
    {
        // play game start audio
        gameManager.Audio.PlaySingle(uiSelectSound);
    }
    #endregion

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
                inGameView.SetActive(false);
                Screen.orientation = ScreenOrientation.Portrait;
                break;
            case UIView.IN_GAME:
                networkViewObj.SetActive(false);
                networkCreateRoomViewObj.SetActive(false);
                inGameView.SetActive(true);
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                break;
        }
    }

    public void OnRunBtnClicked()
    {
        if (activeChar != null)
        {
            gameManager.Audio.PlaySingle(uiSelectSound);
            activeChar.IsRunning = !activeChar.IsRunning;
            if (activeChar.IsRunning) runBtnImg.sprite = runSpriteActive;
            else runBtnImg.sprite = runSprite;
        }
    }

    public void OnCrouchTriggered()
    {
        inCrouch = !inCrouch;
    }

    private void OnCharacterUpdate(Character _char)
    {
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
