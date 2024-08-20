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

        if (IsGround)
        {
            TranslateGround();
            RotateGround();
        }
        else
        {
            TranslateAir();
            VerticalTranslate();
            RotateAir();
        }
    }

    private void TranslateGround()
    {
        targetSpeed = Vector3.ClampMagnitude(npcSquad.CurrentDirection * Speed, Speed);
        npcSquad.TranslateSquad(targetSpeed);
    }

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

    private void VerticalTranslate()
    {
        targetVerticalSpeed = targetVerticalDir * VerticalSpeed;
        currVerticalSpeed = Mathf.Lerp(currVerticalSpeed, targetVerticalSpeed, Acceleration * Time.deltaTime);
        Translation.SetVerticalTranslation(currVerticalSpeed);
    }

    private void SetDirection()
    {
        if (!IsGround)
        {
            if (Mathf.Abs(Target.transform.position.y - transform.position.y) > HeightDelta)
                targetVerticalDir = Mathf.Sign(Target.transform.position.y - transform.position.y);
            else
                targetVerticalDir = 0f;
        }

        targetDirection = Target.transform.position - transform.position;
        targetDirection.y = 0f;
        targetDirection = targetDirection.normalized;
    }
}
