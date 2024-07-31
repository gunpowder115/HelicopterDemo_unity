using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPC_Explorer))]
[RequireComponent(typeof(NPC_Patroller))]
[RequireComponent(typeof(NPC_MoveRelTarget))]
[RequireComponent(typeof(NPC_MoveAttack))]
[RequireComponent(typeof(CargoItem))]
[RequireComponent(typeof(Translation))]
[RequireComponent(typeof(Rotation))]

public class NPC_Mover : MonoBehaviour
{
    [SerializeField] private bool isFriendly = true;
    [SerializeField] private bool isGround = false;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lowSpeedCoef = 0.5f;
    [SerializeField] private float verticalSpeed = 20f;
    [SerializeField] private float heightDelta = 1f;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float minHeight = 15f;
    [SerializeField] private float maxHeight = 50f;
    [SerializeField] private float minPursuitDist = 40f;
    [SerializeField] private float maxPursuitDist = 50f;
    [SerializeField] private float minAttackDist = 10f;
    [SerializeField] private float maxAttackDist = 20f;

    private NpcState npcState;
    private NPC_Takeoff NPC_Takeoff;
    private NPC_Explorer NPC_Explorer;
    private NPC_Patroller NPC_Patroller;
    private NPC_MoveRelTarget NPC_MoveRelTarget;
    private NPC_MoveAttack NPC_MoveAttack;
    private CargoItem thisItem;
    private NpcController npcController;
    private GameObject selectedTarget;
    private LineRenderer lineToTarget;

    #region Properties

    public bool IsGround => isGround;
    public float Speed => speed;
    public float LowSpeed => speed * lowSpeedCoef;
    public float VerticalSpeed => verticalSpeed;
    public float HeightDelta => heightDelta;
    public float Acceleration => acceleration;
    public float MinHeight => minHeight;
    public float MaxHeight => maxHeight;
    public float MinPursuitDist => minPursuitDist;
    public float MaxPursuitDist => maxPursuitDist;
    public float MinAttackDist => minAttackDist;
    public float MaxAttackDist => minPursuitDist;
    public float HorDistToTgt
    {
        get
        {
            Vector3 toTgt = selectedTarget.transform.position - transform.position;
            toTgt.y = 0f;
            return toTgt.magnitude;
        }
    }
    public GameObject SelectedTarget => selectedTarget;
    public LineRenderer LineToTarget => lineToTarget;
    public Translation Translation { get; private set; }
    public Rotation Rotation { get; private set; }
    public Building Building => thisItem.Building;
    public Platform[] BasePlatforms => thisItem.BaseCenter.Platforms;

    #endregion

    private void Awake()
    {
        NPC_Takeoff = GetComponent<NPC_Takeoff>();
        NPC_Explorer = GetComponent<NPC_Explorer>();
        NPC_Patroller = GetComponent<NPC_Patroller>();
        NPC_MoveRelTarget = GetComponent<NPC_MoveRelTarget>();
        NPC_MoveAttack = GetComponent<NPC_MoveAttack>();
        Translation = GetComponent<Translation>();
        Rotation = GetComponent<Rotation>();
        thisItem = GetComponent<CargoItem>();

        npcController = NpcController.singleton;
        lineToTarget = gameObject.AddComponent<LineRenderer>();
        lineToTarget.enabled = false;
    }

    void Start()
    {
        npcState = isGround ? NpcState.Patrolling : NpcState.Takeoff;
    }

    void Update()
    {
        SelectTarget();
        ChangeState();
        Move();
    }

    private void Move()
    {
        EraseLine();
        switch (npcState)
        {
            case NpcState.Takeoff:
                NPC_Takeoff.Move();
                break;
            case NpcState.Patrolling:
                NPC_Patroller.Move();
                break;
            case NpcState.Exploring:
                NPC_Explorer.Move();
                break;
            case NpcState.MoveRelTarget:
                DrawLine(Color.blue);
                NPC_MoveRelTarget.Move();
                break;
            case NpcState.Attack:
                DrawLine(Color.red);
                NPC_MoveAttack.Move();
                break;
        }
    }

    private void ChangeState()
    {
        switch(npcState)
        {
            case NpcState.Takeoff:
                if (NPC_Takeoff.Check_ToPatrolling()) npcState = NpcState.Patrolling;
                break;
            case NpcState.Patrolling: //todo
                if (NPC_Patroller.Check_ToExploring()) npcState = NpcState.Exploring;
                if (NPC_Patroller.Check_ToMoveRelTarget()) npcState = NpcState.MoveRelTarget;
                break;
            case NpcState.Exploring: //todo
                if (NPC_Explorer.Check_ToPatrolling()) npcState = NpcState.Patrolling;
                if (NPC_Explorer.Check_ToMoveRelTarget()) npcState = NpcState.MoveRelTarget;
                break;
            case NpcState.MoveRelTarget: //todo
                if (NPC_MoveRelTarget.Check_ToAttack()) npcState = NpcState.Attack;
                if (NPC_MoveRelTarget.Check_ToPatrolling()) npcState = NpcState.Patrolling;
                if (NPC_MoveRelTarget.Check_ToExploring()) npcState = NpcState.Exploring;
                break;
            case NpcState.Attack:
                if (NPC_MoveAttack.Check_ToMoveRelTarget()) npcState = NpcState.MoveRelTarget;
                break;
        }
    }

    private void SelectTarget()
    {
        KeyValuePair<float, GameObject> nearest;
        if (isFriendly)
        {
            nearest = npcController.FindNearestEnemy(gameObject);
        }
        else
        {
            nearest = npcController.FindNearestFriendly(gameObject);
            var player = npcController.GetPlayer(gameObject);
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

    public enum NpcState
    {
        Takeoff,
        Exploring,
        Patrolling,
        MoveRelTarget,
        Attack,
    }
}
