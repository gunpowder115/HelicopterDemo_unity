using System.Collections.Generic;
using UnityEngine;

public class NpcPatroller : MonoBehaviour
{
    [SerializeField] private float stopTime = 1f;
    [SerializeField] private float height = 30f;
    [SerializeField] private float patrolDist = 20f;

    private int currPatrolPosIndex;
    private Vector3 targetSpeed, currSpeed;
    private Vector3 targetDirection;
    private List<Vector3> patrolPositions;
    private NpcAir NpcAir;

    private bool IsGround => NpcAir.IsGround;
    private float Speed => NpcAir.LowSpeed;
    private float HeightDelta => NpcAir.HeightDelta;
    private float Acceleration => NpcAir.Acceleration;
    private float MinPursuitDist => NpcAir.MinPursuitDist;
    private float HorDistToTgt => NpcAir.HorDistToTgt;
    private Translation Translation => NpcAir.Translation;
    private Rotation Rotation => NpcAir.Rotation;

    private void Awake()
    {
        NpcAir = GetComponent<NpcAir>();
    }

    void Start()
    {
        patrolPositions = new List<Vector3>();
        currPatrolPosIndex = 0;
        foreach (var platform in NpcAir.BasePlatforms)
        {
            patrolPositions.Add(platform.gameObject.transform.position + platform.gameObject.transform.forward * patrolDist);
        }
    }

    public void Move()
    {
        SetDirection();
        Translate();
        Rotate();
    }

    public bool Check_ToExploring()
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
        targetSpeed = Vector3.ClampMagnitude((IsGround ? Rotation.CurrentDirection : targetDirection) * Speed, Speed);
        currSpeed = Vector3.Lerp(currSpeed, targetSpeed, Acceleration * Time.deltaTime);
        Translation.SetGlobalTranslation(currSpeed);
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
        Vector3 airPatrolPos = patrolPositions[currPatrolPosIndex] - transform.position;
        airPatrolPos.y = 0f;

        if (airPatrolPos.magnitude <= HeightDelta)
        {
            currPatrolPosIndex++;
            if (currPatrolPosIndex >= 8)
                currPatrolPosIndex = 0;
        }
        else
            targetDirection = airPatrolPos.normalized;
    }
}
