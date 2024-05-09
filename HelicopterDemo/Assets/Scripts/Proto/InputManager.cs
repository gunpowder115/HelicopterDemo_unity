using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] float changeSpeedInput = 0.7f;
    [SerializeField] bool useNewInputSystem = true;
    [SerializeField] float vertFastCoef = 5f;
    [SerializeField] float maxDistToAim = 20f;
    [SerializeField] LineRenderer lineRenderer;

    bool rotateToDirection;
    bool cameraInAim, aiming;
    bool minigunFire;
    int unguidedMissileIndex, guidedMissileIndex;
    float vertDirection;
    float yawAngle;
    Vector3 targetDirection;
    Vector3 currentDirection;
    Vector3 aimAngles;
    PlayerStates playerState;
    GameObject possibleTarget, selectedTarget;
    TranslationInput translationInput;
    RotationInput rotationInput;
    CameraRotation cameraRotation;
    TargetSelectionInput targetSelectionInput;
    PlayerInput playerInput;
    BarrelShooter minigun;
    NpcController npcController;
    List<MissileShooter> unguidedMissiles, guidedMissiles;

    private void Awake()
    {
        playerInput = new PlayerInput();

        playerInput.Common.MainAction.performed += context => DoMainAction();
        playerInput.Common.MainAction.canceled += context => DoMainActionCancel();

        playerInput.Common.MinorAction.performed += context => DoMinorAction();
        playerInput.Common.MinorActionHold.performed += context => DoMinorActionHold();

        playerInput.Common.AnyTargetSelection.started += context => DoMinorActionCancel(); //todo
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

        List<MissileShooter> missiles = new List<MissileShooter>(GetComponentsInChildren<MissileShooter>());
        unguidedMissiles = new List<MissileShooter>();
        guidedMissiles = new List<MissileShooter>();
        if (missiles != null)
        {
            foreach(var missile in missiles)
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
        bool playerCanTranslate = playerState != PlayerStates.SelectionFarTarget && playerState != PlayerStates.SelectionAnyTarget;
        if (translationInput != null)
        {
            if (inputXZ >= changeSpeedInput && !translationInput.CurrSpeedIsHigh ||
                inputXZ < changeSpeedInput && translationInput.CurrSpeedIsHigh)
                rotateToDirection = translationInput.ChangeSpeed();

            targetDirection = translationInput.TargetDirection;
            if (translationInput.IsHeightBorder && playerState == PlayerStates.VerticalFastMoving)
                playerState = PlayerStates.Normal;

            if (!playerCanTranslate)
                inputX = inputY = inputZ = 0f;

            if (playerState == PlayerStates.Aiming && selectedTarget)
                translationInput.TranslateRelToTarget(new Vector3(inputX, inputY, inputZ), yawAngle);
            else
            {
                translationInput.Translate(Axis_Proto.X, inputX);
                translationInput.Translate(Axis_Proto.Y, playerState == PlayerStates.VerticalFastMoving ? vertFastCoef * vertDirection : inputY);
                translationInput.Translate(Axis_Proto.Z, inputZ);
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
                rotationInput.RotateToDirection(targetDirection, inputXZ, rotateToDirection);
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
            minigun.Fire();

        if (playerState == PlayerStates.Normal)
            DrawLineToEnemy();

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
            playerState = PlayerStates.VerticalFastMoving;
            vertDirection = dir;
        }
    }

    private void FastMove()
    {
        if (playerState == PlayerStates.Normal)
        {
            playerState = PlayerStates.FastMoving;
        }
    }

    private void FastMoveCancel()
    {
        if (playerState == PlayerStates.FastMoving)
        {
            playerState = PlayerStates.Normal;
        }
    }

    private void DoMainAction()
    {
        if (playerState != PlayerStates.SelectionAnyTarget && playerState != PlayerStates.SelectionFarTarget)
            minigunFire = true;
    }

    private void DoMainActionCancel()
    {
        minigunFire = false;
        if (minigun) minigun.StopFire();
    }

    private void DoMinorAction()
    {
        if ((playerState == PlayerStates.Normal && possibleTarget) || playerState == PlayerStates.Aiming)
        {
            cameraInAim = !cameraInAim;
            aiming = true;
            playerState = cameraInAim ? PlayerStates.Aiming : PlayerStates.Normal;
            selectedTarget = cameraInAim ? possibleTarget : null;
        }
        else
        {
            if (playerState == PlayerStates.SelectionFarTarget && guidedMissiles[guidedMissileIndex].IsEnable)
            {
                guidedMissiles[guidedMissileIndex++].Launch();
                if (guidedMissileIndex >= guidedMissiles.Count) guidedMissileIndex = 0;
            }
            targetSelectionInput.HideAim();
            playerState = PlayerStates.Normal;
        }
    }

    private void DoMinorActionHold()
    {
        if (playerState == PlayerStates.Normal)
        {
            targetSelectionInput.ShowAim();
            playerState = PlayerStates.SelectionFarTarget;
        }
    }

    private void DoMinorActionCancel()
    {
        if (playerState != PlayerStates.SelectionAnyTarget && playerState != PlayerStates.SelectionFarTarget &&
            unguidedMissiles[unguidedMissileIndex].IsEnable)
        {
            unguidedMissiles[unguidedMissileIndex++].Launch();
            if (unguidedMissileIndex >= unguidedMissiles.Count) unguidedMissileIndex = 0;
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

    void DrawLineToEnemy()
    {
        var distToEnemies = npcController.FindDistToEnemies(this.gameObject);
        if (distToEnemies.Count > 0)
        {
            var distToNearestEnemy = distToEnemies.ElementAt(0);
            float dist = distToNearestEnemy.Key;
            GameObject nearestEnemy = distToNearestEnemy.Value;
            if (dist < maxDistToAim)
            {
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, this.transform.position);
                lineRenderer.SetPosition(1, nearestEnemy.transform.position);
                possibleTarget = nearestEnemy;
            }
            else
            {
                lineRenderer.enabled = false;
                possibleTarget = null;
            }
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
        FastMoving,
        VerticalFastMoving
    }
}
