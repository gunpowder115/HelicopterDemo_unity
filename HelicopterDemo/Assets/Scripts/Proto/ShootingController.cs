using System.Collections;
using UnityEngine;

public class ShootingController : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float shotDelay = 0.5f;
    [SerializeField] private float rechargeTime = 5.0f;
    [SerializeField] private int maxClipVolume = 16;

    private int currentClipVolume;
    private BarrelController[] barrels;

    // Start is called before the first frame update
    void Start()
    {
        currentClipVolume = maxClipVolume;

        barrels = this.gameObject.GetComponentsInChildren<BarrelController>();
        if (barrels != null)
        {
            foreach (var barrel in barrels)
            {
                if (barrel != null)
                    barrel.ProjectilePrefab = projectilePrefab;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (barrels != null)
        {
            if (currentClipVolume > 0)
            {
                foreach (var barrel in barrels)
                    Shot(barrel);
            }
            else
                Recharge();
        }
    }

    private IEnumerator Shot(BarrelController barrel)
    {
        barrel.CreateProjectile();
        currentClipVolume--;
        yield return new WaitForSeconds(shotDelay);
    }

    private IEnumerator Recharge()
    {
        currentClipVolume = maxClipVolume;

        yield return new WaitForSeconds(rechargeTime);
    }

    enum ShootingStage
    {
        Shooting,
        Recharge
    }
}
