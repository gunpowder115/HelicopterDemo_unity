using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] float changeSpeedInput = 0.7f;
    [SerializeField] bool useNewInputSystem = true;
    [SerializeField] float vertFastCoef = 5f;

    private TranslationInput translationInput;
    private RotationInput rotationInput;
    private CameraRotation cameraRotation;
    private TargetSelectionInput targetSelectionInput;
    private bool rotateToDirection;
    private Vector3 targetDirection;
    private float angularDistance;
    private Vector3 currentDirection;
    private bool cameraInAim, aiming;
    private PlayerInput playerInput;
    private Vector3 aimAngles;
    private float vertDirection;
    private PlayerStates playerState;

    private void Awake()
    {
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

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        translationInput = GetComponentInChildren<TranslationInput>();
        rotationInput = GetComponentInChildren<RotationInput>();
        cameraRotation = GetComponentInChildren<CameraRotation>();
        targetSelectionInput = GetComponentInChildren<TargetSelectionInput>();

        rotateToDirection = false;
        targetDirection = transform.forward;
        currentDirection = transform.forward;
        angularDistance = 0.0f;
        cameraInAim = aiming = false;
        playerState = PlayerStates.Normal;

        //hide cursor in center of screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputDirection = GetInput();
        Vector2 inputVerticalDirection = GetVerticalInput();
        float inputX = inputDirection.x;
        float inputY = inputVerticalDirection.y;
        float inputZ = inputDirection.y;
        float inputXZ = Mathf.Clamp01(new Vector3(inputX, 0f, inputZ).magnitude);

        //move aim if it target selection mode
        bool aimMovement = playerState == PlayerStates.SelectionFarTarget || playerState == PlayerStates.SelectionAnyTarget;
        Vector2 toTargetSelection = new Vector2();
        if (targetSelectionInput && aimMovement)
        {
            targetSelectionInput.MoveAim(new Vector2(inputX, inputZ));
            toTargetSelection = targetSelectionInput.ToTargetSelection;
        }

        //movement around X, Y, Z
        bool playerCanTranslate = playerState != PlayerStates.SelectionFarTarget && playerState != PlayerStates.SelectionAnyTarget;
        if (translationInput != null)
        {
            if (inputXZ >= changeSpeedInput && !translationInput.CurrSpeedIsHigh ||
                inputXZ < changeSpeedInput && translationInput.CurrSpeedIsHigh)
                rotateToDirection = translationInput.ChangeSpeed();

            targetDirection = translationInput.TargetDirection;
            angularDistance = translationInput.GetAngularDistance(currentDirection, targetDirection);
            if (translationInput.IsHeightBorder && playerState == PlayerStates.VerticalFastMoving)
                playerState = PlayerStates.Normal;

            if (!playerCanTranslate)
                inputX = inputY = inputZ = 0f;
            translationInput.Translate(Axis_Proto.X, inputX);
            translationInput.Translate(Axis_Proto.Y, playerState == PlayerStates.VerticalFastMoving ? vertFastCoef * vertDirection : inputY);
            translationInput.Translate(Axis_Proto.Z, inputZ);
        }

        //rotation around X, Y, Z
        if (rotationInput != null)
        {
            currentDirection = rotationInput.CurrentDirection;
            aimAngles = rotationInput.AimAngles;

            rotationInput.RotateByYaw(angularDistance, rotateToDirection);
            rotationInput.RotateByAttitude(targetDirection, inputXZ, rotateToDirection);
        }

        //camera rotation
        if (cameraRotation)
        {
            cameraRotation.UseNewInputSystem = useNewInputSystem;
            bool rotateWithTargetSelection = playerState == PlayerStates.SelectionFarTarget || playerState == PlayerStates.SelectionAnyTarget;

            if (aiming)
                aiming = cameraRotation.ChangeCameraState(cameraInAim, aimAngles);
            else
            {
                if (!cameraInAim)
                {
                    cameraRotation.RotateHorizontally(rotateWithTargetSelection ? toTargetSelection.x : currentDirection.x);
                    cameraRotation.RotateVertically(rotateWithTargetSelection ? toTargetSelection.y : 0f);
                }
                else
                    cameraRotation.RotateWithPlayer(aimAngles);
            }
        }

        Debug.Log(playerState);
    }

    private Vector2 GetInput()
    {
        Vector2 input;
        if (useNewInputSystem)
        {
            input = playerInput.Player.Move.ReadValue<Vector2>();
        }
        else
        {
            float inputX = Input.GetAxis("Horizontal");
            float inputZ = Input.GetAxis("Vertical");
            input = new Vector2(inputX, inputZ);
        }
        return input;
    }

    private Vector2 GetVerticalInput()
    {
        Vector2 input = new Vector2(0f, 0f);
        if (useNewInputSystem)
            input = playerInput.Player.VerticalMove.ReadValue<Vector2>();
        return input;
    }

    private void VerticalFastMove(float dir)
    {
        if (playerState == PlayerStates.Normal)
        {
            playerState = PlayerStates.VerticalFastMoving;
            vertDirection = dir;
        }
    }

    private void FastMove()
    {
        if (playerState == PlayerStates.Normal)
        {
            playerState = PlayerStates.FastMoving;
        }
    }

    private void FastMoveCancel()
    {
        if (playerState == PlayerStates.FastMoving)
        {
            playerState = PlayerStates.Normal;
        }
    }

    private void DoMainAction()
    {
        Debug.Log("Doing main action started");
    }

    private void DoMainActionCancel()
    {
        Debug.Log("Doing main action cancelled");
    }

    private void DoMinorAction()
    {
        if (playerState == PlayerStates.Normal || playerState == PlayerStates.Aiming)
        {
            cameraInAim = !cameraInAim;
            aiming = true;
            playerState = cameraInAim ? PlayerStates.Aiming : PlayerStates.Normal;
        }
        else
        {
            targetSelectionInput.HideAim();
            playerState = PlayerStates.Normal;
        }
    }

    private void DoMinorActionHold()
    {
        if (playerState == PlayerStates.Normal)
        {
            targetSelectionInput.ShowAim();
            playerState = PlayerStates.SelectionFarTarget;
        }
    }

    private void AnyTargetSelection()
    {
        if (playerState == PlayerStates.Normal)
        {
            targetSelectionInput.ShowAim();
            playerState = PlayerStates.SelectionAnyTarget;
        }
    }

    private void AnyTargetSelectionCancel()
    {
        if (playerState == PlayerStates.SelectionAnyTarget)
        {
            targetSelectionInput.HideAim();
            playerState = PlayerStates.Normal;
        }
    }

    public enum Axis_Proto : int
    { X, Y, Z }

    public enum PlayerStates
    {
        Normal,
        Aiming,
        SelectionFarTarget,
        SelectionAnyTarget,
        FastMoving,
        VerticalFastMoving
    }
}
