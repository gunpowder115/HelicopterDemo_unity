using UnityEngine;

[RequireComponent(typeof(Shooter))]

public class NpcAttack : MonoBehaviour
{
    [SerializeField] private float maxHorMoveTime = 3f;
    [SerializeField] private float maxVertMoveTime = 0.5f;
    [SerializeField] private float moveTime = 3f;
    [SerializeField] private float lateralMovingCoef = 0.1f;
    [SerializeField] private float targetHeightDelta = 20f;

    private bool isMoving;
    private bool horWait, vertWait;
    private float currHorTime, endHorTime, currVertTime, endVertTime, currMoveTime;
    private float targetVerticalSpeed, currVerticalSpeed, targetVerticalDir;
    private Vector3 targetSpeed, currSpeed;
    private Vector3 targetDirection;
    private Npc npc;
    private NpcAir npcAir;
    private NpcSquad npcSquad;
    private Health health;
    private Shooter shooter;

    private bool IsGround => npc.IsGround;
    private float LowSpeed => npc.LowSpeed;
    private float VerticalSpeed => npcAir.VerticalSpeed;
    private float HeightDelta => npcAir.HeightDelta;
    private float Acceleration => npc.Acceleration;
    private float MinHeight => npcAir.MinHeight;
    private float MaxHeight => npcAir.MaxHeight;
    private GameObject Target => npc.SelectedTarget;
    private Translation Translation => npc.Translation;
    private Rotation Rotation => npc.Rotation;

    void Start()
    {
        npc = GetComponent<Npc>();
        npcAir = GetComponent<NpcAir>();
        npcSquad = GetComponent<NpcSquad>();
        health = GetComponent<Health>();
        shooter = GetComponent<Shooter>();
    }

    public void Move()
    {
        if (IsGround)
        {
            TranslateGround();
            RotateGround();
        }
        else
        {
            if (health.IsHurt && !isMoving)
            {
                isMoving = true;
                health.IsHurt = false;

                float dir = Random.Range(0, 2) == 0 ? 1 : -1;
                targetDirection = new Vector3(dir, 0f, 0f);
            }

            if (isMoving)
            {
                if (currMoveTime < moveTime)
                {
                    SetHorizontalDirection();
                    currMoveTime += Time.deltaTime;
                }
                else
                {
                    targetDirection = Vector3.zero;
                    targetVerticalDir = 0f;
                    currMoveTime = 0f;
                    isMoving = false;
                }
            }

            SetVerticalDirection();
            TranslateAir();
            VerticalTranslate();
            RotateAir();
        }
    }

    public void Shoot()
    {
        shooter.BarrelFire(Target);
    }

    private void TranslateGround()
    {
        targetSpeed = Vector3.zero;
        npcSquad.TranslateSquad(targetSpeed);
    }

    private void TranslateAir()
    {
        targetDirection = BalanceDistToTarget(targetDirection);
        targetSpeed = Vector3.ClampMagnitude(targetDirection * LowSpeed, LowSpeed);
        currSpeed = Vector3.Lerp(currSpeed, targetSpeed, Acceleration * Time.deltaTime);
        Translation.SetRelToTargetTranslation(currSpeed, Rotation.YawAngle);
    }

    private void VerticalTranslate()
    {
        targetVerticalSpeed = targetVerticalDir * VerticalSpeed;
        currVerticalSpeed = Mathf.Lerp(currVerticalSpeed, targetVerticalSpeed, Acceleration * Time.deltaTime);
        Translation.SetVerticalTranslation(currVerticalSpeed);
    }

    private void RotateGround() => npcSquad.RotateSquad(targetDirection);

    private void RotateAir()
    {
        Quaternion rotToTarget = Quaternion.LookRotation((Target.transform.position - transform.position));
        Rotation.RotateToTarget(rotToTarget, targetDirection.x);
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
                else if (Mathf.Abs(Target.transform.position.y - transform.position.y) > targetHeightDelta)
                    targetVerticalDir = Mathf.Sign(Target.transform.position.y - transform.position.y);
                else if (Mathf.Abs(Target.transform.position.y - transform.position.y) > HeightDelta && isMoving)
                    targetVerticalDir = Target.transform.position.y > transform.position.y ? 1f : -1f;
                else
                    targetVerticalDir = 0f;

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
                float dir = Random.Range(0, 2) == 0 ? 1 : -1;
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
