using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] float maxHorizontalAngle = 30f;
    [SerializeField] float maxVerticalAngle_cameraUp = 40f;
    [SerializeField] float maxVerticalAngle_cameraDown = 5f;
    [SerializeField] float rotSpeed = 1f;
    [SerializeField] float rotSpeedManual = 1f;
    [SerializeField] float aimingSpeed = 3f;

    private bool aiming;
    private Vector2 input, direction;
    private Vector3 aimAngles;
    private GameObject cameraContainer;
    private InputController inputController;

    readonly float magnitudeError = 0.0001f;
    readonly float defaultVerticalAngle = 15f;
    readonly Vector3 cameraAimingPosition = new Vector3(2.08f, 2.26f, -0.89f);
    readonly Vector3 cameraAimingRotation = new Vector3(0, 0, 0);
    readonly Vector3 cameraDefaultPosition = new Vector3(0, 11, -22);
    readonly Vector3 cameraDefaultRotation = new Vector3(15, 0, 0);

    public void SetCameraParams(bool aiming, Vector2 direction, Vector3 aimAngles)
    {
        this.aiming = aiming;
        this.direction = direction;
        this.aimAngles = aimAngles;
    }

    public void CameraMove(ref bool aimingProcess)
    {
        input = inputController.GetCameraInput();

        if (aimingProcess)
            aimingProcess = ChangeCameraState();
        else
        {
            if (!aiming)
            {
                RotateHorizontally();
                RotateVertically();
            }
            else
                RotateWithPlayer();
        }
    }

    private void Start()
    {
        cameraContainer = GameObject.FindGameObjectWithTag("CameraContainer");
        inputController = InputController.singleton;
    }

    private void RotateHorizontally()
    {
        float playerDirX = direction.x;
        float inputHor = input.x;
        playerDirX += inputHor;

        float targetCameraHorRot = playerDirX * maxHorizontalAngle;

        Vector3 eulerAnglesCurrent = transform.rotation.eulerAngles;
        float currRotSpeed = inputHor != 0f ? rotSpeedManual : rotSpeed;
        Vector3 eulerAnglesTarget = new Vector3(eulerAnglesCurrent.x, targetCameraHorRot, eulerAnglesCurrent.z);

        Quaternion rotationTarget = Quaternion.Euler(eulerAnglesTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationTarget, currRotSpeed * Time.deltaTime);
    }

    private void RotateVertically()
    {
        float playerDirZ = direction.y;
        float inputVert = input.y;
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

    private bool ChangeCameraState()
    {
        Quaternion rotationTarget, containerRotationTarget;
        Vector3 positionTarget;
        if (aiming)
        {
            rotationTarget = Quaternion.Euler(cameraAimingRotation);
            positionTarget = cameraAimingPosition;
            containerRotationTarget = Quaternion.Euler(aimAngles);
        }
        else
        {
            rotationTarget = Quaternion.Euler(cameraDefaultRotation);
            positionTarget = cameraDefaultPosition;
            containerRotationTarget = Quaternion.Euler(0f, 0f, 0f);
        }

        transform.localRotation = Quaternion.Lerp(transform.localRotation, rotationTarget, aimingSpeed * Time.deltaTime);
        transform.localPosition = Vector3.Lerp(transform.localPosition, positionTarget, aimingSpeed * Time.deltaTime);
        cameraContainer.transform.rotation = Quaternion.Lerp(cameraContainer.transform.rotation, containerRotationTarget, aimingSpeed * Time.deltaTime);

        Vector3 toAim = positionTarget - transform.localPosition;
        if (toAim.magnitude <= magnitudeError)
        {
            transform.localPosition = positionTarget;
            transform.localRotation = rotationTarget;
            cameraContainer.transform.rotation = containerRotationTarget;
            return false;
        }
        else
            return true;
    }

    private void RotateWithPlayer()
    {
        cameraContainer.transform.rotation = Quaternion.Lerp(cameraContainer.transform.rotation, Quaternion.Euler(aimAngles), aimingSpeed * Time.deltaTime);
    }
}
