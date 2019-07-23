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
    private InputField roomNameInput;
    [SerializeField]
    private Button createRoomBtn;
    [SerializeField]
    private Button joinRoomBtn;
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

    [Space(10f)]
    [SerializeField]
    private Joystick joystick;
    [SerializeField]
    private GameObject jumpBtnObj;
    private Button jumpBtn;
    private Image jumpBtnImg;
    [SerializeField]
    private float jumpRechargeTime = 3f;
    private Coroutine jumpWaitTimeCoroutine;
    [SerializeField]
    private float deadPoint;
    private bool inCrouch = false;

    public override void OnStart(GameManager _manager)
    {
        Debug.Log($"{this.GetType().Name} initialized");
        gameManager = _manager;
        activeChar = gameManager.Char.ActiveCharacter;

        jumpBtn = jumpBtnObj.GetComponent<Button>();
        jumpBtnImg = jumpBtnObj.GetComponent<Image>();

        createRoomBtn.interactable = false;
        joinRoomBtn.interactable = false;

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
        statusText.text = "Connected to Photon Masterserver";
        createRoomBtn.interactable = true;
        joinRoomBtn.interactable = true;
    }

    private void OnJoinedRoom(Room _room)
    {
        statusText.text = "Joined room: " + _room.Name;
        ActivateUIView(UIView.INSIDE_ROOM);
        roomNameText.text = _room.Name;

        foreach(Player player in gameManager.Network.RoomPlayers)
        {
            playerListText.text = "- " + player.NickName + "\n";
        }
        
    }

    #region ButtonCallbacks
    public void OnCreateRoomBtn()
    {
        if (roomNameInput.text != string.Empty && playerNameInput.text != string.Empty && gameManager.Network.IsConnectedAndReady)
        {
            gameManager.Network.Nickname = playerNameInput.text;
            gameManager.Network.CreateRoom(roomNameInput.text);
        }
            
    }

    public void OnJoinRoomBtn()
    {
        if (roomNameInput.text != string.Empty && playerNameInput.text != string.Empty && gameManager.Network.IsConnectedAndReady)
        {
            gameManager.Network.Nickname = playerNameInput.text;
            gameManager.Network.JoinRoom(roomNameInput.text);
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
            if (currentLevel != null) currentLevel.SpawnCharacter();
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

    public void OnJumpTriggered()
    {        
        if (jumpWaitTimeCoroutine == null)
        {
            if (activeChar != null) activeChar.TriggerJump();
            jumpWaitTimeCoroutine = StartCoroutine(JumpWaiter(50f));
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

    private IEnumerator JumpWaiter(float _iterations)
    {
        if (jumpBtn != null) jumpBtn.interactable = false;
        if (jumpBtnImg != null) jumpBtnImg.fillAmount = 0f;
        float secondsRemain = jumpRechargeTime / _iterations;
        while (secondsRemain <= jumpRechargeTime)
        {
            if (jumpBtnImg != null) jumpBtnImg.fillAmount = secondsRemain / jumpRechargeTime;
            yield return new WaitForSeconds(jumpRechargeTime / _iterations);            
            secondsRemain += jumpRechargeTime / _iterations;
        }
        
        if (jumpBtn != null) jumpBtn.interactable = true;
        if (jumpBtnImg != null) jumpBtnImg.fillAmount = 1f;
        jumpWaitTimeCoroutine = null;
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
            if (Input.GetKeyDown(KeyCode.Space)) OnJumpTriggered();
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
