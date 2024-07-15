using UnityEngine;

[RequireComponent(typeof(Translation))]
[RequireComponent(typeof(Rotation))]

public class NPC_Explorer : MonoBehaviour
{
    [SerializeField] private float maxMoveTime = 10f;
    [SerializeField] private float stopTime = 1f;
    [SerializeField] private float absBorderX = 200f;
    [SerializeField] private float absBorderZ = 200f;

    private float currMoveTime, currStopTime;
    private Vector3 targetSpeed, currSpeed;
    private Vector3 targetDirection;
    private Translation translation;
    private Rotation rotation;
    private NPC_Mover NPC_Mover;

    private bool IsGround => NPC_Mover.IsGround;
    private float Speed => NPC_Mover.Speed;
    private float Acceleration => NPC_Mover.Acceleration;

    void Start()
    {
        translation = GetComponent<Translation>();
        rotation = GetComponent<Rotation>();
        NPC_Mover = GetComponent<NPC_Mover>();
        currMoveTime = maxMoveTime;
    }

    public void Move()
    {
        SetDirection();
        if (CheckObstacles())
            currMoveTime = maxMoveTime;

        Translate();
        Rotate();
    }

    private void Translate()
    {
        targetSpeed = Vector3.ClampMagnitude((IsGround ? rotation.CurrentDirection : targetDirection) * Speed, Speed);
        currSpeed = Vector3.Lerp(currSpeed, targetSpeed, Acceleration * Time.deltaTime);
        translation.SetGlobalTranslation(currSpeed);
    }

    private void Rotate()
    {
        var direction = targetDirection != Vector3.zero ? targetDirection : rotation.CurrentDirection;
        var speedCoef = targetDirection != Vector3.zero ? currSpeed.magnitude / Speed : 0f;

        if (IsGround)
            rotation.RotateByYaw(direction);
        else
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
                    CheckBorders();
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

    private void CheckBorders()
    {
        if (transform.position.x > absBorderX || transform.position.x < -absBorderX)
            targetDirection = new Vector3(-targetDirection.x, targetDirection.y, targetDirection.z);

        if (transform.position.z > absBorderZ || transform.position.z < -absBorderZ)
            targetDirection = new Vector3(targetDirection.x, targetDirection.y, -targetDirection.z);
    }
}
