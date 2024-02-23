using UnityEngine;

public class RotationInput_Proto : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 1.0f;
    [SerializeField] float largeAttitudeAngle = 30.0f;
    [SerializeField] float smallAttitudeAngle = 20.0f;

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

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        if (rigidBody)
            rigidBody.freezeRotation = true;
    }

    public void Rotate(Vector3 targetDirection, float angularDistance, float input, bool rotateToDirection)
    {
        targetDirection = transform.worldToLocalMatrix * targetDirection;
        Vector3 eulerAnglesCurrent = transform.rotation.eulerAngles;
        float targetAngle = input * (rotateToDirection ? largeAttitudeAngle : smallAttitudeAngle);
        float targetAngleX = targetDirection.z * targetAngle;
        float targetAngleZ = -targetDirection.x * targetAngle;
        float targetAngleY = rotateToDirection ? eulerAnglesCurrent.y + angularDistance : eulerAnglesCurrent.y;

        Vector3 eulerAnglesTarget = new Vector3(targetAngleX, targetAngleY, targetAngleZ);

        Quaternion rotationTarget = Quaternion.Euler(eulerAnglesTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationTarget, rotationSpeed * Time.deltaTime);
    }
}
