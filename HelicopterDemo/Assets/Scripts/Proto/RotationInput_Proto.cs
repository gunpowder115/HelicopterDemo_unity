using UnityEngine;

public class RotationInput_Proto : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 1.0f;
    [SerializeField] float largeAttitudeAngle = 30.0f;
    [SerializeField] float smallAttitudeAngle = 20.0f;
    [SerializeField] float lerpYawStep = 0.05f;

    private float lerpYawValue;

    public Vector3 CurrentDirection
    {
        get
        {
            if (transform.forward.y != 0f)
                return new Vector3(transform.forward.x, 0f, transform.forward.z);
            else
                return transform.forward;
        }
    }

    public Vector3 CurrentRotation => new Vector3(transform.rotation.x,
                                                    angles[(int)InputManager_Proto.Axis_Proto.Y],
                                                    transform.rotation.z);

    private float[] angles;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        if (rigidBody)
            rigidBody.freezeRotation = true;

        angles = new float[3] { transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z };
        lerpYawValue = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(CurrentRotation);
    }

    public void RotateByYaw(float angularDistance, float input)
    {        
        if (Mathf.Abs(angularDistance) > Mathf.Epsilon)
        {
            int index = (int)InputManager_Proto.Axis_Proto.Y;
            float angle = angles[index];

            angle = Mathf.LerpAngle(angle, angle + angularDistance, lerpYawValue *  input);
            lerpYawValue += lerpYawStep;
            lerpYawValue = Mathf.Clamp01(lerpYawValue);

            angles[index] = angle;
        }
        else
        {
            lerpYawValue = 0f;
        }
    }

    public void RotateByAttitude(Vector3 targetDirection, float input, bool largeTilt)
    {
        Vector3 rotatingVector = Vector3.Cross(Vector3.up, targetDirection);
        float pitchAndRollAngle = input * (largeTilt ? largeAttitudeAngle : smallAttitudeAngle);
        transform.RotateAround(transform.position, rotatingVector, pitchAndRollAngle);
    }
}
