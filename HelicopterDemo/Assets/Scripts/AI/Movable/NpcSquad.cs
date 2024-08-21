using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NpcExplorer))]
[RequireComponent(typeof(NpcPatroller))]
[RequireComponent(typeof(NpcMoveToTgt))]
[RequireComponent(typeof(NpcAttack))]
[RequireComponent(typeof(CargoItem))]

public class NpcSquad : Npc
{
    [SerializeField] private int membersCount = 3;
    [SerializeField] private float squadRadius = 12f;
    [SerializeField] private float memberRadius = 3f;
    [SerializeField] private float deliverySpeed = 5f;
    [SerializeField] private GameObject memberPrefab;

    private Npc attackSource;

    #region Properties

    private bool IsDead //15
    {
        get
        {
            foreach (var npc in Npcs)
                if (npc.gameObject) return false;
            return true;
        }
    }
    private NpcGround MemberUnderAttack //17
    {
        get
        {
            foreach (var npc in Npcs)
                if (npc.UnderAttack)
                {
                    attackSource = npc.AttackSource;
                    return npc;
                }
            return null;
        }
        set
        {
            foreach (var npc in Npcs)
                if (npc.UnderAttack) npc.UnderAttack = value;
        }
    }

    public override float HorDistToTgt
    {
        get
        {
            if (selectedTarget)
            {
                Vector3 toTgt = selectedTarget.transform.position - SquadPos;
                toTgt.y = 0f;
                return toTgt.magnitude;
            }
            else
                return Mathf.Infinity;
        }
    }
    public Vector3 SquadPos
    {
        get
        {
            Vector3 pos = Vector3.zero;
            int count = 0;
            foreach (var npc in Npcs)
            {
                if (npc)
                {
                    pos += npc.gameObject.transform.position;
                    count++;
                }
            }
            pos /= count;
            return pos;
        }
    }
    public Vector3 CurrentDirection => Npcs[0].Rotation.CurrentDirection;
    public List<GameObject> Members { get; private set; }
    public List<NpcGround> Npcs { get; private set; }

    #endregion

    private void Awake()
    {
        base.Init();
        InitMembers();
        npcState = NpcState.Delivery;
        thisItem = GetComponent<CargoItem>();
    }

    private void Update()
    {
        if (Npcs.Count > 0)
        {
            SelectTarget();
            ChangeState();
            Move();
        }

        Debug.Log(npcState);
    }

    public void RotateSquad(Vector3 targetDir)
    {
        foreach (var npc in Npcs)
        {
            if (npc.BehindSquad && npc.FarFromSquad)
                targetDir = (SquadPos - npc.gameObject.transform.position).normalized;
            else
                targetDir = targetDir != Vector3.zero ? targetDir : CurrentDirection;
            npc.Rotation.RotateByYaw(targetDir);
        }
    }

    public void TranslateSquad(Vector3 targetSpeed)
    {
        foreach (var npc in Npcs)
        {
            npc.BehindSquad = BehindOfSquad(npc);
            npc.FarFromSquad = Vector3.Magnitude(SquadPos - npc.gameObject.transform.position) > squadRadius;

            if (npc.BehindSquad && npc.FarFromSquad)
                targetSpeed = highSpeedCoef * targetSpeed.magnitude * (SquadPos - npc.gameObject.transform.position).normalized;

            npc.Translate(targetSpeed);
        }
    }

    public override void RequestDestroy() => Destroy(gameObject);

    public bool RemoveMember(NpcGround member)
    {
        if (Npcs.Contains(member))
            Npcs.Remove(member);
        return Npcs.Count > 0;
    }

    private void SelectTarget()
    {
        KeyValuePair<float, GameObject> nearest;
        if (Npcs[0].IsFriendly)
        {
            nearest = npcController.FindNearestEnemy(SquadPos);
        }
        else
        {
            nearest = npcController.FindNearestFriendly(SquadPos);
            var player = npcController.GetPlayer(SquadPos);
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

        foreach (var npc in Npcs)
            npc.SetTarget(selectedTarget);
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
                npcState = NpcState.Exploring;
                IsExplorer = true;
                IsPatroller = false;
                //npcState = NpcState.Patrolling;
                //IsExplorer = false;
                //IsPatroller = true;
                break;
            case NpcState.Patrolling:
                if (BaseHasProtection)
                {
                    npcState = NpcState.Exploring;
                    IsExplorer = true;
                    IsPatroller = false;
                }
                else if (MemberUnderAttack != null)
                {
                    npcState = NpcState.MoveToTarget;
                    selectedTarget = attackSource.gameObject;
                    MemberUnderAttack = null;
                }
                else if (BaseUnderAttack)
                {
                    //todo
                }
                break;
            case NpcState.Exploring:
                if (EnemyForPursuit)
                    npcState = NpcState.MoveToTarget;
                else if (MemberUnderAttack != null)
                {
                    npcState = NpcState.MoveToTarget;
                    selectedTarget = attackSource.gameObject;
                    MemberUnderAttack = null;
                }
                break;
            case NpcState.MoveToTarget:
                if (EnemyForAttack)
                    npcState = NpcState.Attack;
                else if (EnemyLost && IsExplorer)
                    npcState = NpcState.Exploring;
                else if (EnemyLost && IsPatroller)
                    npcState = NpcState.Patrolling;
                break;
            case NpcState.Attack:
                if (EnemyForPursuit)
                {
                    npcState = NpcState.MoveToTarget;
                }
                else if (EnemyLost && IsExplorer)
                {
                    npcState = NpcState.Exploring;
                }
                else if (EnemyLost && IsPatroller)
                {
                    npcState = NpcState.Patrolling;
                }
                break;
        }
    }

    private void Move()
    {
        switch (npcState)
        {
            case NpcState.Delivery:

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

    private void InitMembers()
    {
        Members = new List<GameObject>();
        Npcs = new List<NpcGround>();
        Vector3 dir = new Vector3(0f, 0f, -1f);
        Quaternion rot = Quaternion.Euler(0f, 360f / membersCount, 0f);

        for (int i = 0; i < membersCount; i++)
        {
            GameObject member = Instantiate(memberPrefab, transform.position, transform.rotation);
            member.transform.Translate(dir * squadRadius / 2f);
            Members.Add(member);
            Npcs.Add(Members[i].GetComponent<NpcGround>());
            Npcs[i].NpcSquad = this;
            dir = rot * dir;
            npcController.Add(member);
        }
    }

    private bool BehindOfSquad(NpcGround member)
    {
        float dot = Vector3.Dot(SquadPos - member.gameObject.transform.position, member.CurrentSpeed);
        return dot > 0f;
    }
}
