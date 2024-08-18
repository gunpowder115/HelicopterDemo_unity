using UnityEngine;

[RequireComponent(typeof(Shooter))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Translation))]
[RequireComponent(typeof(Rotation))]

public class NpcGround : Npc
{
    private NpcSquad npcSquad;

    public bool UnderAttack
    {
        get
        {
            AttackSource = health.AttackSource;
            return health.IsUnderAttack;
        }
        set => health.IsUnderAttack = value;
    }
    public bool BehindSquad { get; set; }
    public bool FarFromSquad { get; set; }
    public Vector3 CurrentSpeed { get; private set; }
    public Npc AttackSource { get; private set; }

    public void SetTarget(GameObject tgt) => selectedTarget = tgt;

    public void Translate(Vector3 targetSpeed)
    {
        CurrentSpeed = Vector3.Lerp(CurrentSpeed, targetSpeed, Acceleration * Time.deltaTime);
        translation.SetGlobalTranslation(CurrentSpeed);
    }
}
