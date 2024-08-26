using UnityEngine;

[RequireComponent(typeof(Shooter))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Translation))]
[RequireComponent(typeof(Rotation))]

public class NpcGround : Npc
{
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
    public override Vector3 NpcPos => transform.position;
    public override Vector3 NpcCurrDir => rotation.CurrentDirection;
    public Npc AttackSource { get; private set; }
    public NpcSquad NpcSquad { get; set; }

    private void Awake()
    {
        base.Init();
    }

    public void SetTarget(GameObject tgt) => selectedTarget = tgt;

    public void Translate(Vector3 targetSpeed)
    {
        CurrentSpeed = Vector3.Lerp(CurrentSpeed, targetSpeed, Acceleration * Time.deltaTime);
        translation.SetGlobalTranslation(CurrentSpeed);
    }

    public override void RequestDestroy()
    {
        npcController.Remove(gameObject);
        bool isSquad = NpcSquad.RemoveMember(this);
        Destroy(gameObject);
        if (!isSquad) Destroy(NpcSquad.gameObject);
    }

    public bool FarFrom(NpcGround npc, float dist) => Vector3.Magnitude(transform.position - npc.gameObject.transform.position) > dist;
    public bool FarFrom(NpcGround npc1, NpcGround npc2, float dist) => FarFrom(npc1, dist) && FarFrom(npc2, dist);
}
