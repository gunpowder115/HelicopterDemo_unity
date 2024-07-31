using UnityEngine;

public class NPC_MoveRelTarget : MonoBehaviour
{
    private float targetVerticalSpeed, currVerticalSpeed, targetVerticalDir;
    private Vector3 targetSpeed, currSpeed;
    private Vector3 targetDirection;
    private NPC_Mover NPC_Mover;

    private bool IsGround => NPC_Mover.IsGround;
    private float Speed => NPC_Mover.Speed;
    private float VerticalSpeed => NPC_Mover.VerticalSpeed;
    private float HeightDelta => NPC_Mover.HeightDelta;
    private float Acceleration => NPC_Mover.Acceleration;
    private float HorDistToTgt => NPC_Mover.HorDistToTgt;
    private GameObject Target => NPC_Mover.SelectedTarget;
    private Translation Translation => NPC_Mover.Translation;
    private Rotation Rotation => NPC_Mover.Rotation;

    void Start()
    {
        NPC_Mover = GetComponent<NPC_Mover>();
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
        return HorDistToTgt > NPC_Mover.MaxPursuitDist;
    }

    public bool Check_ToAttack()
    {
        return HorDistToTgt <= NPC_Mover.MinAttackDist;
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
