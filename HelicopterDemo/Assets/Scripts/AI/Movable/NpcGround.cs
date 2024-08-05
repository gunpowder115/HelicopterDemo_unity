using UnityEngine;

[RequireComponent(typeof(Translation))]
[RequireComponent(typeof(Rotation))]

public class NpcGround : Npc
{
    void Start()
    {
        npcState = NpcState.Patrolling;
    }
}
