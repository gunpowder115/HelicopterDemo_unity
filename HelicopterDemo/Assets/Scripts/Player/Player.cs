using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] float fastSpeedReduction = 3f;
    [SerializeField] float speedReductionDead = 0.05f;
    [SerializeField] LineRenderer lineRenderer;

    bool rotateToDirection;
    bool cameraInAim, aiming;
    float yawAngle;
    float currSpeed;
    float refDistToTargetEnemy;
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
        currSpeed = speed;

        //hide cursor in center of screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputDirection = inputController.GetInput();
        Vector2 inputVerticalDirection = inputController.GetVerticalInput();
        Vector2 cameraInput = inputController.GetCameraInput();
        float inputX = inputDirection.x;
        float inputY = inputVerticalDirection.y;
        float inputZ = inputDirection.y;
        float inputXZ = Mathf.Clamp01(new Vector3(inputX, 0f, inputZ).magnitude);

        Vector2 toTargetSelection = new Vector2();
        if (crosshairController && inputController.AimMovement)
        {
            crosshairController.Translate(new Vector2(inputX, inputZ));
            toTargetSelection = crosshairController.ToTargetSelection;
        }

        //movement around X, Y, Z
        if (translation != null)
        {
            if (inputXZ >= changeSpeedInput && !translation.RotToDir ||
                inputXZ < changeSpeedInput && translation.RotToDir)
                rotateToDirection = translation.SwitchRotation();

            targetDirection = translation.TargetDirection;
            if (translation.IsHeightBorder && inputController.VertFastMoving)
                inputController.ForceStopVertFastMoving();

            if (!inputController.PlayerCanTranslate)
                inputX = inputY = inputZ = 0f;

            if (inputController.PlayerState == PlayerStates.Aiming && selectedTarget)
            {
                currSpeed = speed * lowSpeedCoef;
                Vector3 inputXYZ = new Vector3(inputX, inputY, inputZ);
                //inputXYZ = FixDistToTarget(inputXYZ);
                translation.TranslateRelToTarget(inputXYZ, yawAngle, currSpeed);
                Debug.Log((selectedTarget.transform.position - transform.position).magnitude);
            }
            else
            {
                if (inputController.FastMoving)
                {
                    currSpeed = speed * highSpeedCoef;
                    inputX = (inputX == 0f ? currentDirection.x : inputX);
                    inputZ = (inputZ == 0f ? currentDirection.z : inputZ);
                }
                else if (currSpeed - speed > speedReductionDead * speed)
                {
                    currSpeed = Mathf.LerpUnclamped(currSpeed, speed, fastSpeedReduction * Time.deltaTime);
                    inputX = (inputX == 0f ? currentDirection.x : inputX);
                    inputZ = (inputZ == 0f ? currentDirection.z : inputZ);
                }
                else
                    currSpeed = speed;

                translation.TranslateGlobal(new Vector3(inputX,
                    inputController.VertFastMoving ? vertFastCoef * inputController.VertDirection : inputY, inputZ), currSpeed);
            }
        }

        //rotation around X, Y, Z
        if (rotation != null)
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
                var speedCoef = targetDirection != Vector3.zero ? currSpeed / speed : 0f;
                rotation.RotateToDirection(direction, speedCoef, rotateToDirection);
            }
        }

        //camera rotation
        if (playerCamera)
        {
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

    void ChangeAimState()
    {
        cameraInAim = !cameraInAim;
        aiming = true;
        selectedTarget = cameraInAim ? possibleTarget : null;
        refDistToTargetEnemy = selectedTarget ? (selectedTarget.transform.position - transform.position).magnitude : 0f;
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

    Vector3 FixDistToTarget(Vector3 input)
    {
        bool toTarget = (selectedTarget.transform.position - transform.position).magnitude > refDistToTargetEnemy + 1f;
        bool fromTarget = (selectedTarget.transform.position - transform.position).magnitude < refDistToTargetEnemy - 1f;

        if (toTarget && input.z == 0f)
            input.z = 1f;
        else if (fromTarget && input.z == 0f)
            input.z = -1f;
        else if (input.z != 0f)
            refDistToTargetEnemy = (selectedTarget.transform.position - transform.position).magnitude;

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
