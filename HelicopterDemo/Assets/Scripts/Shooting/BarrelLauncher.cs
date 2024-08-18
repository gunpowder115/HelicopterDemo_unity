using UnityEngine;

public class BarrelLauncher : BaseLauncher
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float shotDeltaTime = 0.5f;
    [SerializeField] float rechargeTime = 5f;
    [SerializeField] int maxClipVolume = 1;

    int currClipVolume;
    float currShotDeltaTime, currRechargeTime;

    public void Fire(GameObject target)
    {
        this.target = target;
        if (currClipVolume >= 0f)
            Shoot();
        else
            Recharge();
    }

    void Start()
    {
        currClipVolume = maxClipVolume;
    }

    void Shoot()
    {
        if (currShotDeltaTime >= shotDeltaTime)
        {
            if (projectilePrefab)
            {
                var proj = Instantiate(projectilePrefab, this.transform.position, CalculateDeflection());
                proj.tag = gameObject.tag;
            }
            else
                Debug.Log(this.ToString() + ": projectilePrefab is NULL!");

            if (maxClipVolume > 0f) currClipVolume--;
            currShotDeltaTime = 0f;
        }
        else
            currShotDeltaTime += Time.deltaTime;
    }

    void Recharge()
    {
        if (currRechargeTime >= rechargeTime)
        {
            currClipVolume = maxClipVolume;
            currRechargeTime = 0f;
        }
        else
            currRechargeTime += Time.deltaTime;
    }
}
