using System;
using UnityEngine;

public class InputController : MonoBehaviour
{
    #region Events

    public event Func<PlayerStates, PlayerStates> TryBindingToObject;
    public event Action TryLaunchUnguidedMissile;
    public event Action CancelBuildSelection;
    public event Action TryLaunchGuidedMissile;
    public event Action StartSelectionFarTarget;
    public event Action StartSelectionAnyTarget;
    public event Action CancelSelectionAnytarget;
    public event Action CancelAiming;

    #endregion

    #region Properties

    public static InputController singleton { get; private set; }
    public bool MinigunFire { get; private set; }
    public bool FastMoving { get; private set; }
    public bool VertFastMoving { get; private set; }
    public bool UnguidedMissileLaunch
    {
        get
        {
            if (unguidedMissileLaunch) { unguidedMissileLaunch = false; return true; }
            else return false;
        }
        private set { }
    }
    public bool GuidedMissileLaunch
    {
        get
        {
            if (guidedMissileLaunch) { guidedMissileLaunch = false; return true; }
            else return false;
        }
        private set { }
    }
    public bool PlayerCanTranslate => playerState == PlayerStates.Normal || playerState == PlayerStates.Aiming;
    public bool AimMovement => playerState == PlayerStates.SelectionAnyTarget || playerState == PlayerStates.SelectionFarTarget;
    public float VertDirection { get; private set; }
    public PlayerStates PlayerState { get => playerState; private set { } }

    #endregion

    bool unguidedMissileLaunch, guidedMissileLaunch;
    PlayerStates playerState;
    PlayerInput playerInput;

    public Vector2 GetInput(bool useNewInputSystem = true)
    {
        Vector2 input;
        if (useNewInputSystem)
            input = playerInput.Player.Move.ReadValue<Vector2>();
        else
        {
            float inputX = Input.GetAxis("Horizontal");
            float inputZ = Input.GetAxis("Vertical");
            input = new Vector2(inputX, inputZ);
        }
        return input;
    }

    public Vector2 GetVerticalInput(bool useNewInputSystem = true)
    {
        Vector2 input = new Vector2(0f, 0f);
        if (useNewInputSystem)
            input = playerInput.Player.VerticalMove.ReadValue<Vector2>();
        return input;
    }

    public Vector2 GetCameraInput(bool useNewInputSystem = true)
    {
        Vector2 input = new Vector2(0f, 0f);
        if (useNewInputSystem)
            input = playerInput.Camera.Move.ReadValue<Vector2>();
        return input;
    }

    public void ForceChangePlayerState(PlayerStates newState) => PlayerState = newState;
    public void ForceStopVertFastMoving() => VertFastMoving = false;

    private InputController() { }

    void Awake()
    {
        singleton = this;

        playerInput = new PlayerInput();

        playerInput.Common.MainAction.performed += context => DoMainAction();
        playerInput.Common.MainAction.canceled += context => DoMainActionCancel();

        playerInput.Common.MinorAction.performed += context => DoMinorAction();
        playerInput.Common.MinorActionHold.performed += context => DoMinorActionHold();

        playerInput.Common.AnyTargetSelection.performed += context => AnyTargetSelection();
        playerInput.Common.AnyTargetSelection.canceled += context => AnyTargetSelectionCancel();

        playerInput.Player.FastMove.performed += context => FastMove();
        playerInput.Player.FastMove.canceled += context => FastMoveCancel();

        playerInput.Player.VerticalFastUp.performed += context => VerticalFastMove(1f);
        playerInput.Player.VerticalFastDown.performed += context => VerticalFastMove(-1f);
    }

    void OnEnable() => playerInput.Enable();

    void OnDisable() => playerInput.Disable();

    void DoMainAction()
    {
        if (playerState == PlayerStates.Normal || playerState == PlayerStates.Aiming)
            MinigunFire = true;
    }

    void DoMainActionCancel() => MinigunFire = false;

    void DoMinorAction()
    {
        switch(playerState)
        {
            case PlayerStates.Normal:
                playerState = (TryBindingToObject?.Invoke(playerState)).Value;
                break;
            case PlayerStates.Aiming:
                TryLaunchUnguidedMissile?.Invoke();
                break;
            case PlayerStates.BuildSelection:
                CancelBuildSelection?.Invoke();
                playerState = PlayerStates.Normal;
                break;
            case PlayerStates.SelectionFarTarget:
                TryLaunchGuidedMissile?.Invoke();
                playerState = PlayerStates.Normal;
                break;
        }
    }

    void DoMinorActionHold()
    {
        switch(playerState)
        {
            case PlayerStates.Normal:
                StartSelectionFarTarget?.Invoke();
                playerState = PlayerStates.SelectionFarTarget;
                break;
        }
    }

    void AnyTargetSelection()
    {
        switch(playerState)
        {
            case PlayerStates.Normal:
                StartSelectionAnyTarget?.Invoke();
                playerState = PlayerStates.SelectionAnyTarget;
                break;
        }
    }

    void AnyTargetSelectionCancel()
    {
        switch(playerState)
        {
            case PlayerStates.SelectionAnyTarget:
                CancelSelectionAnytarget?.Invoke();
                playerState = PlayerStates.Normal;
                break;
        }
    }

    void FastMove()
    {
        switch(playerState)
        {
            case PlayerStates.Normal:
                FastMoving = true;
                break;
            case PlayerStates.Aiming:
                CancelAiming?.Invoke();
                playerState = PlayerStates.Normal;
                break;
        }
    }

    void FastMoveCancel() => FastMoving = false;

    void VerticalFastMove(float dir)
    {
        switch(playerState)
        {
            case PlayerStates.Normal:
                VertFastMoving = true;
                VertDirection = dir;
                break;
        }
    }

    public enum PlayerStates
    {
        Normal,
        Aiming,
        SelectionFarTarget,
        SelectionAnyTarget,
        BuildSelection
    }
}
