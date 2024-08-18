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
    private NpcAir npcAir;
    private NpcSquad npcSquad;
    private Npc npc;

    private bool IsGround => npc.IsGround;
    private float Speed => npc.LowSpeed;
    private float HeightDelta => npcAir.HeightDelta;
    private float Acceleration => npc.Acceleration;
    private Translation Translation => npc.Translation;
    private Rotation Rotation => npc.Rotation;

    private void Awake()
    {
        npc = GetComponent<Npc>();
        npcAir = GetComponent<NpcAir>();
        npcSquad = GetComponent<NpcSquad>();
    }

    void Start()
    {
        patrolPositions = new List<Vector3>();
        currPatrolPosIndex = 0;
        foreach (var platform in npc.BasePlatforms)
        {
            patrolPositions.Add(platform.gameObject.transform.position + platform.gameObject.transform.forward * patrolDist);
        }
    }

    public void Move()
    {
        SetDirection();
        if (IsGround)
        {
            TranslateGround();
            RotateGround();
        }
        else
        {
            TranslateAir();
            RotateAir();
        }
    }

    private void TranslateGround() => npcSquad.TranslateSquad();

    private void TranslateAir()
    {
        targetSpeed = Vector3.ClampMagnitude(targetDirection * Speed, Speed);
        currSpeed = Vector3.Lerp(currSpeed, targetSpeed, Acceleration * Time.deltaTime);
        Translation.SetGlobalTranslation(currSpeed);
    }

    private void RotateGround() => npcSquad.RotateSquad(targetDirection);

    private void RotateAir()
    {
        var direction = targetDirection != Vector3.zero ? targetDirection : Rotation.CurrentDirection;
        var speedCoef = targetDirection != Vector3.zero ? currSpeed.magnitude / Speed : 0f;
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
