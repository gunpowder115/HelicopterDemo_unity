using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] float changeSpeedInput = 0.7f;
    [SerializeField] bool useNewInputSystem = true;
    [SerializeField] float vertFastCoef = 5f;

    private TranslationInput translationInput;
    private RotationInput rotationInput;
    private CameraRotation cameraRotation;
    private bool rotateToDirection;
    private Vector3 targetDirection;
    private float angularDistance;
    private Vector3 currentDirection;
    private bool cameraInAim, aiming;
    private PlayerInput playerInput;
    private Vector3 aimAngles;
    private VertMoveStates vertState;
    private float vertDirection;

    private void Awake()
    {
        playerInput = new PlayerInput();

        playerInput.Common.MainAction.performed += context => DoMainAction();
        playerInput.Common.MinorAction.performed += context => DoMinorAction();
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
        rotateToDirection = false;
        targetDirection = transform.forward;
        currentDirection = transform.forward;
        angularDistance = 0.0f;
        cameraInAim = aiming = false;
        vertState = VertMoveStates.Normal;

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

        //movement around X, Y, Z
        if (translationInput != null)
        {
            if (inputXZ >= changeSpeedInput && !translationInput.CurrSpeedIsHigh ||
                inputXZ < changeSpeedInput && translationInput.CurrSpeedIsHigh)
                rotateToDirection = translationInput.ChangeSpeed();

            targetDirection = translationInput.TargetDirection;
            angularDistance = translationInput.GetAngularDistance(currentDirection, targetDirection);
            if (translationInput.IsHeightBorder)
                vertState = VertMoveStates.Normal;

            translationInput.Translate(Axis_Proto.X, inputX);
            translationInput.Translate(Axis_Proto.Y, vertState == VertMoveStates.Fast ? vertFastCoef * vertDirection : inputY);
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
        if (cameraRotation != null)
        {
            cameraRotation.UseNewInputSystem = useNewInputSystem;

            if (aiming)
                aiming = cameraRotation.ChangeCameraState(cameraInAim, aimAngles);
            else
            {
                if (!cameraInAim)
                {
                    cameraRotation.RotateHorizontally(rotateToDirection ? currentDirection.x : targetDirection.x);
                    cameraRotation.RotateVertically(0f);
                }
                else
                    cameraRotation.RotateWithPlayer(aimAngles);
            }
        }
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
        vertState = VertMoveStates.Fast;
        vertDirection = dir;
    }

    private void DoMainAction()
    {
        Debug.Log("MainAction!");
    }

    private void DoMinorAction()
    {
        cameraInAim = !cameraInAim;
        aiming = true;
    }

    public enum Axis_Proto : int
    { X, Y, Z }

    public enum VertMoveStates
    { Normal, Fast }
}
