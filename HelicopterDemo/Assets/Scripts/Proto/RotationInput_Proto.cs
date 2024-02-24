using UnityEngine;

public class RotationInput_Proto : MonoBehaviour
{
    [SerializeField] float attitudeRotSpeed = 6.0f;
    [SerializeField] float yawRotSpeed = 3.0f;
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

    public void RotateByYaw(float angularDistance, bool rotateToDirection)
    {
        Vector3 eulerAnglesCurrent = transform.rotation.eulerAngles;

        float targetAngleY = rotateToDirection ? eulerAnglesCurrent.y + angularDistance : eulerAnglesCurrent.y;

        Vector3 eulerAnglesTarget = new Vector3(eulerAnglesCurrent.x, targetAngleY, eulerAnglesCurrent.z);

        Quaternion rotationTarget = Quaternion.Euler(eulerAnglesTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationTarget, yawRotSpeed * Time.deltaTime);
    }

    public void RotateByAttitude(Vector3 targetDirection, float input, bool rotateToDirection)
    {
        targetDirection = transform.worldToLocalMatrix * targetDirection;
        Vector3 eulerAnglesCurrent = transform.rotation.eulerAngles;

        float targetAttitudeAngle = input * (rotateToDirection ? largeAttitudeAngle : smallAttitudeAngle);
        float targetAngleX = targetDirection.z * targetAttitudeAngle;
        float targetAngleZ = -targetDirection.x * targetAttitudeAngle;

        Vector3 eulerAnglesTarget = new Vector3(targetAngleX, eulerAnglesCurrent.y, targetAngleZ);

        Quaternion rotationTarget = Quaternion.Euler(eulerAnglesTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationTarget, attitudeRotSpeed * Time.deltaTime);
    }
}
