using UnityEngine;

public class NpcMoveToTgt : MonoBehaviour
{
    private float targetVerticalSpeed, currVerticalSpeed, targetVerticalDir;
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
    private float HorDistToTgt => npc.HorDistToTgt;
    private GameObject Target => npc.SelectedTarget;
    private Translation Translation => npc.Translation;
    private Rotation Rotation => npc.Rotation;

    void Start()
    {
        npc = GetComponent<Npc>();
        npcAir = GetComponent<NpcAir>();
        npcSquad = GetComponent<NpcSquad>();
    }

    public void Move()
    {
        SetDirection();
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

    public bool Check_ToExploring()
    {
        var check = HorDistToTgt > npc.MaxPursuitDist;
        //if (check) NpcAir.RemoveTarget();
        return check;
    }

    public bool Check_ToAttack()
    {
        return HorDistToTgt <= npc.MinAttackDist;
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
        targetVerticalSpeed = targetVerticalDir * VerticalSpeed;
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
        if (Mathf.Abs(Target.transform.position.y - transform.position.y) > HeightDelta)
            targetVerticalDir = Mathf.Sign(Target.transform.position.y - transform.position.y);
        else
            targetVerticalDir = 0f;

        targetDirection = Target.transform.position - transform.position;
        targetDirection.y = 0f;
        targetDirection = targetDirection.normalized;
    }
}
