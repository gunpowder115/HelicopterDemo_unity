using UnityEngine;
using static InputController;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private bool twoShoulders = false;
    [SerializeField] private float defaultVerticalAngle = 15f;
    [SerializeField] private float maxHorizontalAngle = 30f;
    [SerializeField] private float maxVerticalAngle_cameraUp = 40f;
    [SerializeField] private float maxVerticalAngle_cameraDown = 5f;
    [SerializeField] private float rotSpeed = 1f;
    [SerializeField] private float rotSpeedManual = 1f;
    [SerializeField] private float aimingSpeed = 3f;

    [Header("Camera positions & rotations")]
    [SerializeField] private Vector3 cameraDefaultPos = new Vector3(0, 11, -22);
    [SerializeField] private Vector3 cameraDefaultRot = new Vector3(15, 0, 0); //unused
    [SerializeField] private Vector3 cameraAimPosCenter = new Vector3(0f, 3.8f, -8f);
    [SerializeField] private Vector3 cameraAimPosRight = new Vector3(3f, 3.8f, -8f);
    [SerializeField] private Vector3 cameraAimingRot = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 cameraTgtSelPos = new Vector3(0, 15, -40);

    [SerializeField] private Player player;
    [SerializeField] private GameObject cameraContainer;

    private Vector2 input, direction, playerInput;
    private Vector3 cameraAimPosLeft;
    private Vector3 cameraAimPos;
    private InputController inputController;
    private Crosshair crosshair;

    private bool Aiming => player.Aiming;
    private Vector3 AimAngles => player.AimAngles;
    private Vector3 PlayerDir => player.CurrentDirection;

    private void Start()
    {
        inputController = InputController.singleton;
        crosshair = Crosshair.singleton;
        cameraAimPos = cameraAimPosRight;
    }

    private void Update()
    {
        input = inputController.GetCameraInput();
        playerInput = inputController.GetInput();

        Vector2 toTargetSelection = new Vector2();
        if (inputController.AimMovement)
        {
            crosshair.Translate(playerInput);
            toTargetSelection = crosshair.ToTargetSelection;
        }
        direction = new Vector2(inputController.AimMovement ? toTargetSelection.x : PlayerDir.x,
            inputController.AimMovement ? toTargetSelection.y : 0f);

        if (!Aiming)
        {
            RotateHorizontally();
            RotateVertically();
            SetDefault();
        }
        else
            RotateWithPlayer();
    }

    private void RotateHorizontally()
    {
        float playerDirX = direction.x;
        float inputHor = input.x;
        playerDirX += inputHor;

        float targetCameraHorRot = playerDirX * maxHorizontalAngle;

        Vector3 eulerAnglesCurrent = transform.localRotation.eulerAngles;
        float currRotSpeed = inputHor != 0f ? rotSpeedManual : rotSpeed;
        Vector3 eulerAnglesTarget = new Vector3(eulerAnglesCurrent.x, targetCameraHorRot, eulerAnglesCurrent.z);

        Quaternion rotationTarget = Quaternion.Euler(eulerAnglesTarget);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, rotationTarget, currRotSpeed * Time.deltaTime);
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

        Vector3 eulerAnglesCurrent = transform.localRotation.eulerAngles;
        float currRotSpeed = rotSpeedManual;
        Vector3 eulerAnglesTarget = new Vector3(targetCameraVertRot, eulerAnglesCurrent.y, eulerAnglesCurrent.z);

        Quaternion rotationTarget = Quaternion.Euler(eulerAnglesTarget);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, rotationTarget, currRotSpeed * Time.deltaTime);
    }

    private void RotateWithPlayer()
    {
        cameraContainer.transform.rotation = Quaternion.Lerp(cameraContainer.transform.rotation, Quaternion.Euler(AimAngles), aimingSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(cameraAimingRot), aimingSpeed * Time.deltaTime);

        if (twoShoulders)
        {
            if (playerInput.x > 0f)
                cameraAimPos = cameraAimPosRight;
            else if (playerInput.x < 0f)
            {
                cameraAimPosLeft = new Vector3(-cameraAimPosRight.x, cameraAimPosRight.y, cameraAimPosRight.z);
                cameraAimPos = cameraAimPosLeft;
            }
            else
                cameraAimPos = cameraAimPosCenter;
        }
        else
            cameraAimPos = cameraAimPosRight;
        transform.localPosition = Vector3.Lerp(transform.localPosition, cameraAimPos, aimingSpeed * Time.deltaTime);
    }

    private void SetDefault()
    {
        Vector3 cameraPos = inputController.AimMovement ? cameraTgtSelPos : cameraDefaultPos;
        transform.localPosition = Vector3.Lerp(transform.localPosition, cameraPos, aimingSpeed * Time.deltaTime);
        cameraContainer.transform.rotation = Quaternion.Lerp(cameraContainer.transform.rotation, Quaternion.Euler(0f, 0f, 0f), aimingSpeed * Time.deltaTime);
    }
}
