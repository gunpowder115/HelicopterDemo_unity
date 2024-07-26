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
    [SerializeField] private bool isGround = false;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lowSpeedCoef = 0.5f;
    [SerializeField] private float verticalSpeed = 20f;
    [SerializeField] private float heightDelta = 1f;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float minHeight = 15f;
    [SerializeField] private float maxHeight = 50f;

    private NpcState npcState;
    private NPC_Takeoff NPC_Takeoff;
    private NPC_Explorer NPC_Explorer;
    private NPC_Patroller NPC_Patroller;
    private NPC_MoveRelTarget NPC_MoveRelTarget;
    private NPC_MoveAttack NPC_MoveAttack;
    private CargoItem thisItem;

    #region Properties

    public bool IsGround => isGround;
    public float Speed => speed;
    public float LowSpeed => speed * lowSpeedCoef;
    public float VerticalSpeed => verticalSpeed;
    public float HeightDelta => heightDelta;
    public float Acceleration => acceleration;
    public float MinHeight => minHeight;
    public float MaxHeight => maxHeight;
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
    }

    void Start()
    {
        npcState = isGround ? NpcState.Patrolling : NpcState.Takeoff;
    }

    void Update()
    {
        Move();
        ChangeState();
    }

    private void Move()
    {
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

                break;
            case NpcState.Attack:

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
        }
    }

    public enum NpcState
    {
        Takeoff,
        Exploring,
        Patrolling,
        MoveRelTarget,
        Attack,
    }
}
