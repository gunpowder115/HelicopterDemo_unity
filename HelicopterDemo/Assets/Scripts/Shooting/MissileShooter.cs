using System.Collections;
using UnityEngine;

public class MissileShooter : MonoBehaviour
{
    public bool IsGuided => guided;
    public bool IsEnable => isEnable;

    [SerializeField] GameObject missilePrefab;
    [SerializeField] float rechargeTime = 5f;
    [SerializeField] bool guided = false;

    bool isEnable;
    GameObject[] childObjects;

    public void Launch(GameObject target)
    {
        if (missilePrefab)
            Instantiate(missilePrefab, transform.position + transform.forward * 1.5f, CalculateRotToTarget(target));
        else
            Debug.Log(this.ToString() + ": missilePrefab is NULL!");
        StartCoroutine(MissileActivity());
    }

    void Start()
    {
        isEnable = true;
        childObjects = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            childObjects[i] = transform.GetChild(i).gameObject;
    }

    IEnumerator MissileActivity()
    {
        foreach (var obj in childObjects)
            obj.SetActive(false);
        isEnable = false;

        yield return new WaitForSeconds(rechargeTime);

        foreach (var obj in childObjects)
            obj.SetActive(true);
        isEnable = true;
    }

    Quaternion CalculateRotToTarget(GameObject target)
    {
        if (target && !guided)
            return Quaternion.LookRotation(target.transform.position - transform.position);
        else
            return transform.rotation;
    }
}
