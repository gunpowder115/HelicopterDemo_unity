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
    public List<GameObject> Members { get; private set; }
    public List<NpcGround> Npcs { get; private set; }

    private void Awake()
    {
        Members = new List<GameObject>();
        Npcs = new List<NpcGround>();
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
        switch (npcState)
        {
            case NpcState.Delivery:

                break;
            case NpcState.Patrolling:

                break;
            case NpcState.Exploring:

                break;
            case NpcState.MoveToTarget:

                break;
            case NpcState.Attack:

                break;
        }
        SetMembersState(npcState);
    }

    private void Move()
    {
        switch(npcState)
        {
            case NpcState.Delivery:

                break;
            case NpcState.Patrolling:

                break;
            case NpcState.Exploring:

                break;
            case NpcState.MoveToTarget:

                break;
            case NpcState.Attack:

                break;
        }
    }

    private void SetMembersState(NpcState newState)
    {
        foreach (var npc in Npcs)
            npc.SetState(newState);
    }

    private void MembersMove()
    {
        foreach (var npc in Npcs)
            npc.Move();
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
