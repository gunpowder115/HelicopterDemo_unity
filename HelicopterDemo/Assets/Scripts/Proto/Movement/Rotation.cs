using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 3.0f;
    [SerializeField] float normAttitudeAngle = 30.0f;

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

    public void RotateToDirection(Vector3 targetDirection, float speedCoef, bool rotateToDirection)
    {
        Quaternion yawRotation = Quaternion.LookRotation(targetDirection);
        float targetAngleY = rotateToDirection ? yawRotation.eulerAngles.y : transform.rotation.eulerAngles.y;

        targetDirection = transform.worldToLocalMatrix * targetDirection;
        float targetAttitudeAngle = speedCoef * normAttitudeAngle;
        float targetAngleX = targetDirection.z * targetAttitudeAngle;
        float targetAngleZ = -targetDirection.x * targetAttitudeAngle;

        Vector3 eulerAnglesTarget = new Vector3(targetAngleX, targetAngleY, targetAngleZ);
        Quaternion rotationTarget = Quaternion.Euler(eulerAnglesTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationTarget, rotationSpeed * Time.deltaTime);
    }

    public void RotateToTarget(Quaternion targetRotation, float rollInput)
    {
        float targetAngleZ = -rollInput * normAttitudeAngle;
        targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, targetRotation.eulerAngles.z + targetAngleZ);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
