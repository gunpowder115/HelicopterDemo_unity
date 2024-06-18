using System.Collections.Generic;
using UnityEngine;
using static InputController;

[RequireComponent(typeof(InputController))]
[RequireComponent(typeof(Translation))]
[RequireComponent(typeof(Shooter))]

public class Player : MonoBehaviour
{
    [SerializeField] float changeSpeedInput = 0.7f;
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
    [SerializeField] Health health;

    bool rotateToDirection;
    float yawAngle;
    float currVerticalSpeed, targetVerticalSpeed;
    Vector3 currSpeed, targetSpeed;
    Vector3 targetDirection;
    GameObject possibleTarget, selectedTarget, possiblePlatform, selectedPlatform;
    Translation translation;
    Rotation rotation;
    Crosshair crosshairController;
    NpcController npcController;
    PlatformController platformController;
    InputController inputController;
    Shooter shooter;

    public bool Aiming { get; private set; }
    public Vector3 AimAngles { get; private set; }
    public Vector3 CurrentDirection { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        translation = GetComponent<Translation>();
        rotation = GetComponentInChildren<Rotation>();
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
        CurrentDirection = transform.forward;
        lineRenderer.enabled = false;

        //hide cursor in center of screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputDirection = inputController.GetInput();
        float inputX = inputDirection.x;
        float inputZ = inputDirection.y;

        if (!health.IsAlive)
        {
            Respawn();
            health.SetAlive(true);
        }
        else
            Translate(inputX, inputZ);

        //rotation around X, Y, Z
        if (rotation != null)
            Rotate(inputX);

        if (shooter && inputController.MinigunFire)
            shooter.BarrelFire(selectedTarget);

        if (inputController.PlayerState == PlayerStates.Normal)
            DrawLineToTarget();
        else
            lineRenderer.enabled = false;

        if (inputController.PlayerState == PlayerStates.Aiming &&
            (!selectedTarget || AimAngles.x > 45f || (selectedTarget.transform.position - transform.position).magnitude > maxDistToAim))
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
        }
        else
        {
            Vector3 inputXYZ = new Vector3(inputX, inputY, inputZ);

            if (inputController.FastMoving)
            {
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
    }

    void Rotate(float inputX)
    {
        CurrentDirection = rotation.CurrentDirection;
        AimAngles = rotation.AimAngles;
        yawAngle = rotation.YawAngle;

        if (inputController.PlayerState == PlayerStates.Aiming && selectedTarget)
        {
            Quaternion rotToTarget = Quaternion.LookRotation((selectedTarget.transform.position - this.transform.position));
            rotation.RotateToTarget(rotToTarget, inputX);
        }
        else
        {
            var direction = targetDirection != Vector3.zero ? targetDirection : CurrentDirection;
            var speedCoef = targetDirection != Vector3.zero ? currSpeed.magnitude / speed : 0f;
            rotation.RotateToDirection(direction, speedCoef, rotateToDirection);
        }
    }

    void ChangeAimState()
    {
        Aiming = !Aiming;
        selectedTarget = Aiming ? possibleTarget : null;
        if (Aiming) lineRenderer.enabled = false;
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

    void Respawn()
    {
        transform.position = new Vector3(0, 10, 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public enum Axis_Proto : int
    { X, Y, Z }

    public enum TargetTypes
    {
        None,
        Enemy,
        Platform
    }
}
