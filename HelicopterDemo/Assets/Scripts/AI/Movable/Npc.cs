using UnityEngine;

public abstract class Npc : MonoBehaviour
{
    [SerializeField] protected bool isFriendly = true;
    [SerializeField] protected bool isGround = false;
    [SerializeField] protected float speed = 20f;
    [SerializeField] protected float lowSpeedCoef = 0.5f;
    [SerializeField] protected float highSpeedCoef = 1.5f;
    [SerializeField] protected float acceleration = 1f;
    [SerializeField] protected float minPursuitDist = 40f;
    [SerializeField] protected float maxPursuitDist = 50f;
    [SerializeField] protected float minAttackDist = 10f;
    [SerializeField] protected float maxAttackDist = 20f;

    protected NpcState npcState;
    protected NpcExplorer npcExplorer;
    protected NpcPatroller npcPatroller;
    protected NpcMoveToTgt npcMoveToTgt;
    protected NpcAttack npcAttack;
    protected Translation translation;
    protected Rotation rotation;
    protected Shooter shooter;
    protected Health health;
    protected GameObject selectedTarget;
    protected NpcController npcController;

    protected void Init()
    {
        npcExplorer = GetComponent<NpcExplorer>();
        npcPatroller = GetComponent<NpcPatroller>();
        npcMoveToTgt = GetComponent<NpcMoveToTgt>();
        npcAttack = GetComponent<NpcAttack>();
        translation = GetComponent<Translation>();
        rotation = GetComponent<Rotation>();
        shooter = GetComponent<Shooter>();
        health = GetComponent<Health>();
        npcController = NpcController.singleton;
    }

    public bool IsFriendly
    {
        get => isFriendly;
        set => isFriendly = value;
    }

    public enum NpcState
    {
        Idle,
        Delivery,
        Takeoff,
        Exploring,
        Patrolling,
        MoveToTarget,
        Attack,
    }
}
