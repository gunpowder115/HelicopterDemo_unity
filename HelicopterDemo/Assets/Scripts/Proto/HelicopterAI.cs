using UnityEngine;
using static InputManager;

public class HelicopterAI : MonoBehaviour
{
    [Header("Normal (high) and patrol (low) speed")]
    [SerializeField] private float normalSpeed = 1f;
    [SerializeField] private float patrolSpeed = 0.7f;
    [SerializeField] private float patrolVerticalSpeed = 0.3f;
    [SerializeField] private float normalVerticalSpeed = 0.5f;
    [Header("Borders")]
    [SerializeField] private float minHeight = 15f;
    [SerializeField] private float maxHeight = 50f;
    [SerializeField] private float minDistance = 15f;
    [SerializeField] private float maxDistance = 50f;
    [SerializeField] private float absBorderX = 200f;
    [SerializeField] private float absBorderZ = 200f;
    [Header("For pursuit target")]
    [SerializeField] private float minPursuitDistance = 40f;
    [SerializeField] private float maxPursuitDistance = 50f;
    [SerializeField] private float minAttackDistance = 10f;
    [SerializeField] private float maxAttackDistance = 20f;
    [SerializeField] private bool opportToChangeTarget = true;

    public GameObject SelectedTarget { get; set; }
    private float DistanceToTarget
    {
        get
        {
            if (SelectedTarget)
            {
                Vector3 diff = SelectedTarget.transform.position - this.gameObject.transform.position;
                Vector3 diffHor = new Vector3(diff.x, 0f, diff.z);
                return diffHor.magnitude;
            }
            else
                return Mathf.Infinity;
        }
    }
    private Vector3 DirectionToTarget
    {
        get
        {
            if (SelectedTarget)
            {
                Vector3 dirToTarget = SelectedTarget.transform.position - this.gameObject.transform.position;
                Vector3 dirToTargetHor = new Vector3(dirToTarget.x, 0f, dirToTarget.z).normalized;
                return dirToTargetHor;
            }
            else
                return Vector3.zero;
        }
    }

    private FlightPhases flightPhase;
    public FlightPhases FlightPhase
    {
        get => flightPhase;
        set => flightPhase = value;
    }

    private TranslationInput translationInput;
    private RotationInput rotationInput;
    private TargetFinder targetFinder;
    private TargetSelector targetSelector;
    private ShootingController shootingController;

    private bool flight;
    private RaycastHit hit;
    private Vector3 targetInput;
    private Vector3 currentInput;
    private Vector3 targetDirection;
    private Vector3 currentDirection;
    private float targetHeight;

    private void Start()
    {
        translationInput = GetComponentInChildren<TranslationInput>();
        rotationInput = GetComponentInChildren<RotationInput>();
        targetFinder = GetComponent<TargetFinder>();
        targetSelector = GetComponent<TargetSelector>();
        shootingController = GetComponentInChildren<ShootingController>();
        hit = new RaycastHit();
    }

    // Update is called once per frame
    void Update()
    {
        if (flight)
        {
            switch (FlightPhase)
            {
                case FlightPhases.Takeoff:
                    if (this.gameObject.transform.position.y > minHeight)
                    {
                        targetInput = GetTargetInput();
                        if (!translationInput.CurrSpeedIsHigh)
                            translationInput.ChangeSpeed();
                        FlightPhase = FlightPhases.Patrolling;
                    }
                    break;

                case FlightPhases.Patrolling:
                    ClampVerticalMovement();
                    CheckObstacles();
                    SelectNewTarget();
                    if (DistanceToTarget < minPursuitDistance)
                        FlightPhase = FlightPhases.Pursuit;
                    break;

                case FlightPhases.Pursuit:
                    targetInput = GetTargetInput();
                    SelectNewTarget();
                    if (DistanceToTarget < minAttackDistance)
                        FlightPhase = FlightPhases.Attack;
                    else if (DistanceToTarget > maxPursuitDistance)
                        FlightPhase = FlightPhases.Patrolling;
                    break;

                case FlightPhases.Attack:
                    targetInput = GetTargetInput();
                    SelectNewTarget();
                    AttackSelectedTarget();
                    if (DistanceToTarget > maxAttackDistance)
                        FlightPhase = FlightPhases.Pursuit;
                    break;

                default:
                    break;
            }
            Movement();
        }
    }

    public void StartFlight()
    {
        FlightPhase = FlightPhases.Takeoff;
        targetInput = new Vector3(0f, patrolVerticalSpeed, 0f);
        flight = true;
    }

