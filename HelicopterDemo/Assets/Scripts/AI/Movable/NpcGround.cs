using UnityEngine;

[RequireComponent(typeof(Shooter))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Translation))]
[RequireComponent(typeof(Rotation))]

public class NpcGround : Npc
{
    private NpcSquad npcSquad;

    public void SetTarget(GameObject tgt) => selectedTarget = tgt;

    public void SetState(NpcState newState) => npcState = newState;

    public void Move()
    {
        
    }
}
