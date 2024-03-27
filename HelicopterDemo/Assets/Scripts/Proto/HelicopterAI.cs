using UnityEngine;
using static InputManager;

public class HelicopterAI : MonoBehaviour
{
    [SerializeField] private float normalSpeed = 1f;
    [SerializeField] private float patrolSpeed = 0.7f;
    [SerializeField] private float patrolVerticalSpeed = 0.3f;
    [SerializeField] private float normalVerticalSpeed = 0.5f;
    [SerializeField] private float minHeight = 15f;
    [SerializeField] private float maxHeight = 50f;
    [SerializeField] private float minDistance = 15f;
    [SerializeField] private float maxDistance = 50f;
    [SerializeField] private float absBorderX = 200f;
    [SerializeField] private float absBorderZ = 200f;
    [SerializeField] private float minPursuitDistance = 40f;
    [SerializeField] private float maxPursuitDistance = 50f;
    [SerializeField] private float minAttackDistance = 10f;
    [SerializeField] private float maxAttackDistance = 20f;

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

    private bool flight; 
    private RaycastHit hit;
    private Vector3 targetInput;
    private Vector3 currentInput;
    private Vector3 targetDirection;
    private Vector3 currentDirection;
    private float angularDistance;
    private float targetHeight;

    private void Start()
    {
        translationInput = GetComponentInChildren<TranslationInput>();
        rotationInput = GetComponentInChildren<RotationInput>();
        targetFinder = GetComponent<TargetFinder>();
        targetSelector = GetComponent<TargetSelector>();
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
                    if (this.gameObject.tag.Contains("Enemy"))
                        Debug.Log("Takeoff");
                    break;

                case FlightPhases.Patrolling:
                    ClampVerticalMovement();
                    CheckObstacles();
                    if (targetFinder && targetSelector)
                    {
                        SelectedTarget = targetSelector.SelectTarget(targetFinder.FoundTargets);
                        if (SelectedTarget)
                        {
                            if (DistanceToTarget < minPursuitDistance)
                            {
                                FlightPhase = FlightPhases.Pursuit;
                            }
                        }
                    }                    
                    if (this.gameObject.tag.Contains("Enemy"))
                        Debug.Log("Patrolling");
                    break;

                case FlightPhases.Pursuit:
                    ClampVerticalMovement();
                    CheckObstacles();
                    TargetPursuit();
                    if (DistanceToTarget < minAttackDistance)
                    {
                        FlightPhase = FlightPhases.Attack;
                        targetInput = new Vector3(0f, 0f, 0f);
                    }
                    else if (DistanceToTarget > maxPursuitDistance)
                    {
                        FlightPhase = FlightPhases.Patrolling;
                    }                    
                    if (this.gameObject.tag.Contains("Enemy"))
                        Debug.Log("Pursuit");
                    break;

                case FlightPhases.Attack:
                    ClampVerticalMovement();
                    CheckObstacles();
                    if (DistanceToTarget > maxAttackDistance)
                    {
                        FlightPhase = FlightPhases.Pursuit;
                    }
                    if (this.gameObject.tag.Contains("Enemy"))
                        Debug.Log("Attack");
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

    private void TargetPursuit()
    {
        Vector3 toTarget = SelectedTarget.transform.position - this.gameObject.transform.position;
        targetInput = toTarget.normalized;
    }

    private Vector3 GetTargetInput()
    {
        float inputX = Random.Range(-1f, 1f);
        float inputZ = Random.Range(-1f, 1f);

        Vector3 result = new Vector3(inputX, 0f, inputZ).normalized;

        float speedScale = normalSpeed;
        if (FlightPhase == FlightPhases.Patrolling || FlightPhase == FlightPhases.Takeoff)
            speedScale = patrolSpeed;

        result.Scale(new Vector3(speedScale, speedScale, speedScale));

        if (FlightPhase == FlightPhases.Pursuit || FlightPhase == FlightPhases.Attack)
            targetHeight = SelectedTarget.transform.position.y;
        else
            targetHeight = Random.Range(minHeight, maxHeight);
        float deltaHeight = targetHeight - this.gameObject.transform.position.y;

        float verticalSpeedScale = normalVerticalSpeed;
        if (FlightPhase == FlightPhases.Patrolling || FlightPhase == FlightPhases.Takeoff)
            verticalSpeedScale = patrolVerticalSpeed;

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
            angularDistance = translationInput.GetAngularDistance(currentDirection);
            targetDirection = translationInput.TargetDirection;

            translationInput.Translate(Axis_Proto.X, currentInput.x);
            translationInput.Translate(Axis_Proto.Y, currentInput.y);
            translationInput.Translate(Axis_Proto.Z, currentInput.z);
        }

        if (rotationInput != null)
        {
            currentDirection = rotationInput.CurrentDirection;

            rotationInput.RotateByYaw(angularDistance, true);
            rotationInput.RotateByAttitude(targetDirection, inputXZ, true);
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

    public enum FlightPhases
    {
        Takeoff,
        Patrolling,
        Pursuit,
        Attack,
        Leaving,
    }
}
