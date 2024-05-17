using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static InputController;

[RequireComponent(typeof(InputController))]
[RequireComponent(typeof(Shooter))]

public class InputManager : MonoBehaviour
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
    Vector3 targetDirection;
    Vector3 currentDirection;
    Vector3 aimAngles;
    GameObject possibleTarget, selectedTarget, possiblePlatform, selectedPlatform;
    TranslationInput translationInput;
    RotationInput rotationInput;
    CameraRotation cameraRotation;
    CrosshairController crosshairController;
    NpcController npcController;
    PlatformController platformController;
    InputController inputController;
    Shooter shooter;

    // Start is called before the first frame update
    void Start()
    {
        translationInput = GetComponentInChildren<TranslationInput>();
        rotationInput = GetComponentInChildren<RotationInput>();
        cameraRotation = GetComponentInChildren<CameraRotation>();
        shooter = GetComponent<Shooter>();

        npcController = NpcController.singleton;
        platformController = PlatformController.singleton;
        crosshairController = CrosshairController.singleton;

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
        float inputX = inputDirection.x;
        float inputY = inputVerticalDirection.y;
        float inputZ = inputDirection.y;
        float inputXZ = Mathf.Clamp01(new Vector3(inputX, 0f, inputZ).magnitude);

        Vector2 toTargetSelection = new Vector2();
        if (crosshairController && inputController.AimMovement)
        {
            crosshairController.MoveAim(new Vector2(inputX, inputZ));
            toTargetSelection = crosshairController.ToTargetSelection;
        }

        //movement around X, Y, Z
        if (translationInput != null)
        {
            if (inputXZ >= changeSpeedInput && !translationInput.RotToDir ||
                inputXZ < changeSpeedInput && translationInput.RotToDir)
                rotateToDirection = translationInput.SwitchRotation();

            targetDirection = translationInput.TargetDirection;
            if (translationInput.IsHeightBorder && inputController.VertFastMoving)
                inputController.ForceStopVertFastMoving();

            if (!inputController.PlayerCanTranslate)
                inputX = inputY = inputZ = 0f;

            if (inputController.PlayerState == PlayerStates.Aiming && selectedTarget)
            {
                currSpeed = speed * lowSpeedCoef;
                translationInput.TranslateRelToTarget(new Vector3(inputX, inputY, inputZ), yawAngle, currSpeed);
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

                translationInput.TranslateGlobal(new Vector3(inputX,
                    inputController.VertFastMoving ? vertFastCoef * inputController.VertDirection : inputY, inputZ), currSpeed);
            }
        }

        //rotation around X, Y, Z
        if (rotationInput != null)
        {
            currentDirection = rotationInput.CurrentDirection;
            aimAngles = rotationInput.AimAngles;
            yawAngle = rotationInput.YawAngle;

            if (inputController.PlayerState == PlayerStates.Aiming && selectedTarget)
            {
                Quaternion rotToTarget = Quaternion.LookRotation((selectedTarget.transform.position - this.transform.position));
                rotationInput.RotateToTarget(rotToTarget, inputX);
            }
            else
            {
                var direction = targetDirection != Vector3.zero ? targetDirection : currentDirection;
                var speedCoef = targetDirection != Vector3.zero ? currSpeed / speed : 0f;
                rotationInput.RotateToDirection(direction, speedCoef, rotateToDirection);
            }
        }

        //camera rotation
        if (cameraRotation)
        {
            cameraRotation.UseNewInputSystem = useNewInputSystem;
            bool rotateWithTargetSelection = inputController.PlayerState == PlayerStates.SelectionFarTarget ||
                inputController.PlayerState == PlayerStates.SelectionAnyTarget;

            if (aiming)
                aiming = cameraRotation.ChangeCameraState(cameraInAim, aimAngles);
            else
            {
                if (!cameraInAim)
                {
                    cameraRotation.RotateHorizontally(rotateWithTargetSelection ? toTargetSelection.x : currentDirection.x);
                    cameraRotation.RotateVertically(rotateWithTargetSelection ? toTargetSelection.y : 0f);
                }
                else
                    cameraRotation.RotateWithPlayer(aimAngles);
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
        if (cameraInAim) lineRenderer.enabled = false;
    }

    void DrawLineToTarget()
    {
        KeyValuePair<float, GameObject> nearest;
        TargetTypes targetType;
        if (FindNearestObject(out nearest, out targetType))
        {
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
    }

    bool FindNearestObject(out KeyValuePair<float, GameObject> nearest, out TargetTypes targetType)
    {
        var distToEnemies = npcController.FindDistToEnemies(this.gameObject);
        var distToPlatforms = platformController.FindDistToPlatforms(this.gameObject);
        bool areEnemies = distToEnemies.Count > 0;
        bool arePlatforms = distToPlatforms.Count > 0;

        var distToNearestEnemy = areEnemies ? distToEnemies.ElementAt(0) : new KeyValuePair<float, GameObject>(Mathf.Infinity, null);
        var distToNearestPlatform = arePlatforms ? distToPlatforms.ElementAt(0) : new KeyValuePair<float, GameObject>(Mathf.Infinity, null);

        if (areEnemies || arePlatforms)
        {
            if (distToNearestEnemy.Key < distToNearestPlatform.Key)
            {
                nearest = distToNearestEnemy;
                targetType = TargetTypes.Enemy;
            }
            else
            {
                nearest = distToNearestPlatform;
                targetType = TargetTypes.Platform;
            }
            return true;
        }
        else
        {
            nearest = new KeyValuePair<float, GameObject>(Mathf.Infinity, null);
            targetType = TargetTypes.None;
            return false;
        }
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
        crosshairController.HideAim();
    }

    void StartSelectionFarTarget() => crosshairController.ShowAim();

    void StartSelectionAnyTarget() => crosshairController.ShowAim();

    void CancelSelectionAnytarget() => crosshairController.HideAim();

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
