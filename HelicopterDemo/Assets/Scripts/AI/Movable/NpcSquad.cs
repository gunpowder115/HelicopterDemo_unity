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
    [SerializeField] private float dropHeight = 30f;
    [SerializeField] private float parachuneHeight = 15f;
    [SerializeField] private GameObject memberPrefab;
    [SerializeField] private GameObject parachutePrefab;

    private Vector3 squadPos;
    private Npc attackSource;
    private List<GameObject> parachutes;

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

    private Vector3 CurrentDirection => Npcs[0].Rotation.CurrentDirection;
    public override Vector3 NpcPos => squadPos;
    public override Vector3 NpcCurrDir => CurrentDirection;
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
            SetMembersTrackers();
            ChangeState();
            Move();
        }

        Debug.Log(npcState);
    }

    public void MoveSquad(Vector3 targetDir, float speed)
    {
        if (targetDir == Vector3.zero) targetDir = CurrentDirection;

        if (Npcs.Count > 1)
        {
            Vector3[] newNpcSpeed;
            if (Npcs.Count == 2)
                newNpcSpeed = CorrectSpeed_2(targetDir, speed);
            else
                newNpcSpeed = CorrectSpeed_3(targetDir, speed);

            for (int i = 0; i < Npcs.Count; i++)
            {
                Npcs[i].Translate(Npcs[i].NpcCurrDir * newNpcSpeed[i].magnitude);
                Npcs[i].Rotation.RotateByYaw(newNpcSpeed[i].normalized);
            }
        }
        else
        {
            squadPos = GetSquadPos(0);
            Npcs[0].Translate(Npcs[0].NpcCurrDir * speed);
            Npcs[0].Rotation.RotateByYaw(targetDir);
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
            nearest = npcController.FindNearestEnemy(squadPos);
        }
        else
        {
            nearest = npcController.FindNearestFriendly(squadPos);
            var player = npcController.GetPlayer(squadPos);
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
                if (Npcs[0].gameObject.transform.position.y <= transform.position.y)
                {
                    npcState = NpcState.Patrolling;
                    IsExplorer = false;
                    IsPatroller = true;
                    for (int i = 0; i < Npcs.Count; i++)
                    {
                        Npcs[i].Drop(0f);
                        Npcs[i].transform.position = new Vector3(Npcs[i].transform.position.x, transform.position.y, Npcs[i].transform.position.z);
                        Destroy(parachutes[i]);
                    }
                    parachutes.Clear();
                }
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
                foreach (var npc in Npcs)
                    npc.Drop(-deliverySpeed);
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
        parachutes = new List<GameObject>();
        Vector3 dir = new Vector3(0f, 0f, -1f);
        Quaternion rot = Quaternion.Euler(0f, 360f / membersCount, 0f);

        for (int i = 0; i < membersCount; i++)
        {
            GameObject member = Instantiate(memberPrefab, transform.position + new Vector3(0f, dropHeight, 0f), transform.rotation, transform);
            member.transform.Translate(dir * squadRadius / 2f);
            Members.Add(member);
            Npcs.Add(Members[i].GetComponent<NpcGround>());
            Npcs[i].NpcSquad = this;
            dir = rot * dir;
            npcController.Add(member);

            parachutes.Add(Instantiate(parachutePrefab, member.transform.position + new Vector3(0f, parachuneHeight, 0f), transform.rotation, member.transform));
        }
        squadPos = GetSquadPos();
    }

    private bool BehindOfSquad(NpcGround member)
    {
        float dot = Vector3.Dot(squadPos - member.gameObject.transform.position, member.CurrentSpeed);
        return dot > 0f;
    }

    private void SetMembersTrackers()
    {
        foreach (var npc in Npcs)
            npc.SetTrackersRotation();
    }

    private Vector3[] CorrectSpeed_3(Vector3 targetDir, float speed)
    {
        Vector3 targetSpeed = targetDir * speed;
        Vector3[] correctedSpeed = new Vector3[] { targetSpeed, targetSpeed, targetSpeed };

        var npc0 = Npcs[0];
        var npc1 = Npcs[1];
        var npc2 = Npcs[2];

        bool[] npcFar = new bool[3];
        npcFar[0] = npc0.FarFrom(npc0, squadRadius * 1.1f);
        npcFar[1] = npc0.FarFrom(npc1, squadRadius * 1.1f);
        npcFar[2] = npc0.FarFrom(npc2, squadRadius * 1.1f);
        bool allNpcFar = npcFar[1] && npcFar[2];

        if (allNpcFar)
            squadPos = GetSquadPos(0);
        else if (npcFar[1] || npcFar[2])
            squadPos = GetSquadPos(0, npcFar[1] ? 2 : 1);
        else
            squadPos = GetSquadPos();

        for (int i = 0; i < Npcs.Count; i++)
        {
            Vector3 newTargetSpeed = targetSpeed;
            if (allNpcFar) //two members are lagging behind first member
            {
                if (i == 0)
                    newTargetSpeed = BehindOfSquad(Npcs[0]) ? newTargetSpeed * highSpeedCoef : newTargetSpeed / highSpeedCoef;
                else if (BehindOfSquad(Npcs[i]))
                    newTargetSpeed = highSpeedCoef * speed * (squadPos - Npcs[i].gameObject.transform.position).normalized;
                else
                    newTargetSpeed /= highSpeedCoef;
            }
            else if (npcFar[i]) //one members is lagging behind first member
            {
                if (BehindOfSquad(Npcs[i]))
                    newTargetSpeed = highSpeedCoef * speed * (squadPos - Npcs[i].gameObject.transform.position).normalized;
                else
                    newTargetSpeed /= highSpeedCoef;
            }

            correctedSpeed[i] = newTargetSpeed;
        }
        return correctedSpeed;
    }

    private Vector3[] CorrectSpeed_2(Vector3 targetDir, float speed)
    {
        Vector3 targetSpeed = targetDir * speed;
        Vector3[] correctedSpeed = new Vector3[] { targetSpeed, targetSpeed };

        var npc0 = Npcs[0];
        var npc1 = Npcs[1];
        bool npcFar = npc0.FarFrom(npc1, squadRadius);

        if (npcFar)
        {
            squadPos = GetSquadPos(0);
            Vector3 newTargetSpeed = targetSpeed;

            newTargetSpeed = BehindOfSquad(npc0) ? newTargetSpeed * highSpeedCoef : newTargetSpeed / highSpeedCoef;
            correctedSpeed[0] = newTargetSpeed;

            newTargetSpeed = targetSpeed;
            if (BehindOfSquad(npc1))
                newTargetSpeed = highSpeedCoef * speed * (squadPos - npc1.gameObject.transform.position).normalized;
            else
                newTargetSpeed /= highSpeedCoef;
            correctedSpeed[1] = newTargetSpeed;
        }
        else
        {
            squadPos = GetSquadPos(0, 1);
            correctedSpeed[0] = targetSpeed;
            correctedSpeed[1] = targetSpeed;
        }

        return correctedSpeed;
    }

    private Vector3 GetSquadPos()
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

    private Vector3 GetSquadPos(int npc1, int npc2)
    {
        Vector3 pos = Vector3.zero;
        pos += Npcs[npc1].gameObject.transform.position;
        pos += Npcs[npc2].gameObject.transform.position;
        pos /= 2f;
        return pos;
    }

    private Vector3 GetSquadPos(int npc) => Npcs[npc].gameObject.transform.position;
}
