using UnityEngine;

public class RotationInput : MonoBehaviour
{
    [SerializeField] float attitudeRotSpeed = 6.0f;
    [SerializeField] float yawRotSpeed = 3.0f;
    [SerializeField] float largeAttitudeAngle = 30.0f;
    [SerializeField] float smallAttitudeAngle = 20.0f;

    public float YawAngle => transform.rotation.eulerAngles.y;
    public Vector3 CurrentDirection  => new Vector3(transform.forward.x, 0f, transform.forward.z);
    public Vector3 AimAngles => new Vector3(this.transform.eulerAngles.x, this.transform.eulerAngles.y, this.transform.eulerAngles.z);

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        if (rigidBody)
            rigidBody.freezeRotation = true;
    }

    public void RotateToDirection(Vector3 targetDirection, float input, bool rotateToDirection)
    {
        Vector3 eulerAnglesCurrent = transform.rotation.eulerAngles;
        Quaternion yawRotation = Quaternion.LookRotation(targetDirection);
        float targetAngleY = rotateToDirection ? yawRotation.eulerAngles.y : eulerAnglesCurrent.y;

        targetDirection = transform.worldToLocalMatrix * targetDirection;
        float targetAttitudeAngle = input * (rotateToDirection ? largeAttitudeAngle : smallAttitudeAngle);
        float targetAngleX = targetDirection.z * targetAttitudeAngle;
        float targetAngleZ = -targetDirection.x * targetAttitudeAngle;

        Vector3 eulerAnglesTarget = new Vector3(targetAngleX, targetAngleY, targetAngleZ);
        Quaternion rotationTarget = Quaternion.Euler(eulerAnglesTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationTarget, yawRotSpeed * Time.deltaTime);
    }

    public void RotateToTarget(Quaternion targetRotation, float rollInput)
    {
        float targetAngleZ = -rollInput * largeAttitudeAngle;
        targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, targetRotation.eulerAngles.z + targetAngleZ);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, yawRotSpeed * Time.deltaTime);
    }
}
