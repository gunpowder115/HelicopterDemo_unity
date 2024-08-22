using System.Collections.Generic;
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
    [SerializeField] protected float distDelta = 1f;

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
    protected CargoItem thisItem;
    protected NpcController npcController;
    protected List<TargetTracker> trackers;

    #region Properties

    #region For change state

    protected bool BaseHasProtection => BaseCenter.HasPrimaryProtection || (BaseCenter.HasSecondaryProtection && BaseCenter.Protection != thisItem); //4
    protected bool BaseUnderAttack => BaseCenter.IsUnderAttack; //6
    protected bool EnemyForAttack => HorDistToTgt <= MinAttackDist; //7
    protected bool EnemyForPursuit => npcState == NpcState.Attack ?
        HorDistToTgt > MaxAttackDist : HorDistToTgt <= MinPursuitDist; //8
    protected bool EnemyLost => selectedTarget == null; //9
    protected bool IsExplorer { get; set; } //10
    protected bool IsPatroller { get; set; } //11

    #endregion

    public bool IsFriendly
    {
        get => isFriendly;
        set => isFriendly = value;
    }
    public bool IsGround => isGround;
    public float Speed => speed;
    public float LowSpeed => speed * lowSpeedCoef;
    public float HighSpeed => speed * highSpeedCoef;
    public float Acceleration => acceleration;
    public float MinPursuitDist => minPursuitDist;
    public float MaxPursuitDist => maxPursuitDist;
    public float MinAttackDist => minAttackDist;
    public float MaxAttackDist => minPursuitDist;
    public float DistDelta => distDelta;
    public float HorDistToTgt
    {
        get
        {
            if (selectedTarget)
            {
                Vector3 toTgt = selectedTarget.transform.position - NpcPos;
                toTgt.y = 0f;
                return toTgt.magnitude;
            }
            else
                return Mathf.Infinity;
        }
    }
    public abstract Vector3 NpcPos { get; }
    public abstract Vector3 NpcCurrDir { get; }
    public GameObject SelectedTarget => selectedTarget;
    public Translation Translation => translation;
    public Rotation Rotation => rotation;
    public Building Building => thisItem.Building;
    public BaseCenter BaseCenter => thisItem.BaseCenter;
    public Platform[] BasePlatforms => thisItem.BaseCenter.Platforms;

    #endregion

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

        trackers = new List<TargetTracker>();
        trackers.AddRange(gameObject.GetComponentsInChildren<TargetTracker>());
    }

    public void SetTrackersRotation()
    {
        foreach (var track in trackers)
            track.SetRotation(selectedTarget, NpcCurrDir);
    }

    public abstract void RequestDestroy();

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
