using UnityEngine;

[RequireComponent(typeof(Translation))]
[RequireComponent(typeof(Rotation))]

public class NPC_Explorer : MonoBehaviour
{
    [SerializeField] private float maxMoveTime = 10f;
    [SerializeField] private float stopTime = 1f;

    private float currMoveTime, currStopTime;
    private Vector3 targetSpeed, currSpeed;
    private Vector3 targetDirection;
    private Translation translation;
    private Rotation rotation;

    void Start()
    {
        translation = GetComponent<Translation>();
        rotation = GetComponent<Rotation>();
        currMoveTime = maxMoveTime;
    }

    public void Move(float speed, float accel)
    {
        SetDirection();
        if (CheckObstacles())
            currMoveTime = maxMoveTime;

        Translate(speed, accel);
        Rotate(speed);
    }

    private void Translate(float speed, float accel)
    {
        targetSpeed = Vector3.ClampMagnitude(targetDirection * speed, speed);
        currSpeed = Vector3.Lerp(currSpeed, targetSpeed, accel * Time.deltaTime);
        translation.SetGlobalTranslation(currSpeed);
    }

    private void Rotate(float speed)
    {
        var direction = targetDirection != Vector3.zero ? targetDirection : rotation.CurrentDirection;
        var speedCoef = targetDirection != Vector3.zero ? currSpeed.magnitude / speed : 0f;
        rotation.RotateToDirection(direction, speedCoef, true);
    }

    private void SetDirection()
    {
        if (currMoveTime >= maxMoveTime)
        {
            if (Wait())
                targetDirection = Vector2.zero;
            else
            {
                do
                {
                    targetDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
                } while (CheckObstacles());
                currMoveTime = 0f;
            }
        }
        else
            currMoveTime += Time.deltaTime;
    }

    private bool Wait()
    {
        if (currStopTime >= stopTime)
        {
            currStopTime = 0f;
            return false;
        }
        else
            currStopTime += Time.deltaTime;
        return true;
    }

    private bool CheckObstacles()
    {
        Ray ray = new Ray(gameObject.transform.position, targetDirection);
        if (Physics.SphereCast(ray, 5.0f, out RaycastHit hit))
        {
            GameObject hitObject = hit.transform.gameObject;
            return hitObject.CompareTag("Obstacle") && hit.distance < 20.0f;
        }
        return false;
    }
}
