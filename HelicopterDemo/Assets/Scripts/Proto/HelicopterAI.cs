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

    private TranslationInput translationInput;
    private RotationInput rotationInput;

    private FlightPhase flightPhase;
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
        hit = new RaycastHit();
    }

    // Update is called once per frame
    void Update()
    {
        if (flight)
        {
            switch (flightPhase)
            {
                case FlightPhase.Takeoff:
                    if (this.gameObject.transform.position.y > minHeight)
                    {
                        targetInput = GetTargetInput();
                        if (!translationInput.CurrSpeedIsHigh)
                            translationInput.ChangeSpeed();
                        flightPhase = FlightPhase.Patrolling;
                    }
                    break;

                default:
                    if ((targetInput.y > 0f && this.gameObject.transform.position.y >= targetHeight) ||
                        (targetInput.y < 0f && this.gameObject.transform.position.y <= targetHeight))
                    {
                        targetInput = new Vector3(targetInput.x, 0f, targetInput.z);
                    }

                    Ray ray = new Ray(this.gameObject.transform.position, this.gameObject.transform.forward);
                    if (Physics.SphereCast(ray, 5.0f, out hit))
                    {
                        GameObject hitObject = hit.transform.gameObject;
                        if (hitObject.CompareTag("Obstacle") && hit.distance < 20.0f)
                        {
                            targetInput = GetTargetInput();
                        }
                    }
                    break;
            }

            Movement();
        }
    }

    public void StartFlight()
    {
        flightPhase = FlightPhase.Takeoff;
        targetInput = new Vector3(0f, patrolVerticalSpeed, 0f);
        flight = true;
    }

    private Vector3 GetTargetInput()
    {
        float inputX = Random.Range(-1f, 1f);
        float inputZ = Random.Range(-1f, 1f);

        Vector3 result = new Vector3(inputX, 0f, inputZ).normalized;

        float speedScale = normalSpeed;
        if (flightPhase == FlightPhase.Patrolling || flightPhase == FlightPhase.Takeoff)
            speedScale = patrolSpeed;

        result.Scale(new Vector3(speedScale, speedScale, speedScale));

        targetHeight = Random.Range(minHeight, maxHeight);
        float deltaHeight = targetHeight - this.gameObject.transform.position.y;

        float verticalSpeedScale = normalVerticalSpeed;
        if (flightPhase == FlightPhase.Patrolling || flightPhase == FlightPhase.Takeoff)
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

    public enum FlightPhase
    {
        Takeoff,
        Patrolling,
        Pursuit,
        Attack,
    }
}
