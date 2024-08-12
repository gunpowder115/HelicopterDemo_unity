using UnityEngine;

public class NpcExplorer : MonoBehaviour
{
    [SerializeField] private float maxMoveTime = 10f;
    [SerializeField] private float stopTime = 1f;
    [SerializeField] private float absBorderX = 200f;
    [SerializeField] private float absBorderZ = 200f;

    private float currMoveTime, currStopTime;
    private float targetHeight, targetVerticalSpeed, currVerticalSpeed;
    private Vector3 targetSpeed, currSpeed;
    private Vector3 targetDirection;
    private Npc npc;
    private NpcAir npcAir;
    private NpcSquad npcSquad;

    private bool IsGround => npc.IsGround;
    private float Speed => npc.Speed;
    private float VerticalSpeed => npcAir.VerticalSpeed;
    private float HeightDelta => npcAir.HeightDelta;
    private float Acceleration => npc.Acceleration;
    private float MinHeight => npcAir.MinHeight;
    private float MaxHeight => npcAir.MaxHeight;
    private float MinPursuitDist => npc.MinPursuitDist;
    private float HorDistToTgt => npc.HorDistToTgt;
    private Translation Translation => npc.Translation;
    private Rotation Rotation => npc.Rotation;

    void Start()
    {
        npc = GetComponent<Npc>();
        npcAir = GetComponent<NpcAir>();
        npcSquad = GetComponent<NpcSquad>();
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
        return HorDistToTgt <= MinPursuitDist;
    }

    private void Translate()
    {
        var dir = IsGround ? npcSquad.CurrentDirection : targetDirection;

        targetSpeed = Vector3.ClampMagnitude(dir * Speed, Speed);
        currSpeed = Vector3.Lerp(currSpeed, targetSpeed, Acceleration * Time.deltaTime);

        if (IsGround)
            npcSquad.TranslateSquad(currSpeed);
        else
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
        var curDir = IsGround ? npcSquad.CurrentDirection : Rotation.CurrentDirection;
        var direction = targetDirection != Vector3.zero ? targetDirection : curDir;
        var speedCoef = targetDirection != Vector3.zero ? currSpeed.magnitude / Speed : 0f;

        if (IsGround)
            npcSquad.RotateSquad(direction);
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