    private Vector3 GetTargetInput()
    {
        //get raw target input
        float inputX;
        float inputZ;
        float speedScale = normalSpeed;
        float verticalSpeedScale = normalVerticalSpeed;
        switch (FlightPhase)
        {
            case FlightPhases.Pursuit:
                inputX = DirectionToTarget.x;
                inputZ = DirectionToTarget.z;
                targetHeight = SelectedTarget.transform.position.y;
                break;

            case FlightPhases.Attack:
                inputX = 0f;
                inputZ = 0f;
                targetHeight = SelectedTarget.transform.position.y;
                break;

            case FlightPhases.Takeoff:
            case FlightPhases.Patrolling:
            default:
                inputX = Random.Range(-1f, 1f);
                inputZ = Random.Range(-1f, 1f);
                targetHeight = Random.Range(minHeight, maxHeight);
                speedScale = patrolSpeed;
                verticalSpeedScale = patrolVerticalSpeed;
                break;
        }
        Vector3 result = new Vector3(inputX, 0f, inputZ).normalized;

        result.Scale(new Vector3(speedScale, speedScale, speedScale));

        float deltaHeight = targetHeight - this.gameObject.transform.position.y;
        float inputY = Mathf.Clamp(Mathf.Abs(deltaHeight), -verticalSpeedScale, verticalSpeedScale);
        inputY *= Mathf.Sign(deltaHeight);

        result = new Vector3(result.x, inputY, result.z);
        return result;
    }

    private Vector3 GetCurrentInput()
    {
        currentInput = new Vector3(GetCurrentInputByAxis(targetInput.x, currentInput.x),
                                    GetCurrentInputByAxis(targetInput.y, currentInput.y),
                                    GetCurrentInputByAxis(targetInput.z, currentInput.z));
        return currentInput;
    }

    private float GetCurrentInputByAxis(float tgtIn, float currIn)
    {
        float deltaIn = tgtIn - currIn;
        float newCurrIn = currIn + Mathf.Sign(deltaIn) * Time.deltaTime / 0.33f;
        return Mathf.Clamp(newCurrIn,
                            tgtIn > 0f ? -tgtIn : tgtIn,
                            tgtIn > 0f ? tgtIn : -tgtIn);
    }

    private void Movement()
    {
        currentInput = GetCurrentInput();
        float inputXZ = Mathf.Clamp01(new Vector3(currentInput.x, 0f, currentInput.z).magnitude);

        if (translationInput != null)
        {
            targetDirection = translationInput.TargetDirection;
            translationInput.TranslateGlobal(new Vector3(currentInput.x, currentInput.y, currentInput.z));
        }

        if (rotationInput != null)
        {
            currentDirection = rotationInput.CurrentDirection;
            rotationInput.RotateToDirection(targetDirection, inputXZ, true);
        }
    }

    private void ClampVerticalMovement()
    {
        if ((targetInput.y > 0f && this.gameObject.transform.position.y >= targetHeight) ||
            (targetInput.y < 0f && this.gameObject.transform.position.y <= targetHeight))
        {
            targetInput = new Vector3(targetInput.x, 0f, targetInput.z);
        }
    }

    private void CheckObstacles()
    {
        Ray ray = new Ray(this.gameObject.transform.position, new Vector3(targetInput.x, 0f, targetInput.z));
        if (Physics.SphereCast(ray, 5.0f, out hit))
        {
            GameObject hitObject = hit.transform.gameObject;
            if (hitObject.CompareTag("Obstacle") && hit.distance < 20.0f)
            {
                targetInput = GetTargetInput();
            }
        }
    }

    private void SelectNewTarget()
    {
        if (targetFinder && targetSelector)
        {
            GameObject newTarget = targetSelector.SelectTarget(targetFinder.FoundTargets);
            if (newTarget)
            {
                if (!SelectedTarget)
                    SelectedTarget = newTarget;
                if (opportToChangeTarget)
                {
                    Vector3 toNewTarget = newTarget.transform.position - this.gameObject.transform.position;
                    toNewTarget = new Vector3(toNewTarget.x, 0f, toNewTarget.z);
                    float distToNewTarget = toNewTarget.magnitude;
                    if (distToNewTarget < DistanceToTarget)
                        SelectedTarget = newTarget;
                }
            }
        }
    }

    private void AttackSelectedTarget()
    {
        shootingController?.BarrelShot();
    }

    public enum FlightPhases
    {
        Takeoff,
        Patrolling,
        Pursuit,
        Attack,
        Leaving,
    }
}
