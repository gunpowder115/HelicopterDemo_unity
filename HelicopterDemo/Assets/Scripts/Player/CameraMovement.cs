using UnityEngine;
using static InputController;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private bool twoShoulders = false;
    [SerializeField] private float maxHorizontalAngle = 30f;
    [SerializeField] private float maxVerticalAngle_cameraUp = 40f;
    [SerializeField] private float maxVerticalAngle_cameraDown = 5f;
    [SerializeField] private float rotSpeed = 1f;
    [SerializeField] private float rotSpeedManual = 1f;
    [SerializeField] private float aimingSpeed = 3f;
    [SerializeField] private Player player;
    [SerializeField] private GameObject cameraContainer;

    private Vector2 input, direction, playerInput;
    private Vector3 cameraAimingPosition;
    private InputController inputController;
    private Crosshair crosshair;

    readonly float defaultVerticalAngle = 15f;
    readonly Vector3 cameraAimingPositionRight = new Vector3(2.8f, 2.35f, -3.35f);
    readonly Vector3 cameraAimingPositionLeft = new Vector3(-2.8f, 2.35f, -3.35f);
    readonly Vector3 cameraAimingRotation = new Vector3(0, 0, 0);
    readonly Vector3 cameraDefaultPosition = new Vector3(0, 11, -22);
    readonly Vector3 cameraDefaultRotation = new Vector3(15, 0, 0);
    readonly Vector3 cameraTgtSelPosition = new Vector3(0, 15, -40);

    private bool Aiming => player.Aiming;
    private Vector3 AimAngles => player.AimAngles;
    private Vector3 PlayerDir => player.CurrentDirection;

    private void Start()
    {
        inputController = InputController.singleton;
        crosshair = Crosshair.singleton;
        cameraAimingPosition = cameraAimingPositionRight;
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
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(cameraAimingRotation), aimingSpeed * Time.deltaTime);

        if (twoShoulders)
        {
            if (playerInput.x > 0f)
                cameraAimingPosition = cameraAimingPositionRight;
            else if (playerInput.x < 0f)
                cameraAimingPosition = cameraAimingPositionLeft;
        }
        else
            cameraAimingPosition = cameraAimingPositionRight;
        transform.localPosition = Vector3.Lerp(transform.localPosition, cameraAimingPosition, aimingSpeed * Time.deltaTime);
    }

    private void SetDefault()
    {
        Vector3 cameraPos = inputController.AimMovement ? cameraTgtSelPosition : cameraDefaultPosition;
        transform.localPosition = Vector3.Lerp(transform.localPosition, cameraPos, aimingSpeed * Time.deltaTime);
        cameraContainer.transform.rotation = Quaternion.Lerp(cameraContainer.transform.rotation, Quaternion.Euler(0f, 0f, 0f), aimingSpeed * Time.deltaTime);
    }
}
