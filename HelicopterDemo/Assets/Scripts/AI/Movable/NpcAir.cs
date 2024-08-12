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

    //private bool EndOfTakeoff => NpcTakeoff.EndOfTakeoff;
    //private bool BaseHasProtection => thisItem.BaseCenter.HasProtection;
    //private bool enemyDetected => ;
    //private bool npcUnderAttack => ;
    //private bool baseUnderAttack => ;
    //private bool enemyForAttack => ;
    //private bool enemyForPursuit => ;
    //private bool enemyLost => ;
    //private bool isExplorer { get; set; }
    //private bool isPatroller { get; set; }
    //private bool isLowHealth => ;
    //private bool isNormHealth => ;
    //private bool enemyIsFar => ;

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

    
    private void ChangeState() //todo
    {
        //if (!BaseHasProtection && isExplorer)
        //{
        //    npcState = NpcState.Patrolling;
        //    isExplorer = false;
        //    isPatroller = true;
        //}
        //else if (isExplorer && isLowHealth)
        //    npcState = NpcState.MoveFromTarget;

        //switch (npcState)
        //{
        //    case NpcState.Takeoff:
        //        if (EndOfTakeoff) npcState = NpcState.Patrolling;
        //        break;

        //    case NpcState.Patrolling:
        //        if (BaseHasProtection)
        //        {
        //            npcState = NpcState.Exploring;
        //            isExplorer = true;
        //            isPatroller = false;
        //        }
        //        else if (enemyFinded || npcUnderAttack || baseUnderAttack)
        //            npcState = NpcState.MoveToTarget;
        //        break;

        //    case NpcState.Exploring:
        //        if (enemyFinded || npcUnderAttack)
        //            npcState = NpcState.MoveToTarget;
        //        break;

        //    case NpcState.MoveToTarget:
        //        if (enemyForAttack)
        //            npcState = NpcState.Attack;
        //        break;

        //    case NpcState.Attack:
        //        if (enemyForPursuit)
        //            npcState = NpcState.MoveToTarget;
        //        else if (enemyLost && isExplorer)
        //            npcState = NpcState.Exploring;
        //        else if (enemyLost && isPatroller)
        //            npcState = NpcState.Patrolling;
        //        break;

        //    case NpcState.MoveFromTarget:
        //        if (isNormHealth || enemyIsFar)
        //        {
        //            npcState = NpcState.Exploring;
        //            isExplorer = true;
        //            isPatroller = false;
        //        }
        //        break;
        //}

        switch (npcState)
        {
            case NpcState.Takeoff:
                if (npcTakeoff.EndOfTakeoff) npcState = NpcState.Patrolling;
                break;
            case NpcState.Patrolling:
                if (npcPatroller.Check_ToExploring()) npcState = NpcState.Exploring;
                if (npcPatroller.Check_ToMoveRelTarget()) npcState = NpcState.MoveToTarget;
                break;
            case NpcState.Exploring:
                if (npcExplorer.Check_ToPatrolling()) npcState = NpcState.Patrolling;
                if (npcExplorer.Check_ToMoveRelTarget()) npcState = NpcState.MoveToTarget;
                break;
            case NpcState.MoveToTarget:
                if (npcMoveToTgt.Check_ToAttack()) npcState = NpcState.Attack;
                if (npcMoveToTgt.Check_ToPatrolling()) npcState = NpcState.Patrolling;
                if (npcMoveToTgt.Check_ToExploring()) npcState = NpcState.Exploring;
                break;
            case NpcState.Attack:
                if (npcAttack.Check_ToMoveToTarget()) npcState = NpcState.MoveToTarget;
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
        selectedTarget = nearest.Value;
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
