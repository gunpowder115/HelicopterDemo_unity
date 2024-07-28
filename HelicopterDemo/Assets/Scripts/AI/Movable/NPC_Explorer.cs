using UnityEngine;

public class NPC_Explorer : MonoBehaviour
{
    [SerializeField] private float maxMoveTime = 10f;
    [SerializeField] private float stopTime = 1f;
    [SerializeField] private float absBorderX = 200f;
    [SerializeField] private float absBorderZ = 200f;

    private float currMoveTime, currStopTime;
    private float targetHeight, targetVerticalSpeed, currVerticalSpeed;
    private Vector3 targetSpeed, currSpeed;
    private Vector3 targetDirection;
    private NPC_Mover NPC_Mover;

    private bool IsGround => NPC_Mover.IsGround;
    private float Speed => NPC_Mover.Speed;
    private float VerticalSpeed => NPC_Mover.VerticalSpeed;
    private float HeightDelta => NPC_Mover.HeightDelta;
    private float Acceleration => NPC_Mover.Acceleration;
    private float MinHeight => NPC_Mover.MinHeight;
    private float MaxHeight => NPC_Mover.MaxHeight;
    private Translation Translation => NPC_Mover.Translation;
    private Rotation Rotation => NPC_Mover.Rotation;

    void Start()
    {
        NPC_Mover = GetComponent<NPC_Mover>();
        currMoveTime = maxMoveTime;
    }

    public void Move()
    {
        SetDirection();
        if (CheckObstacles())
            currMoveTime = maxMoveTime;

        Translate();
        if (!IsGround)
            VerticalTranslate();
        Rotate();
    }

    public bool Check_ToPatrolling()
    {
        //todo
        return false;
    }

    public bool Check_ToMoveRelTarget()
    {
        //todo
        return false;
    }

    private void Translate()
    {
        targetSpeed = Vector3.ClampMagnitude((IsGround ? Rotation.CurrentDirection : targetDirection) * Speed, Speed);
        currSpeed = Vector3.Lerp(currSpeed, targetSpeed, Acceleration * Time.deltaTime);
        Translation.SetGlobalTranslation(currSpeed);
    }

    private void VerticalTranslate()
    {
        float vertDir = 0f;
        if (Mathf.Abs(targetHeight - transform.position.y) > HeightDelta)
            vertDir = targetHeight > transform.position.y ? 1f : -1f;
        targetVerticalSpeed = vertDir * VerticalSpeed;
        currVerticalSpeed = Mathf.Lerp(currVerticalSpeed, targetVerticalSpeed, Acceleration * Time.deltaTime);
        Translation.SetVerticalTranslation(currVerticalSpeed);
    }

    private void Rotate()
    {
        var direction = targetDirection != Vector3.zero ? targetDirection : Rotation.CurrentDirection;
        var speedCoef = targetDirection != Vector3.zero ? currSpeed.magnitude / Speed : 0f;

        if (IsGround)
            Rotation.RotateByYaw(direction);
        else
            Rotation.RotateToDirection(direction, speedCoef, true);
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
                    targetHeight = Random.Range(MinHeight, MaxHeight);
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
