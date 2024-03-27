using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelector : MonoBehaviour
{
    [SerializeField] private float findDeltaTime = 1f;

    private float distToSelectedTarget;
    private GameObject selectedTarget;

    public GameObject SelectTarget(List<GameObject> foundTargets)
    {
        if (foundTargets != null)
        {
            if (foundTargets.Count > 0)
            {
                StartCoroutine(SelectClosestTarget(foundTargets));
                return selectedTarget;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    private IEnumerator SelectClosestTarget(List<GameObject> foundTargets)
    {
        yield return new WaitForSeconds(findDeltaTime);

        if (foundTargets != null)
            selectedTarget = FindClosestObjectByHorizontal(foundTargets, out distToSelectedTarget);
    }

    private GameObject FindClosestObjectByHorizontal(List<GameObject> targets, out float distance)
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
