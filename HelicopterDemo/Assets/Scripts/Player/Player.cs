using System.Collections.Generic;
using UnityEngine;
using static InputController;

[RequireComponent(typeof(InputController))]
[RequireComponent(typeof(Shooter))]

public class Player : MonoBehaviour
{
    [SerializeField] float changeSpeedInput = 0.7f;
    [SerializeField] bool useNewInputSystem = true;
    [SerializeField] float vertFastCoef = 5f;
    [SerializeField] float minDistToAim = 17f;
    [SerializeField] float maxDistToAim = 20f;
    [SerializeField] float speed = 20f;
    [SerializeField] float lowSpeedCoef = 0.5f;
    [SerializeField] float highSpeedCoef = 3f;
    [SerializeField] float verticalSpeed = 30f;
    [SerializeField] float lateralMovingCoef = 0.1f;
    [SerializeField] float acceleration = 1f;
    [SerializeField] LineRenderer lineRenderer;

    bool rotateToDirection;
    bool cameraInAim, aiming;
    float yawAngle;
    float currVerticalSpeed, targetVerticalSpeed;
    Vector3 currSpeed, targetSpeed;
    Vector3 targetDirection;
    Vector3 currentDirection;
    Vector3 aimAngles;
    GameObject possibleTarget, selectedTarget, possiblePlatform, selectedPlatform;
    Translation translation;
    Rotation rotation;
    PlayerCamera playerCamera;
    Crosshair crosshairController;
    NpcController npcController;
    PlatformController platformController;
    InputController inputController;
    Shooter shooter;

    // Start is called before the first frame update
    void Start()
    {
        translation = GetComponentInChildren<Translation>();
        rotation = GetComponentInChildren<Rotation>();
        playerCamera = GetComponentInChildren<PlayerCamera>();
        shooter = GetComponent<Shooter>();

        npcController = NpcController.singleton;
        platformController = PlatformController.singleton;
        crosshairController = Crosshair.singleton;

        inputController = InputController.singleton;
        if (!inputController) return;
        inputController.TryBindingToObject += TryBindingToObject;
        inputController.TryLaunchUnguidedMissile += TryLaunchUnguidedMissile;
        inputController.CancelBuildSelection += CancelBuildSelection;
        inputController.TryLaunchGuidedMissile += TryLaunchGuidedMissile;
        inputController.StartSelectionFarTarget += StartSelectionFarTarget;
        inputController.StartSelectionAnyTarget += StartSelectionAnyTarget;
        inputController.CancelSelectionAnytarget += CancelSelectionAnytarget;
        inputController.CancelAiming += CancelAiming;

        rotateToDirection = false;
        targetDirection = transform.forward;
        currentDirection = transform.forward;
        cameraInAim = aiming = false;
        lineRenderer.enabled = false;

        //hide cursor in center of screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputDirection = inputController.GetInput();
        Vector2 cameraInput = inputController.GetCameraInput();
        float inputX = inputDirection.x;
        float inputZ = inputDirection.y;

        //movement around X, Y, Z
        if (translation != null)
            Translate(inputX, inputZ);

        //rotation around X, Y, Z
        if (rotation != null)
            Rotate(inputX);

        //camera rotation
        if (playerCamera)
            RotateCamera(cameraInput, inputX, inputZ);

        if (shooter)
        {
            if (inputController.MinigunFire)
                shooter.BarrelFire(selectedTarget);
            else
                shooter.StopBarrelFire();
        }

        if (inputController.PlayerState == PlayerStates.Normal)
            DrawLineToTarget();

        if (inputController.PlayerState == PlayerStates.Aiming &&
            (aimAngles.x > 45f || (selectedTarget.transform.position - transform.position).magnitude > maxDistToAim))
        {
            inputController.ForceChangePlayerState(PlayerStates.Normal);
            ChangeAimState();
        }
    }

    void Translate(float inputX, float inputZ)
    {
        float inputVerticalDirection = inputController.VerticalMoving;
        float inputVerticalFast = inputController.VerticalFastMoving;

        float inputXZ = Mathf.Clamp01(new Vector3(inputX, 0f, inputZ).magnitude);
        float inputY = inputVerticalFast != 0f ? inputVerticalFast : inputVerticalDirection;

        if (inputXZ >= changeSpeedInput && !translation.RotToDir ||
            inputXZ < changeSpeedInput && translation.RotToDir)
            rotateToDirection = translation.SwitchRotation();

        targetDirection = translation.TargetDirectionNorm;

        if (!inputController.PlayerCanTranslate)
            inputX = inputY = inputZ = 0f;

        if (inputController.PlayerState == PlayerStates.Aiming && selectedTarget)
        {
            Vector3 inputXYZ = new Vector3(inputX, inputY, inputZ);
            inputXYZ = BalanceDistToTarget(inputXYZ);
            targetSpeed = Vector3.ClampMagnitude(inputXYZ * speed * lowSpeedCoef, speed * lowSpeedCoef);
            currSpeed = Vector3.Lerp(currSpeed, targetSpeed, acceleration * Time.deltaTime);

            translation.SetRelToTargetTranslation(currSpeed, yawAngle);

            Debug.Log((selectedTarget.transform.position - transform.position).magnitude);
        }
        else
        {
            Vector3 inputXYZ = new Vector3(inputX, inputY, inputZ);

            if (inputController.FastMoving)
            {
                inputX = (inputX == 0f ? currentDirection.x : inputX);
                inputZ = (inputZ == 0f ? currentDirection.z : inputZ);
                inputXYZ = new Vector3(inputX, inputY, inputZ);
                targetSpeed = Vector3.ClampMagnitude(inputXYZ * speed * highSpeedCoef, speed * highSpeedCoef);
            }
            else if (inputXZ == 0f)
                targetSpeed = Vector3.zero;
            else
                targetSpeed = Vector3.ClampMagnitude(inputXYZ * speed, speed);

            currSpeed = Vector3.Lerp(currSpeed, targetSpeed, acceleration * Time.deltaTime);
            translation.SetGlobalTranslation(currSpeed);
        }

        targetVerticalSpeed = inputY * verticalSpeed;
        if (inputVerticalFast != 0f) targetVerticalSpeed *= vertFastCoef;
        currVerticalSpeed = Mathf.Lerp(currVerticalSpeed, targetVerticalSpeed, acceleration * Time.deltaTime);
        translation.SetVerticalTranslation(currVerticalSpeed);

        translation.Translate();
    }

