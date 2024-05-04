using System.Collections;
using UnityEngine;

public class BarrelShooter : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float shotDeltaTime = 0.5f;
    [SerializeField] float firstShotDelay = 0f;
    [SerializeField] float maxDeflectionAngle = 15f;
    [SerializeField] float rechargeTime = 5f;
    [SerializeField] int maxClipVolume = 1;

    bool firstShot;
    int currClipVolume;
    float currShotDeltaTime, currRechargeTime;

    public void Fire()
    {
        if (firstShot)
            FirstShot();
        else if (currClipVolume >= 0f)
            Shoot();
        else
            Recharge();
    }

    public void StopFire() => Recharge();

    // Start is called before the first frame update
    void Start()
    {
        firstShot = true;
        currClipVolume = maxClipVolume;
    }

    void FirstShot()
    {
        if (currShotDeltaTime >= firstShotDelay)
        {
            currShotDeltaTime = 0f;
            firstShot = false;
        }
        else
            currShotDeltaTime += Time.deltaTime;
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

    Quaternion CalculateDeflection()
    {
        float deflectionHor = Random.Range(-maxDeflectionAngle, maxDeflectionAngle);
        float deflectionVert = Random.Range(-maxDeflectionAngle, maxDeflectionAngle);

        Vector3 euler = this.transform.eulerAngles;
        euler = new Vector3(euler.x + deflectionHor, euler.y + deflectionVert, euler.z);
        return Quaternion.Euler(euler);
    }
}
