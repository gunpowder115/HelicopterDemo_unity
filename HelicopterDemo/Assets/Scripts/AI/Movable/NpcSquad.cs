using System.Collections.Generic;
using UnityEngine;
using static Npc;

[RequireComponent(typeof(CargoItem))]

public class NpcSquad : MonoBehaviour
{
    private NpcState npcState;

    public List<NpcGround> Npcs { get; private set; }

    private void Awake()
    {
        Npcs = new List<NpcGround>();
    }

    private void Start()
    {
        npcState = NpcState.Patrolling;
    }

    private void Update()
    {
        SelectTarget();
        ChangeState();
        Move();
    }

    private void SelectTarget()
    {
        //todo
    }

    private void ChangeState()
    {
        switch (npcState)
        {
            case NpcState.Patrolling:

                break;
            case NpcState.Exploring:

                break;
            case NpcState.MoveToTarget:

                break;
            case NpcState.Attack:

                break;
        }
    }

    private void Move()
    {
        switch(npcState)
        {
            case NpcState.Patrolling:

                break;
            case NpcState.Exploring:

                break;
            case NpcState.MoveToTarget:

                break;
            case NpcState.Attack:

                break;
        }
    }
}
