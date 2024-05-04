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

    public void Launch()
    {
        if (missilePrefab)
            Instantiate(missilePrefab, this.transform.position, this.transform.rotation);
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
}
