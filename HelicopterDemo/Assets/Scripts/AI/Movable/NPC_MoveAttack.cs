using UnityEngine;

public class NPC_MoveAttack : MonoBehaviour
{
    [SerializeField] private float maxHorMoveTime = 3f;
    [SerializeField] private float maxVertMoveTime = 0.5f;
    [SerializeField] private float lateralMovingCoef = 0.1f;

    private bool horWait, vertWait;
    private float currHorTime, endHorTime, currVertTime, endVertTime;
    private float targetVerticalSpeed, currVerticalSpeed, targetVerticalDir;
    private Vector3 targetSpeed, currSpeed;
    private Vector3 targetDirection;
    private NPC_Mover NPC_Mover;

    private bool IsGround => NPC_Mover.IsGround;
    private float LowSpeed => NPC_Mover.LowSpeed;
    private float VerticalSpeed => NPC_Mover.VerticalSpeed;
    private float HeightDelta => NPC_Mover.HeightDelta;
    private float Acceleration => NPC_Mover.Acceleration;
    private float MinHeight => NPC_Mover.MinHeight;
    private float MaxHeight => NPC_Mover.MaxHeight;
    private float HorDistToTgt => NPC_Mover.HorDistToTgt;
    private GameObject Target => NPC_Mover.SelectedTarget;
    private Translation Translation => NPC_Mover.Translation;
    private Rotation Rotation => NPC_Mover.Rotation;
    private Shooter Shooter => NPC_Mover.Shooter;

    void Start()
    {
        NPC_Mover = GetComponent<NPC_Mover>();
    }

    public void Move()
    {
        SetHorizontalDirection();
        SetVerticalDirection();

        Translate();
        if (!IsGround)
            VerticalTranslate();
        Rotate();
    }

    public void Shoot()
    {
        Shooter.BarrelFire(Target);
    }

    public bool Check_ToMoveRelTarget()
    {
        return HorDistToTgt > NPC_Mover.MaxAttackDist;
    }

    private void Translate()
    {
        if (!IsGround)
        {
            targetDirection = BalanceDistToTarget(targetDirection);
            targetSpeed = Vector3.ClampMagnitude(targetDirection * LowSpeed, LowSpeed);
            currSpeed = Vector3.Lerp(currSpeed, targetSpeed, Acceleration * Time.deltaTime);

            Translation.SetRelToTargetTranslation(currSpeed, Rotation.YawAngle);
        }
        else
        {
            //todo
        }
    }

    private void VerticalTranslate()
    {
        targetVerticalSpeed = targetVerticalDir * VerticalSpeed;
        currVerticalSpeed = Mathf.Lerp(currVerticalSpeed, targetVerticalSpeed, Acceleration * Time.deltaTime);
        Translation.SetVerticalTranslation(currVerticalSpeed);
    }

    private void Rotate()
    {
        if (!IsGround)
        {
            Quaternion rotToTarget = Quaternion.LookRotation((Target.transform.position - transform.position));
            Rotation.RotateToTarget(rotToTarget, targetDirection.x);
        }
        else
        {
            //todo
        }
    }

    private void SetVerticalDirection()
    {
        if (currVertTime > endVertTime)
        {
            if (vertWait)
            {
                if (transform.position.y > MaxHeight)
                    targetVerticalDir = -1f;
                else if (transform.position.y < MinHeight)
                    targetVerticalDir = 1f;
                else if (Mathf.Abs(Target.transform.position.y - transform.position.y) > 20f)
                    targetVerticalDir = Mathf.Sign(Target.transform.position.y - transform.position.y);
                else if (Mathf.Abs(Target.transform.position.y - transform.position.y) > HeightDelta)
                    targetVerticalDir = Target.transform.position.y > transform.position.y ? 1f : -1f;

                vertWait = false;
            }
            else
            {
                vertWait = true;
            }
            currVertTime = 0f;
            endVertTime = Random.Range(0.1f, maxVertMoveTime);
        }
        else
            currVertTime += Time.deltaTime;
    }

    private void SetHorizontalDirection()
    {
        if (currHorTime > endHorTime)
        {
            if (horWait)
            {
                float dir = Random.Range(-1f, 1f);
                targetDirection = new Vector3(dir, 0f, 0f);

                horWait = false;
            }
            else
            {
                horWait = true;
            }
            currHorTime = 0f;
            endHorTime = Random.Range(0.1f, maxHorMoveTime);
        }
        else
            currHorTime += Time.deltaTime;
    }

    private Vector3 BalanceDistToTarget(Vector3 input)
    {
        if (input.z == 0f)
            input.z = Mathf.Abs(input.x) * lateralMovingCoef;
        return input;
    }
}
