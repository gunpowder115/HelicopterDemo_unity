using UnityEngine;

public class RotationInput_Proto : MonoBehaviour
{
    [SerializeField] float increaseAngleKoef = 1.0f; //dependence coefficient angle changing from input

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
}
