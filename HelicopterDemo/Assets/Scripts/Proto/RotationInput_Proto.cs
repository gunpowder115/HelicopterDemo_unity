using UnityEngine;

public class RotationInput_Proto : MonoBehaviour
{
    [SerializeField] float increaseAngleKoef = 1.0f; //dependence coefficient angle changing from input
    [SerializeField] float yawRotationSpeed = 1.0f;

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

    public void RotateToAngle(InputManager_Proto.Axis_Proto axis, float input, float targetAngle)
    {
        int index = (int)axis;

        float deltaAngle = input * increaseAngleKoef;
        float angle = angles[index];

        if (angle > targetAngle)
        {
            angle -= deltaAngle;
        }
        else if (angle < targetAngle)
        {
            angle += deltaAngle;
        }

        angles[index] = angle;
    }

    public void RotateToDirection(InputManager_Proto.Axis_Proto axis, Vector3 targetDirection)
    {
        if (Vector3.Angle(Vector3.forward, targetDirection) == 180.0f)
            targetDirection = new Vector3(targetDirection.x + targetDirection.magnitude * 0.01f, 0.0f, targetDirection.z);

        transform.rotation = Quaternion.FromToRotation(Vector3.forward, targetDirection);
    }
}
