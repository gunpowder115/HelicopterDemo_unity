using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFinder : MonoBehaviour
{
    [SerializeField] private float findDeltaTime = 1f;
    [SerializeField] private float minFindDist = 10f;
    [SerializeField] private float maxFindDist = 20f;

    private string groundTag, airTag;
    private bool isEnemy;

    public List<GameObject> FoundTargets { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        if (this.gameObject.tag.Contains("Enemy"))
        {
            groundTag = "FriendlyGround";
            airTag = "FriendlyAir";
            isEnemy = true;
        }
        else if (this.gameObject.tag.Contains("Friendly"))
        {
            groundTag = "EnemyGround";
            airTag = "EnemyAir";
            isEnemy = false;
        }

        FoundTargets = new List<GameObject>();
    }

    void Update()
    {
        //if (!this.gameObject.tag.Contains("Neutral"))
        //    StartCoroutine(FindTargetsByHysteresisLoop());
        FindAllTargets();
    }

    private void FindAllTargets()
    {
        FoundTargets = new List<GameObject>(GameObject.FindGameObjectsWithTag(groundTag));
        FoundTargets.AddRange(GameObject.FindGameObjectsWithTag(airTag));
        GameObject playerTarget = isEnemy ? GameObject.FindGameObjectWithTag("Player") : null;
        if (playerTarget)
            FoundTargets.Add(playerTarget);
    }

    /*
     * Отслеживание целей в соотв. с петлёй гистерезиса:
     * 
     * цель отслеживается:           -----↔---------↔---
     *                              |           |
     *                              ↓           ↑
     *                              |           |
     * цель не отслеживается: --↔---------↔-----
     *                              R1          R2
     */
    private IEnumerator FindTargetsByHysteresisLoop()
    {
        yield return new WaitForSeconds(findDeltaTime);

        List<GameObject> allTargets = new List<GameObject>(GameObject.FindGameObjectsWithTag(groundTag));
        allTargets.AddRange(GameObject.FindGameObjectsWithTag(airTag));
        GameObject playerTarget = isEnemy ? GameObject.FindGameObjectWithTag("Player") : null;
        if (playerTarget)
            allTargets.Add(playerTarget);

        foreach (var tgt in allTargets)
        {
            if (!FoundTargets.Contains(tgt) && FindDistanceByHorizontalTo(tgt) < minFindDist)
            {
                FoundTargets.Add(tgt);
            }
            else if (FoundTargets.Contains(tgt) && FindDistanceByHorizontalTo(tgt) > maxFindDist)
            {
                FoundTargets.Remove(tgt);
            }
        }
    }

    private float FindDistanceByHorizontalTo(GameObject tgt)
    {
        Vector3 position = this.transform.position;
        float distance = Mathf.Infinity;
        if (tgt)
        {
            Vector3 diff = tgt.transform.position - position;
            Vector3 diffHor = new Vector3(diff.x, 0f, diff.z);
            distance = diffHor.magnitude;
        }
        return distance;
    }
}