    void Rotate(float inputX)
    {
        currentDirection = rotation.CurrentDirection;
        aimAngles = rotation.AimAngles;
        yawAngle = rotation.YawAngle;

        if (inputController.PlayerState == PlayerStates.Aiming && selectedTarget)
        {
            Quaternion rotToTarget = Quaternion.LookRotation((selectedTarget.transform.position - this.transform.position));
            rotation.RotateToTarget(rotToTarget, inputX);
        }
        else
        {
            var direction = targetDirection != Vector3.zero ? targetDirection : currentDirection;
            var speedCoef = targetDirection != Vector3.zero ? currSpeed.magnitude / speed : 0f;
            rotation.RotateToDirection(direction, speedCoef, rotateToDirection);
        }
    }

    void RotateCamera(Vector3 cameraInput, float inputX, float inputZ)
    {
        Vector2 toTargetSelection = new Vector2();
        if (crosshairController && inputController.AimMovement)
        {
            crosshairController.Translate(new Vector2(inputX, inputZ));
            toTargetSelection = crosshairController.ToTargetSelection;
        }

        playerCamera.UseNewInputSystem = useNewInputSystem;
        bool rotateWithTargetSelection = inputController.PlayerState == PlayerStates.SelectionFarTarget ||
            inputController.PlayerState == PlayerStates.SelectionAnyTarget;

        if (aiming)
            aiming = playerCamera.ChangeCameraState(cameraInAim, aimAngles);
        else
        {
            if (!cameraInAim)
            {
                playerCamera.RotateHorizontally(rotateWithTargetSelection ? toTargetSelection.x : currentDirection.x, cameraInput.x);
                playerCamera.RotateVertically(rotateWithTargetSelection ? toTargetSelection.y : 0f, cameraInput.y);
            }
            else
                playerCamera.RotateWithPlayer(aimAngles);
        }
    }

    void ChangeAimState()
    {
        cameraInAim = !cameraInAim;
        aiming = true;
        selectedTarget = cameraInAim ? possibleTarget : null;
        if (cameraInAim) lineRenderer.enabled = false;
    }

    void DrawLineToTarget()
    {
        KeyValuePair<float, GameObject> nearest;
        TargetTypes targetType;
        var nearestNpc = npcController.FindNearestEnemy(gameObject);
        var nearestPlatform = platformController.FindNearestPlatform(gameObject);

        nearest = nearestNpc.Key < nearestPlatform.Key ? nearestNpc : nearestPlatform;
        targetType = nearestNpc.Key < nearestPlatform.Key ? TargetTypes.Enemy : TargetTypes.Platform;

        if (nearest.Key < minDistToAim)
        {
            lineRenderer.enabled = true;
            Color lineColor = targetType == TargetTypes.Enemy ? Color.red : Color.blue;
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
            lineRenderer.SetPosition(0, this.transform.position);
            lineRenderer.SetPosition(1, nearest.Value.transform.position);
            possibleTarget = targetType == TargetTypes.Enemy ? nearest.Value : null;
            possiblePlatform = targetType == TargetTypes.Platform ? nearest.Value : null;
        }
        else
        {
            lineRenderer.enabled = false;
            possibleTarget = possiblePlatform = null;
        }
    }

    Vector3 BalanceDistToTarget(Vector3 input)
    {
        if (input.z == 0f)
            input.z = Mathf.Abs(input.x) * lateralMovingCoef;
        return input;
    }

    PlayerStates TryBindingToObject(PlayerStates playerState)
    {
        if (possibleTarget)
        {
            ChangeAimState();
            return PlayerStates.Aiming;
        }
        else if (possiblePlatform)
        {
            StartBuildSelection();
            return PlayerStates.BuildSelection;
        }
        else
        {
            TryLaunchUnguidedMissile();
            return playerState;
        }
    }

    void TryLaunchUnguidedMissile()
    {
        if (shooter) shooter.UnguidedMissileLaunch(selectedTarget);
    }

    void StartBuildSelection()
    {
        selectedPlatform = possiblePlatform;
        lineRenderer.enabled = false;
    }

    void CancelBuildSelection() => selectedPlatform = null;

    void TryLaunchGuidedMissile()
    {
        if (shooter) shooter.GuidedMissileLaunch(selectedTarget);
        crosshairController.Hide();
    }

    void StartSelectionFarTarget() => crosshairController.Show();

    void StartSelectionAnyTarget() => crosshairController.Show();

    void CancelSelectionAnytarget() => crosshairController.Hide();

    void CancelAiming() => ChangeAimState();

    public enum Axis_Proto : int
    { X, Y, Z }

    public enum TargetTypes
    {
        None,
        Enemy,
        Platform
    }
}
