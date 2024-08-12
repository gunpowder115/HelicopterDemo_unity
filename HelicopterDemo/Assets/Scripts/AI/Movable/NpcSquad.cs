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
    [SerializeField] private float squadRadius = 20f;
    [SerializeField] private float memberRadius = 3f;
    [SerializeField] private float deliverySpeed = 5f;
    [SerializeField] private GameObject memberPrefab;

    private bool BaseHasProtection => BaseCenter.HasPrimaryProtection || (BaseCenter.HasSecondaryProtection && BaseCenter.Protection != thisItem); //4
    private bool BaseUnderAttack => BaseCenter.IsUnderAttack; //6
    private bool EnemyForAttack => HorDistToSquadPos <= MinAttackDist; //7
    private bool EnemyForPursuit => HorDistToSquadPos > MaxAttackDist; //8
    private bool EnemyLost => selectedTarget == null;
    private bool IsExplorer { get; set; } //10
    private bool IsPatroller { get; set; } //11
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
                if (npc.UnderAttack) return npc;
            return null;
        }
    }

    public float HorDistToSquadPos
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

    private void Awake()
    {
        Members = new List<GameObject>();
        Npcs = new List<NpcGround>();

        npcState = NpcState.Delivery;
    }

    private void Update()
    {
        if (Npcs.Count > 0)
        {
            SelectTarget();
            ChangeState();
            Move();
        }
    }

    public void RotateSquad(Vector3 dir)
    {
        foreach (var npc in Npcs)
            npc.Rotation.RotateByYaw(dir);
    }

    public void TranslateSquad(Vector3 speed)
    {
        foreach (var npc in Npcs)
            npc.Translation.SetGlobalTranslation(speed);
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
        var selectedTarget = nearest.Value;

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
                npcState = NpcState.Patrolling;
                IsExplorer = false;
                IsPatroller = true;
                break;
            case NpcState.Patrolling:
                if (BaseHasProtection)
                {
                    npcState = NpcState.Exploring;
                    IsExplorer = true;
                    IsPatroller = false;
                }
                else if (BaseUnderAttack || MemberUnderAttack != null)
                {
                    npcState = NpcState.MoveToTarget;
                }
                break;
            case NpcState.Exploring:
                if (EnemyForPursuit || MemberUnderAttack != null)
                {
                    npcState = NpcState.MoveToTarget;
                }
                break;
            case NpcState.MoveToTarget:
                if (EnemyForAttack)
                {
                    npcState = NpcState.Attack;
                }
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
        switch(npcState)
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
            GameObject member = Instantiate(memberPrefab, transform);
            member.transform.Translate(dir * squadRadius / 2f);
            Members.Add(member);
            Npcs.Add(Members[i].GetComponent<NpcGround>());
            dir = rot * dir;
        }
    }
}
