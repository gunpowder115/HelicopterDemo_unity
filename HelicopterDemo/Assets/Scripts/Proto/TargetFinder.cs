using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFinder : MonoBehaviour
{
    [SerializeField] private float findDeltaTime = 1f;

    private HelicopterAI helicopterAI;
    private GameObject[] groundTargets;
    private GameObject[] airTargets;
    private GameObject playerTarget;
    private GameObject selectedTarget;
    private string groundTag, airTag;
    private bool isEnemy;

    // Start is called before the first frame update
    void Start()
    {
        if (this.gameObject.tag == "EnemyGround" || this.gameObject.tag == "EnemyAir")
        {
            groundTag = "FriendlyGround";
            airTag = "FriendlyAir";
            isEnemy = true;
        }
        else
        {
            groundTag = "EnemyGround";
            airTag = "EnemyAir";
            isEnemy = false;
        }

        helicopterAI = GetComponent<HelicopterAI>();
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(SearchForTargets());
    }

    private IEnumerator SearchForTargets()
    {
        yield return new WaitForSeconds(findDeltaTime);

        groundTargets = GameObject.FindGameObjectsWithTag(groundTag);
        airTargets = GameObject.FindGameObjectsWithTag(airTag);
        playerTarget = isEnemy ? GameObject.FindGameObjectWithTag("Player") : null;

        GameObject closestGround = FindClosestObjectByHorizontal(groundTargets, out float distGround);
        GameObject closestAir = FindClosestObjectByHorizontal(airTargets, out float distAir);
        float distPlayer = playerTarget ? (playerTarget.transform.position - this.transform.position).sqrMagnitude : -1f;

        GameObject[] closestTargets = new GameObject[3] { closestGround, closestAir, playerTarget };
        float[] distTargets = new float[3] { distGround, distAir, distPlayer };

        selectedTarget = FindClosestObjectByHorizontal(closestTargets, out float resultDistance);
        if (selectedTarget != null && helicopterAI != null)
        {
            helicopterAI.FlightPhase = HelicopterAI.FlightPhases.Pursuit;
            helicopterAI.SelectedTarget = selectedTarget;
        }
    }

    private GameObject FindClosestObject(GameObject[] targets, out float distance)
    {
        distance = Mathf.Infinity;
        GameObject result = null;
        Vector3 position = this.transform.position;
        foreach(GameObject tgt in targets)
        {
            Vector3 diff = tgt.transform.position - position;
            float currDistance = diff.sqrMagnitude;
            if (currDistance < distance)
            {
                distance = currDistance;
                result = tgt;
            }
        }
        return result;
    }

    private GameObject FindClosestObjectByHorizontal(GameObject[] targets, out float distance)
    {
        distance = Mathf.Infinity;
        GameObject result = null;
        Vector3 position = this.transform.position;
        foreach (GameObject tgt in targets)
        {
            if (tgt != null)
            {
                Vector3 diff = tgt.transform.position - position;
                Vector3 diffHor = new Vector3(diff.x, 0f, diff.z);
                float currDistance = diffHor.sqrMagnitude;
                if (currDistance < distance)
                {
                    distance = currDistance;
                    result = tgt;
                }
            }
        }
        return result;
    }
}
