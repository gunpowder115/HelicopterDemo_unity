using System.Collections.Generic;
using UnityEngine;

public class NPC_Patroller : MonoBehaviour
{
    [SerializeField] private float stopTime = 1f;
    [SerializeField] private float height = 30f;
    [SerializeField] private float patrolDist = 20f;

    private int currPatrolPosIndex;
    private float currStopTime;
    private float targetVerticalSpeed, currVerticalSpeed;
    private Vector3 targetSpeed, currSpeed;
    private Vector3 targetDirection;
    private List<Vector3> patrolPositions;
    private NPC_Mover NPC_Mover;

    private bool IsGround => NPC_Mover.IsGround;
    private float Speed => NPC_Mover.LowSpeed;
    private float VerticalSpeed => NPC_Mover.VerticalSpeed;
    private float HeightDelta => NPC_Mover.HeightDelta;
    private float Acceleration => NPC_Mover.Acceleration;
    private Translation Translation => NPC_Mover.Translation;
    private Rotation Rotation => NPC_Mover.Rotation;

    private void Awake()
    {
        NPC_Mover = GetComponent<NPC_Mover>();
    }

    void Start()
    {
        patrolPositions = new List<Vector3>();
        currPatrolPosIndex = 0;
        foreach (var platform in NPC_Mover.BasePlatforms)
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
