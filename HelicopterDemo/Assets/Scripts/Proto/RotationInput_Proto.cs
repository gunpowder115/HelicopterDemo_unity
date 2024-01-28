using UnityEngine;

public class RotationInput_Proto : MonoBehaviour
{
    [SerializeField] float increaseAngleKoef = 1.0f; //dependence coefficient angle changing from input
    [SerializeField] float yawRotationSpeed = 1.0f;

    public Vector3 CurrentDirection { get => transform.forward; }

    private float[] angles;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        if (rigidBody)
            rigidBody.freezeRotation = true;

        angles = new float[3] { transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z };
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotateVector = new Vector3(angles[(int)InputManager_Proto.Axis_Proto.X],
                                            angles[(int)InputManager_Proto.Axis_Proto.Y],
                                            angles[(int)InputManager_Proto.Axis_Proto.Z]);
        transform.localEulerAngles = rotateVector;
    }

    public void RotateNoLimits(InputManager_Proto.Axis_Proto axis, float input)
    {
        int index = (int)axis;

        float deltaAngle = input * increaseAngleKoef;
        float angle = angles[index];

        angle += deltaAngle;

        angles[index] = angle;
    }

    public void DecreaseAngularDistance(InputManager_Proto.Axis_Proto axis, float input, float angularDistance)
    {
        if (Mathf.Abs(angularDistance) > 1f)
        {
            int index = (int)axis;

            float deltaAngle = input * increaseAngleKoef;
            float angle = angles[index];

            angle += Mathf.Sign(angularDistance);

            angles[index] = angle;
        }
    }

    public void RotateToAngle(InputManager_Proto.Axis_Proto axis, float input, float targetAngle)
    {
        if (Mathf.Abs(targetAngle) > 0.01f)
        {
            int index = (int)axis;

            float deltaAngle = input * increaseAngleKoef;
            float angle = angles[index];

            if (targetAngle < 0f)
            {
                angle -= deltaAngle;
            }
            else if (targetAngle > 0f)
            {
                angle += deltaAngle;
            }

            angles[index] = angle;
        }
    }

    public void RotateToDirection(InputManager_Proto.Axis_Proto axis, Vector3 targetDirection)
    {        
        if (Vector3.Angle(CurrentDirection, targetDirection) == 180.0f)
            targetDirection = new Vector3(targetDirection.x + targetDirection.magnitude * 0.01f, 0.0f, targetDirection.z);

        transform.rotation = Quaternion.FromToRotation(CurrentDirection, targetDirection);
    }
}
