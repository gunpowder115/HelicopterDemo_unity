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
    public float VerticalMoving
    {
        get
        {
            bool up = ((LeftUp || RightUp) && LeftUp != RightUp) || SingleUp;
            bool down = ((LeftDown || RightDown) && LeftDown != RightDown) || SingleDown;

            if (up) return 1f;
            if (down) return -1f;
            return 0f;
        }
    }
    public float VerticalFastMoving
    {
        get
        {
            if (fastUp) return 1f;
            if (fastDown) return -1f;
            return 0f;
        }
    }
    public PlayerStates PlayerState => playerState;

    #region Private

    bool LeftUp
    {
        get => verticalInputButtons[leftUpIndex];
        set => verticalInputButtons[leftUpIndex] = value;
    }
    bool RightUp
    {
        get => verticalInputButtons[rightUpIndex];
        set => verticalInputButtons[rightUpIndex] = value;
    }
    bool SingleUp
    {
        get => verticalInputButtons[singleUpIndex];
        set => verticalInputButtons[singleUpIndex] = value;
    }
    bool LeftDown
    {
        get => verticalInputButtons[leftDownIndex];
        set => verticalInputButtons[leftDownIndex] = value;
    }
    bool RightDown
    {
        get => verticalInputButtons[rightDownIndex];
        set => verticalInputButtons[rightDownIndex] = value;
    }
    bool SingleDown
    {
        get => verticalInputButtons[singleDownIndex];
        set => verticalInputButtons[singleDownIndex] = value;
    }

    #endregion

    #endregion

    readonly int verticalInputButtonsCount = 6;
    readonly int leftUpIndex = (int)VerticalMoveDirection.LeftUp;
    readonly int rightUpIndex = (int)VerticalMoveDirection.RightUp;
    readonly int singleUpIndex = (int)VerticalMoveDirection.SingleUp;
    readonly int leftDownIndex = (int)VerticalMoveDirection.LeftDown;
    readonly int rightDownIndex = (int)VerticalMoveDirection.RightDown;
    readonly int singleDownIndex = (int)VerticalMoveDirection.SingleDown;

    bool unguidedMissileLaunch, guidedMissileLaunch;
    bool fastUp, fastDown;
    bool[] verticalInputButtons;
    PlayerStates playerState;
    PlayerInput playerInput;

    public Vector2 GetInput() => playerInput.Player.Move.ReadValue<Vector2>();

    public Vector2 GetCameraInput() => playerInput.Camera.Move.ReadValue<Vector2>();

    public void ForceChangePlayerState(PlayerStates newState) => playerState = newState;

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

        #region Vertical Move

        playerInput.Player.LeftUp.performed += context => SetVerticalMove(VerticalMoveDirection.LeftUp);
        playerInput.Player.RightUp.performed += context => SetVerticalMove(VerticalMoveDirection.RightUp);
        playerInput.Player.LeftDown.performed += context => SetVerticalMove(VerticalMoveDirection.LeftDown);
        playerInput.Player.RightDown.performed += context => SetVerticalMove(VerticalMoveDirection.RightDown);

        playerInput.Player.LeftUp.canceled += context => CancelVerticalMove(VerticalMoveDirection.LeftUp);
        playerInput.Player.RightUp.canceled += context => CancelVerticalMove(VerticalMoveDirection.RightUp);
        playerInput.Player.LeftDown.canceled += context => CancelVerticalMove(VerticalMoveDirection.LeftDown);
        playerInput.Player.RightDown.canceled += context => CancelVerticalMove(VerticalMoveDirection.RightDown);

        playerInput.Player.SingleUp.performed += context => SetVerticalMove(VerticalMoveDirection.SingleUp);
        playerInput.Player.SingleDown.performed += context => SetVerticalMove(VerticalMoveDirection.SingleDown);
        playerInput.Player.VertFastMod.performed += context => SetVerticalSingleFast();

        playerInput.Player.SingleUp.canceled += context => CancelVerticalMove(VerticalMoveDirection.SingleUp);
        playerInput.Player.SingleDown.canceled += context => CancelVerticalMove(VerticalMoveDirection.SingleDown);
        playerInput.Player.VertFastMod.canceled += context => CancelVerticalSingleFast();

        #endregion

        verticalInputButtons = new bool[verticalInputButtonsCount];
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
        switch (playerState)
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
        switch (playerState)
        {
            case PlayerStates.Normal:
                StartSelectionFarTarget?.Invoke();
                playerState = PlayerStates.SelectionFarTarget;
                break;
        }
    }

    void AnyTargetSelection()
    {
        switch (playerState)
        {
            case PlayerStates.Normal:
                StartSelectionAnyTarget?.Invoke();
                playerState = PlayerStates.SelectionAnyTarget;
                break;
        }
    }

    void AnyTargetSelectionCancel()
    {
        switch (playerState)
        {
            case PlayerStates.SelectionAnyTarget:
                CancelSelectionAnytarget?.Invoke();
                playerState = PlayerStates.Normal;
                break;
        }
    }

    void FastMove()
    {
        switch (playerState)
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

    void SetVerticalMove(VerticalMoveDirection vertMoveDir)
    {
        switch (playerState)
        {
            case PlayerStates.Normal:
                verticalInputButtons[(int)vertMoveDir] = true;
                if (LeftUp && RightUp)
                    fastUp = true;
                else if (LeftDown && RightDown)
                    fastDown = true;
                break;
            case PlayerStates.Aiming:
                verticalInputButtons[(int)vertMoveDir] = true;
                break;
        }
        //no movement if Up & Down pressed in one time
        if (((LeftUp || RightUp) && (LeftDown || RightDown)) || (SingleUp && SingleDown))
        {
            LeftUp = RightUp = LeftDown = RightDown = SingleUp = SingleDown = false;
            fastUp = fastDown = false;
        }
    }

    void CancelVerticalMove(VerticalMoveDirection vertMoveDir)
    {
        fastUp = fastDown = false;
        verticalInputButtons[(int)vertMoveDir] = false;
    }

    void SetVerticalSingleFast()
    {
        if (SingleUp && !SingleDown) fastUp = true;
        if (SingleDown && !SingleUp) fastDown = true;
    }

    void CancelVerticalSingleFast() => fastUp = fastDown = false;

    public enum PlayerStates
    {
        Normal,
        Aiming,
        SelectionFarTarget,
        SelectionAnyTarget,
        BuildSelection
    }

    enum VerticalMoveDirection : int
    {
        LeftUp,
        RightUp,
        SingleUp,
        LeftDown,
        RightDown,
        SingleDown
    }
}
