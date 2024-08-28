using System.Collections;
using UnityEngine;

public class MissileLauncher : BaseLauncher
{
    public bool IsGuided => guided;
    public bool IsEnable => isEnable;

    [SerializeField] GameObject missilePrefab;
    [SerializeField] float rechargeTime = 5f;
    [SerializeField] float shotDeltaTime = 0.5f;
    [SerializeField] bool guided = false;

    private bool isEnable;
    private float currShotDeltaTime;
    private GameObject[] childObjects;

    public void Launch(GameObject target)
    {
        this.target = target;
        if (missilePrefab)
        {
            if (guided)
            {
                var missileItem = Instantiate(missilePrefab, transform.position + transform.forward * 1.5f, transform.rotation);
                missileItem.tag = gameObject.tag;
                GuidedMissile guidedMissile = missileItem.GetComponent<GuidedMissile>();
                if (guidedMissile) guidedMissile.SelectedTarget = target;
            }
            else
            {
                var missile = Instantiate(missilePrefab, transform.position + transform.forward * 1.5f, CalculateDeflection());
                missile.tag = gameObject.tag;
            }
        }
        else
            Debug.Log(this.ToString() + ": missilePrefab is NULL!");
        StartCoroutine(MissileActivity());
        currShotDeltaTime = 0f;
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
