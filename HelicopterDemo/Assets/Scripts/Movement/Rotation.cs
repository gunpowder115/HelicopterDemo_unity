using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 3.0f;
    [SerializeField] float normAttitudeAngle = 30.0f;

    public float YawAngle => transform.rotation.eulerAngles.y;
    public Vector3 CurrentDirection  => new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
    public Vector3 AimAngles => new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0f);

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

    public void RotateByYaw(Vector3 targetDirection)
    {
        Quaternion yawRotation = Quaternion.LookRotation(targetDirection);
        float targetAngleY = yawRotation.eulerAngles.y;

        Vector3 eulerAnglesTarget = new Vector3(transform.eulerAngles.x, targetAngleY, transform.eulerAngles.z);
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
