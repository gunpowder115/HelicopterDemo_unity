using UnityEngine;

public class NpcMoveToTgt : MonoBehaviour
{
    private float targetVerticalSpeed, currVerticalSpeed, targetVerticalDir;
    private Vector3 targetSpeed, currSpeed;
    private Vector3 targetDirection;
    private NpcAir NpcAir;
    private Health health;

    private bool IsGround => NpcAir.IsGround;
    private float Speed => NpcAir.Speed;
    private float HighSpeed => NpcAir.HighSpeed;
    private float VerticalSpeed => NpcAir.VerticalSpeed;
    private float HeightDelta => NpcAir.HeightDelta;
    private float Acceleration => NpcAir.Acceleration;
    private float HorDistToTgt => NpcAir.HorDistToTgt;
    private GameObject Target => NpcAir.SelectedTarget;
    private Translation Translation => NpcAir.Translation;
    private Rotation Rotation => NpcAir.Rotation;

    void Start()
    {
        NpcAir = GetComponent<NpcAir>();
        health = GetComponent<Health>();
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
        var check = HorDistToTgt > NpcAir.MaxPursuitDist;
        //if (check) NpcAir.RemoveTarget();
        return check;
    }

    public bool Check_ToAttack()
    {
        return HorDistToTgt <= NpcAir.MinAttackDist;
    }

    private void Translate()
    {
        targetSpeed = Vector3.ClampMagnitude((IsGround ? Rotation.CurrentDirection : targetDirection) * Speed, Speed);
        currSpeed = Vector3.Lerp(currSpeed, targetSpeed, Acceleration * Time.deltaTime);
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
        var direction = targetDirection != Vector3.zero ? targetDirection : Rotation.CurrentDirection;
        var speedCoef = targetDirection != Vector3.zero ? currSpeed.magnitude / Speed : 0f;

        if (IsGround)
            Rotation.RotateByYaw(direction);
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
