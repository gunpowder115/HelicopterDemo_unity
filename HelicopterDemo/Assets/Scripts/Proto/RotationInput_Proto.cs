using UnityEngine;

public class RotationInput_Proto : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 1.0f;

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
    private float pitchAndRollAngle;
    private Vector3 rotatingVector;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        if (rigidBody)
            rigidBody.freezeRotation = true;

        angles = new float[3] { transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z };
        pitchAndRollAngle = 30f;
        rotatingVector = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(CurrentRotation);
    }

    public void DecreaseAngularDistance(InputManager_Proto.Axis_Proto axis, float input, float angularDistance)
    {
        if (Mathf.Abs(angularDistance) > 0f)
        {
            int index = (int)axis;
            float angle = angles[index];

            angle = Mathf.LerpAngle(angle, angle + angularDistance, Time.deltaTime * rotationSpeed);

            angles[index] = angle;
        }
    }

    public void Rotate(Vector3 targetDirection)
    {
        if (targetDirection != Vector3.zero)
        {
            rotatingVector = Vector3.Cross(Vector3.up, targetDirection);
            transform.RotateAround(transform.position, rotatingVector, pitchAndRollAngle);
            pitchAndRollAngle++;
        }
        else
        {
            transform.RotateAround(transform.position, rotatingVector, pitchAndRollAngle);
            pitchAndRollAngle--;
        }
        pitchAndRollAngle = Mathf.Clamp(pitchAndRollAngle, 0f, 30f);
    }
}
