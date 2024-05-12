using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    bool minigunFire;
    bool verticalFastMove, fastMove;
    int unguidedMissileIndex, guidedMissileIndex;
    float vertDirection;
    float yawAngle;
    float currSpeed;
    Vector3 targetDirection;
    Vector3 currentDirection;
    Vector3 aimAngles;
    PlayerStates playerState;
    GameObject possibleTarget, selectedTarget, possiblePlatform, selectedPlatform;
    TranslationInput translationInput;
    RotationInput rotationInput;
    CameraRotation cameraRotation;
    TargetSelectionInput targetSelectionInput;
    PlayerInput playerInput;
    BarrelShooter minigun;
    NpcController npcController;
    PlatformController platformController;
    List<MissileShooter> unguidedMissiles, guidedMissiles;

    private void Awake()
    {
        playerInput = new PlayerInput();

        playerInput.Common.MainAction.performed += context => DoMainAction();
        playerInput.Common.MainAction.canceled += context => DoMainActionCancel();

        playerInput.Common.MinorAction.performed += context => DoMinorAction();
        playerInput.Common.MinorActionHold.performed += context => DoMinorActionHold();

        playerInput.Common.AnyTargetSelection.performed += context => AnyTargetSelection();
        playerInput.Common.AnyTargetSelection.canceled += context => AnyTargetSelectionCancel();

        playerInput.Player.FastMove.performed += context => FastMove();
        playerInput.Player.FastMove.canceled += context => FastMoveCancel();

        playerInput.Player.VerticalFastUp.performed += context => VerticalFastMove(1f);
        playerInput.Player.VerticalFastDown.performed += context => VerticalFastMove(-1f);
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
        targetSelectionInput = GetComponentInChildren<TargetSelectionInput>();
        minigun = GetComponentInChildren<BarrelShooter>();
        npcController = NpcController.GetInstance();
        platformController = PlatformController.GetInstance();

        List<MissileShooter> missiles = new List<MissileShooter>(GetComponentsInChildren<MissileShooter>());
        unguidedMissiles = new List<MissileShooter>();
        guidedMissiles = new List<MissileShooter>();
        if (missiles != null)
        {
            foreach (var missile in missiles)
            {
                if (missile.IsGuided)
                    guidedMissiles.Add(missile);
                else
                    unguidedMissiles.Add(missile);
            }
        }

        rotateToDirection = false;
        targetDirection = transform.forward;
        currentDirection = transform.forward;
        cameraInAim = aiming = false;
        playerState = PlayerStates.Normal;
        lineRenderer.enabled = false;
        currSpeed = speed;

        //hide cursor in center of screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputDirection = GetInput();
        Vector2 inputVerticalDirection = GetVerticalInput();
        float inputX = inputDirection.x;
        float inputY = inputVerticalDirection.y;
        float inputZ = inputDirection.y;
        float inputXZ = Mathf.Clamp01(new Vector3(inputX, 0f, inputZ).magnitude);

        //move aim if it target selection mode
        bool aimMovement = playerState == PlayerStates.SelectionFarTarget || playerState == PlayerStates.SelectionAnyTarget;
        Vector2 toTargetSelection = new Vector2();
        if (targetSelectionInput && aimMovement)
        {
            targetSelectionInput.MoveAim(new Vector2(inputX, inputZ));
            toTargetSelection = targetSelectionInput.ToTargetSelection;
        }

        //movement around X, Y, Z
        bool playerCanTranslate = playerState != PlayerStates.SelectionFarTarget && 
            playerState != PlayerStates.SelectionAnyTarget && playerState != PlayerStates.BuildSelection;
        if (translationInput != null)
        {
            if (inputXZ >= changeSpeedInput && !translationInput.RotToDir ||
                inputXZ < changeSpeedInput && translationInput.RotToDir)
                rotateToDirection = translationInput.SwitchRotation();

            targetDirection = translationInput.TargetDirection;
            if (translationInput.IsHeightBorder && verticalFastMove)
                verticalFastMove = false;

            if (!playerCanTranslate)
                inputX = inputY = inputZ = 0f;

            if (playerState == PlayerStates.Aiming && selectedTarget)
            {
                currSpeed = speed * lowSpeedCoef;
                translationInput.TranslateRelToTarget(new Vector3(inputX, inputY, inputZ), yawAngle, currSpeed);
            }
            else
            {
                if (fastMove)
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

                translationInput.TranslateGlobal(new Vector3(inputX, verticalFastMove ? vertFastCoef * vertDirection : inputY, inputZ), currSpeed);
            }
        }

        //rotation around X, Y, Z
        if (rotationInput != null)
        {
            currentDirection = rotationInput.CurrentDirection;
            aimAngles = rotationInput.AimAngles;
            yawAngle = rotationInput.YawAngle;

            if (playerState == PlayerStates.Aiming && selectedTarget)
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
            bool rotateWithTargetSelection = playerState == PlayerStates.SelectionFarTarget || playerState == PlayerStates.SelectionAnyTarget;

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

        if (minigun && minigunFire)
            minigun.Fire(selectedTarget);

        if (playerState == PlayerStates.Normal)
            DrawLineToTarget();

        if (playerState == PlayerStates.Aiming && (aimAngles.x > 45f || (selectedTarget.transform.position - transform.position).magnitude > maxDistToAim))
            ChangeAimState();

        Debug.Log(playerState);
    }

    private Vector2 GetInput()
    {
        Vector2 input;
        if (useNewInputSystem)
        {
            input = playerInput.Player.Move.ReadValue<Vector2>();
        }
        else
        {
            float inputX = Input.GetAxis("Horizontal");
            float inputZ = Input.GetAxis("Vertical");
            input = new Vector2(inputX, inputZ);
        }
        return input;
    }

    private Vector2 GetVerticalInput()
    {
        Vector2 input = new Vector2(0f, 0f);
        if (useNewInputSystem)
            input = playerInput.Player.VerticalMove.ReadValue<Vector2>();
        return input;
    }

    private void VerticalFastMove(float dir)
    {
        if (playerState == PlayerStates.Normal)
        {
            verticalFastMove = true;
            vertDirection = dir;
        }
    }

    private void FastMove()
    {
        if (playerState == PlayerStates.Normal)
            fastMove = true;
        else if (playerState == PlayerStates.Aiming)
            ChangeAimState();
    }

    private void FastMoveCancel()
    {
        fastMove = false;
    }

    private void DoMainAction()
    {
        if (playerState == PlayerStates.Normal || playerState == PlayerStates.Aiming)
            minigunFire = true;
    }

    private void DoMainActionCancel()
    {
        minigunFire = false;
        if (minigun) minigun.StopFire();
    }

    private void DoMinorAction()
    {
        if (playerState == PlayerStates.Normal && (possibleTarget || possiblePlatform))
        {
            if (possibleTarget) ChangeAimState();
            else if (possiblePlatform)
            {
                selectedPlatform = possiblePlatform;
                lineRenderer.enabled = false;
                playerState = PlayerStates.BuildSelection;
            }
        }
        else if (playerState == PlayerStates.BuildSelection)
        {
            selectedPlatform = null;
            playerState = PlayerStates.Normal;
        }
        else if (playerState == PlayerStates.SelectionFarTarget && guidedMissiles[guidedMissileIndex].IsEnable)
        {
            guidedMissiles[guidedMissileIndex++].Launch(null);
            if (guidedMissileIndex >= guidedMissiles.Count) guidedMissileIndex = 0;
            targetSelectionInput.HideAim();
            playerState = PlayerStates.Normal;
        }
        else if (playerState != PlayerStates.SelectionAnyTarget && playerState != PlayerStates.SelectionFarTarget &&
            unguidedMissiles[unguidedMissileIndex].IsEnable)
        {
            unguidedMissiles[unguidedMissileIndex++].Launch(selectedTarget);
            if (unguidedMissileIndex >= unguidedMissiles.Count) unguidedMissileIndex = 0;
        }
    }

    void ChangeAimState()
    {
        cameraInAim = !cameraInAim;
        aiming = true;
        playerState = cameraInAim ? PlayerStates.Aiming : PlayerStates.Normal;
        selectedTarget = cameraInAim ? possibleTarget : null;
        if (playerState == PlayerStates.Aiming) lineRenderer.enabled = false;
    }

    private void DoMinorActionHold()
    {
        if (playerState == PlayerStates.Normal)
        {
            targetSelectionInput.ShowAim();
            playerState = PlayerStates.SelectionFarTarget;
        }
    }

    private void AnyTargetSelection()
    {
        if (playerState == PlayerStates.Normal)
        {
            targetSelectionInput.ShowAim();
            playerState = PlayerStates.SelectionAnyTarget;
        }
    }

    private void AnyTargetSelectionCancel()
    {
        if (playerState == PlayerStates.SelectionAnyTarget)
        {
            targetSelectionInput.HideAim();
            playerState = PlayerStates.Normal;
        }
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

    public enum Axis_Proto : int
    { X, Y, Z }

    public enum PlayerStates
    {
        Normal,
        Aiming,
        SelectionFarTarget,
        SelectionAnyTarget,
        BuildSelection
    }

    public enum TargetTypes
    {
        None,
        Enemy,
        Platform
    }
}
