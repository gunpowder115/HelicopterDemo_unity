using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] float changeSpeedInput = 0.7f;
    [SerializeField] bool useNewInputSystem = true;

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

    private void Awake()
    {
        playerInput = new PlayerInput();

        playerInput.Player.Shoot.performed += context => Shoot();
        playerInput.Common.MinorAction.performed += context => DoMinorAction();
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

        //hide cursor in center of screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputDirection = GetInput();
        float inputX = inputDirection.x;
        float inputY = Input.GetAxis("Jump");
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

            translationInput.Translate(Axis_Proto.X, inputX);
            translationInput.Translate(Axis_Proto.Y, inputY);
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
            Debug.Log(input.x);
        }
        else
        {
            float inputX = Input.GetAxis("Horizontal");
            float inputZ = Input.GetAxis("Vertical");
            input = new Vector2(inputX, inputZ);
        }
        return input;
    }

    private void Shoot()
    {
        Debug.Log("Shot!");
    }

    private void DoMinorAction()
    {
        cameraInAim = !cameraInAim;
        aiming = true;
    }

    public enum Axis_Proto : int
    { X, Y, Z }
}
