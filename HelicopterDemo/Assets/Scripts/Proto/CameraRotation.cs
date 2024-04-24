using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] float maxHorizontalAngle = 30f;
    [SerializeField] float maxVerticalAngle_cameraUp = 40f;
    [SerializeField] float maxVerticalAngle_cameraDown = 5f;
    [SerializeField] float rotSpeed = 1f;
    [SerializeField] float rotSpeedManual = 1f;
    [SerializeField] float aimingSpeed = 3f;

    public bool UseNewInputSystem { get; set; }

    private float inputHor, inputVert;
    private PlayerInput playerInput;
    private GameObject cameraContainer;

    private readonly float defaultVerticalAngle = 15f;
    private readonly Vector3 cameraAimingPosition = new Vector3(2.08f, 2.26f, -0.89f);
    private readonly Vector3 cameraAimingRotation = new Vector3(0f, 0f, 0f);
    private readonly Vector3 cameraDefaultPosition = new Vector3(0f, 11.58f, -22.22f);
    private readonly Vector3 cameraDefaultRotation = new Vector3(15f, 0f, 0f);

    private void Awake()
    {
        playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void Start()
    {
        cameraContainer = GameObject.FindGameObjectWithTag("CameraContainer");
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    public void RotateHorizontally(float playerDirX)
    {
        playerDirX += inputHor;

        float targetCameraHorRot = playerDirX * maxHorizontalAngle;

        Vector3 eulerAnglesCurrent = transform.rotation.eulerAngles;
        float currRotSpeed = inputHor != 0f ? rotSpeedManual : rotSpeed;
        Vector3 eulerAnglesTarget = new Vector3(eulerAnglesCurrent.x, targetCameraHorRot, eulerAnglesCurrent.z);

        Quaternion rotationTarget = Quaternion.Euler(eulerAnglesTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationTarget, currRotSpeed * Time.deltaTime);
    }

    public void RotateVertically(float playerDirZ)
    {
        playerDirZ -= inputVert;

        float targetCameraVertRot;
        if (playerDirZ > 0f)
            targetCameraVertRot = playerDirZ * maxVerticalAngle_cameraUp;
        else if (playerDirZ < 0f)
            targetCameraVertRot = playerDirZ * maxVerticalAngle_cameraDown;
        else
            targetCameraVertRot = defaultVerticalAngle;

        Vector3 eulerAnglesCurrent = transform.rotation.eulerAngles;
        float currRotSpeed = rotSpeedManual;
        Vector3 eulerAnglesTarget = new Vector3(targetCameraVertRot, eulerAnglesCurrent.y, eulerAnglesCurrent.z);

        Quaternion rotationTarget = Quaternion.Euler(eulerAnglesTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationTarget, currRotSpeed * Time.deltaTime);
    }

    public bool ChangeCameraState(bool needToAim)
    {
        Quaternion rotationTarget;
        Vector3 positionTarget;
        if (needToAim)
        {
            rotationTarget = Quaternion.Euler(cameraAimingRotation);
            positionTarget = cameraAimingPosition;
        }
        else
        {
            rotationTarget = Quaternion.Euler(cameraDefaultRotation);
            positionTarget = cameraDefaultPosition;
            cameraContainer.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, rotationTarget, aimingSpeed * Time.deltaTime);
        transform.localPosition = Vector3.Lerp(transform.localPosition, positionTarget, aimingSpeed * Time.deltaTime);

        Vector3 toAim = positionTarget - transform.localPosition;
        if (toAim.magnitude <= Time.deltaTime)
        {
            transform.localPosition = positionTarget;
            transform.rotation = rotationTarget;
            return false;
        }
        else
            return true;
    }

    public void RotateWithPlayer(Vector3 targetRotation)
    {
        cameraContainer.transform.eulerAngles = targetRotation;
    }

    private void GetInput()
    {
        if (UseNewInputSystem)
        {
            Vector2 input = playerInput.Camera.Move.ReadValue<Vector2>();
            inputHor = input.x;
            inputVert = input.y;
        }
        else
        {
            inputHor = Input.GetAxis("CameraHorizontal");
            inputVert = Input.GetAxis("CameraVertical");
        }
    }
}
