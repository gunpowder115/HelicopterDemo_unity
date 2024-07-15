using UnityEngine;

[RequireComponent(typeof(NPC_Explorer))]
[RequireComponent(typeof(NPC_Patroller))]
[RequireComponent(typeof(NPC_MoveRelTarget))]
[RequireComponent(typeof(NPC_MoveAttack))]

public class NPC_Mover : MonoBehaviour
{
    [SerializeField] private bool isGround = false;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float acceleration = 1f;

    private NpcState npcState;
    private NPC_Explorer NPC_Explorer;
    private NPC_Patroller NPC_Patroller;
    private NPC_MoveRelTarget NPC_MoveRelTarget;
    private NPC_MoveAttack NPC_MoveAttack;

    public bool IsGround => isGround;
    public float Speed => speed;
    public float Acceleration => acceleration;

    void Start()
    {
        npcState = NpcState.Exploring;
        NPC_Explorer = GetComponent<NPC_Explorer>();
        NPC_Patroller = GetComponent<NPC_Patroller>();
        NPC_MoveRelTarget = GetComponent<NPC_MoveRelTarget>();
        NPC_MoveAttack = GetComponent<NPC_MoveAttack>();
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
            case NpcState.Patrolling:

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
        //todo
    }

    public enum NpcState
    {
        Exploring,
        Patrolling,
        MoveRelTarget,
        Attack,
    }
}
