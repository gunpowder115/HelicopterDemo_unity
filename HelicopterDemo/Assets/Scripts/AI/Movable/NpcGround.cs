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
    public Npc AttackSource { get; private set; }

    public void SetTarget(GameObject tgt) => selectedTarget = tgt;
}
