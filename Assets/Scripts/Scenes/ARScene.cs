using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ARScene : SceneMonoBehaviour
{
    private GameManager gameManager;
    private Character activeChar;

    [Header("Character UI Control")]
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

        gameManager.Events.CharacterChanged += OnCharacterUpdate;
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
