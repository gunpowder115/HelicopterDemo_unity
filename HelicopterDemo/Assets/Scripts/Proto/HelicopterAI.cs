using UnityEngine;
using static InputManager;

public class HelicopterAI : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float verticalSpeed = 0.5f;
    [SerializeField] private float minHeight = 15f;
    [SerializeField] private float maxHeight = 50f;
    [SerializeField] private float minDistance = 15f;
    [SerializeField] private float maxDistance = 50f;
    [SerializeField] private float absBorderX = 200f;
    [SerializeField] private float absBorderZ = 200f;

    private TranslationInput translationInput;
    private RotationInput rotationInput;
    private FlightPhase flightPhase;
    private Vector3 translation;
    private float distance;
    private bool flight;
    private RaycastHit hit;

    private Vector3 targetInput;
    private Vector3 currentInput;
    private Vector3 targetDirection;
    private Vector3 currentDirection;
    private float angularDistance;

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
                    if (!Takeoff())
                    {
                        targetInput = GetTargetInput();
                        if (!translationInput.CurrSpeedIsHigh)
                            translationInput.ChangeSpeed();
                        flightPhase = FlightPhase.DirectFlight;
                    }
                    break;

                case FlightPhase.DirectFlight:
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
        }
    }

    public void StartFlight(Transform platformTransform)
    {
        this.gameObject.transform.position = platformTransform.position;
        this.gameObject.transform.rotation = platformTransform.rotation;
        flightPhase = FlightPhase.Takeoff;
        flight = true;
    }

    private bool Takeoff()
    {
        if (this.gameObject.transform.position.y < minHeight)
        {
            this.gameObject.transform.Translate(Vector3.up * verticalSpeed * Time.deltaTime);
            return true;
        }
        else
            return false;
    }

    private Vector3 GetTargetInput()
    {
        float inputX = Random.Range(-1f, 1f);
        float inputY = 0f;
        //float inputY = Mathf.Clamp(DeltaHeight, -1f, 1f);
        float inputZ = Random.Range(-1f, 1f);
        return new Vector3(inputX, inputY, inputZ).normalized;
    }

    private Vector3 GetCurrentInput()
    {
        float deltaX = targetInput.x - currentInput.x;
        float deltaZ = targetInput.z - currentInput.z;

        float currInX = currentInput.x + Mathf.Sign(deltaX) * Time.deltaTime / 0.33f;
        float currInZ = currentInput.z + Mathf.Sign(deltaZ) * Time.deltaTime / 0.33f;
        currInX = Mathf.Clamp(currInX, 
                                targetInput.x > 0f ? -targetInput.x : targetInput.x, 
                                targetInput.x > 0f ? targetInput.x : -targetInput.x);
        currInZ = Mathf.Clamp(currInZ, 
                                targetInput.z > 0f ? -targetInput.z : targetInput.z, 
                                targetInput.z > 0f ? targetInput.z : -targetInput.z);

        currentInput = new Vector3(currInX, currentInput.y, currInZ);
        return currentInput;
    }

    public enum FlightPhase
    {
        Takeoff,
        DirectFlight,
    }
}
