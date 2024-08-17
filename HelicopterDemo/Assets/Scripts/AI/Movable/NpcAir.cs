using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NpcExplorer))]
[RequireComponent(typeof(NpcPatroller))]
[RequireComponent(typeof(NpcMoveToTgt))]
[RequireComponent(typeof(NpcAttack))]
[RequireComponent(typeof(Shooter))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(NpcTakeoff))]
[RequireComponent(typeof(Translation))]
[RequireComponent(typeof(Rotation))]
[RequireComponent(typeof(CargoItem))]

public class NpcAir : Npc
{
    [SerializeField] private float verticalSpeed = 20f;
    [SerializeField] private float heightDelta = 1f;
    [SerializeField] private float minHeight = 15f;
    [SerializeField] private float maxHeight = 50f;

    private NpcTakeoff npcTakeoff;
    private LineRenderer lineToTarget;

    #region Properties

    #region For change state

    private bool EndOfTakeoff => npcTakeoff.EndOfTakeoff; //2
    private bool NpcUnderAttack //17
    {
        get => health.IsUnderAttack;
        set => health.IsUnderAttack = value;
    }

    #endregion

    public float VerticalSpeed => verticalSpeed;
    public float HeightDelta => heightDelta;
    public float MinHeight => minHeight;
    public float MaxHeight => maxHeight;
    public LineRenderer LineToTarget => lineToTarget;

    #endregion

    private void Awake()
    {
        base.Init();

        npcTakeoff = GetComponent<NpcTakeoff>();
        thisItem = GetComponent<CargoItem>();

        lineToTarget = gameObject.AddComponent<LineRenderer>();
        lineToTarget.enabled = false;

        npcState = NpcState.Takeoff;
    }

    void Update()
    {
        SelectTarget();
        ChangeState();
        Move();
        Debug.Log(npcState);
    }

    public void RemoveTarget() => selectedTarget = null;

    private void Move()
    {
        EraseLine();
        switch (npcState)
        {
            case NpcState.Takeoff:
                npcTakeoff.Move();
                break;
            case NpcState.Patrolling:
                npcPatroller.Move();
                break;
            case NpcState.Exploring:
                npcExplorer.Move();
                break;
            case NpcState.MoveToTarget:
                npcMoveToTgt.Move();
                break;
            case NpcState.Attack:
                npcAttack.Move();
                npcAttack.Shoot();
                break;
        }
    }

    
    private void ChangeState()
    {
        if (!BaseHasProtection && IsExplorer)
        {
            npcState = NpcState.Patrolling;
            IsExplorer = false;
            IsPatroller = true;
        }

        switch (npcState)
        {
            case NpcState.Delivery:
                npcState = NpcState.Takeoff;
                break;
            case NpcState.Takeoff:
                if (EndOfTakeoff)
                {
                    npcState = NpcState.Patrolling;
                    IsExplorer = false;
                    IsPatroller = true;
                }
                break;
            case NpcState.Patrolling:
                if (BaseHasProtection)
                {
                    npcState = NpcState.Exploring;
                    IsExplorer = true;
                    IsPatroller = false;
                }
                else if (NpcUnderAttack || BaseUnderAttack)
                {
                    npcState = NpcState.MoveToTarget;
                    selectedTarget = health.AttackSource.gameObject;
                    NpcUnderAttack = false;
                }
                break;
            case NpcState.Exploring:
                if (EnemyForPursuit || NpcUnderAttack)
                {
                    npcState = NpcState.MoveToTarget;
                    selectedTarget = health.AttackSource.gameObject;
                    NpcUnderAttack = false;
                }
                break;
            case NpcState.MoveToTarget:
                if (EnemyForAttack)
                    npcState = NpcState.Attack;
                else if (EnemyLost && IsExplorer)
                {
                    npcState = NpcState.Exploring;
                }
                else if (EnemyLost && IsPatroller)
                {
                    npcState = NpcState.Patrolling;
                }
                break;
            case NpcState.Attack:
                if (EnemyForPursuit)
                    npcState = NpcState.MoveToTarget;
                else if (EnemyLost && IsExplorer)
                    npcState = NpcState.Exploring;
                else if (EnemyLost && IsPatroller)
                    npcState = NpcState.Patrolling;
                break;
        }
    }

    private void SelectTarget()
    {
        KeyValuePair<float, GameObject> nearest;
        if (IsFriendly)
        {
            nearest = npcController.FindNearestEnemy(transform.position);
        }
        else
        {
            nearest = npcController.FindNearestFriendly(transform.position);
            var player = npcController.GetPlayer(transform.position);
            nearest = player.Key < nearest.Key ? player : nearest;
        }

        switch (npcState)
        {
            case NpcState.Attack:
                selectedTarget = nearest.Value;
                break;
            case NpcState.MoveToTarget:
                selectedTarget = nearest.Key > MaxPursuitDist ? null : nearest.Value;
                break;
            default:
                selectedTarget = nearest.Key <= MinPursuitDist ? nearest.Value : null;
                break;

        }
    }

    private void DrawLine(Color color)
    {
        LineToTarget.enabled = true;
        LineToTarget.startColor = color;
        LineToTarget.endColor = color;
        LineToTarget.SetPosition(0, transform.position);
        LineToTarget.SetPosition(1, selectedTarget.transform.position);
    }

    private void EraseLine() => LineToTarget.enabled = false;
}
