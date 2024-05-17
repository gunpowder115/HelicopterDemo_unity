using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    #region Properties

    public static NpcController singleton { get; private set; }
    public List<GameObject> NPCs => npcs;
    public int EnemyAirCount => GetNpcCount(enemyAirTag);
    public int FriendlyAirCount => GetNpcCount(friendlyAirTag);
    public int EnemyGroundCount => GetNpcCount(enemyGroundTag);
    public int FriendlyGroundCount => GetNpcCount(friendlyGroundTag);
    public int EnemyBuildCount => GetNpcCount(enemyBuildTag);
    public int FriendlyBuildCount => GetNpcCount(friendlyBuildTag);

    #endregion

    #region Readonly

    readonly string enemyAirTag = "EnemyAir";
    readonly string friendlyAirTag = "FriendlyAir";
    readonly string enemyGroundTag = "EnemyGround";
    readonly string friendlyGroundTag = "FriendlyGround";
    readonly string enemyBuildTag = "EnemyBuild";
    readonly string friendlyBuildTag = "FriendlyBuild";

    #endregion

    List<GameObject> npcs;

    static NpcController instance;

    public void Add(GameObject npc)
    {
        if (!npcs.Contains(npc))
            npcs.Add(npc);
    }

    public void Remove(GameObject npc)
    {
        if (npcs.Contains(npc))
            npcs.Remove(npc);
    }

    public SortedDictionary<float, GameObject> FindDistToEnemies(in GameObject origin) => FindDistToNpcs(in origin, true);
    public SortedDictionary<float, GameObject> FindDistToFriendlies(in GameObject origin) => FindDistToNpcs(in origin, false);
    public KeyValuePair<float, GameObject> FindNearestEnemy(in GameObject origin) => FindNearestNpc(in origin, FindDistToEnemies(in origin));
    public KeyValuePair<float, GameObject> FindNearestFriendly(in GameObject origin) => FindNearestNpc(in origin, FindDistToFriendlies(in origin));

    void Awake()
    {
        singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance == this)
            Destroy(gameObject);

        npcs = new List<GameObject>();
        npcs.AddRange(GameObject.FindGameObjectsWithTag(enemyAirTag));
        npcs.AddRange(GameObject.FindGameObjectsWithTag(friendlyAirTag));
        npcs.AddRange(GameObject.FindGameObjectsWithTag(enemyGroundTag));
        npcs.AddRange(GameObject.FindGameObjectsWithTag(friendlyGroundTag));
        npcs.AddRange(GameObject.FindGameObjectsWithTag(enemyBuildTag));
        npcs.AddRange(GameObject.FindGameObjectsWithTag(friendlyBuildTag));
    }

    int GetNpcCount(string tag)
    {
        int count = 0;
        foreach (var npc in npcs)
            if (npc.CompareTag(tag)) count++;
        return count;
    }

    SortedDictionary<float, GameObject> FindDistToNpcs(in GameObject origin, bool findEnemies = true)
    {
        string selAirTag = findEnemies ? enemyAirTag : friendlyAirTag;
        string selGroundTag = findEnemies ? enemyGroundTag : friendlyGroundTag;
        string selBuildTag = findEnemies ? enemyBuildTag : friendlyBuildTag;
        SortedDictionary<float, GameObject> result = new SortedDictionary<float, GameObject>();

        foreach (var npc in npcs)
        {
            if (npc.CompareTag(selAirTag) || npc.CompareTag(selGroundTag) || npc.CompareTag(selBuildTag))
            {
                float distTo = Vector3.Magnitude(npc.transform.position - origin.transform.position);
                result.Add(distTo, npc);
            }
        }
        return result;
    }

    KeyValuePair<float, GameObject> FindNearestNpc(in GameObject origin, SortedDictionary<float, GameObject> npcs)
    {
        bool areNpcs = npcs.Count > 0;
        KeyValuePair<float, GameObject> nearestNpc = areNpcs ? npcs.ElementAt(0) : new KeyValuePair<float, GameObject>(Mathf.Infinity, null);
        return nearestNpc;
    }
}
