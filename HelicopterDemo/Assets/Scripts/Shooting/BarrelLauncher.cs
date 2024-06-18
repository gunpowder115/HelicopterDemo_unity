using UnityEngine;

public class BarrelLauncher : BaseLauncher
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float shotDeltaTime = 0.5f;
    [SerializeField] float rechargeTime = 5f;
    [SerializeField] int maxClipVolume = 1;

    bool firstShot;
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

    public void StopFire() => Stop();

    void Start()
    {
        firstShot = true;
        currClipVolume = maxClipVolume;
    }

    void Shoot()
    {
        if (currShotDeltaTime >= shotDeltaTime)
        {
            if (projectilePrefab)
                Instantiate(projectilePrefab, this.transform.position, CalculateDeflection());
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
            firstShot = true;
            currRechargeTime = 0f;
        }
        else
            currRechargeTime += Time.deltaTime;
    }

    void Stop() => firstShot = true;
}
