using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] float maxHorizontalAngle = 30f;
    [SerializeField] float maxVerticalAngle_cameraUp = 40f;
    [SerializeField] float maxVerticalAngle_cameraDown = 5f;
    [SerializeField] float rotSpeed = 1f;
    [SerializeField] float rotSpeedManual = 1f;
    [SerializeField] float aimingSpeed = 3f;

    public bool UseNewInputSystem { get; set; }

    GameObject cameraContainer;

    readonly float defaultVerticalAngle = 15f;
    readonly Vector3 cameraAimingPosition = new Vector3(2.08f, 2.26f, -0.89f);
    readonly Vector3 cameraAimingRotation = new Vector3(0f, 0f, 0f);
    readonly Vector3 cameraDefaultPosition = new Vector3(0f, 11.58f, -22.22f);
    readonly Vector3 cameraDefaultRotation = new Vector3(15f, 0f, 0f);

    void Start()
    {
        cameraContainer = GameObject.FindGameObjectWithTag("CameraContainer");
    }

    public void RotateHorizontally(float playerDirX, float inputHor)
    {
        playerDirX += inputHor;

        float targetCameraHorRot = playerDirX * maxHorizontalAngle;

        Vector3 eulerAnglesCurrent = transform.rotation.eulerAngles;
        float currRotSpeed = inputHor != 0f ? rotSpeedManual : rotSpeed;
        Vector3 eulerAnglesTarget = new Vector3(eulerAnglesCurrent.x, targetCameraHorRot, eulerAnglesCurrent.z);

        Quaternion rotationTarget = Quaternion.Euler(eulerAnglesTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationTarget, currRotSpeed * Time.deltaTime);
    }

    public void RotateVertically(float playerDirZ, float inputVert)
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

    public bool ChangeCameraState(bool needToAim, Vector3 targetRotation)
    {
        Quaternion rotationTarget, containerRotationTarget;
        Vector3 positionTarget;
        if (needToAim)
        {
            rotationTarget = Quaternion.Euler(cameraAimingRotation);
            positionTarget = cameraAimingPosition;
            containerRotationTarget = Quaternion.Euler(targetRotation);
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
        if (toAim.magnitude <= Time.deltaTime)
        {
            transform.localPosition = positionTarget;
            transform.localRotation = rotationTarget;
            cameraContainer.transform.rotation = containerRotationTarget;
            return false;
        }
        else
            return true;
    }

    public void RotateWithPlayer(Vector3 targetRotation)
    {
        cameraContainer.transform.rotation = Quaternion.Lerp(cameraContainer.transform.rotation, Quaternion.Euler(targetRotation), aimingSpeed * Time.deltaTime);
    }
}
