using UnityEngine;

public class ShootingController : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float shotDelay = 0.5f;
    [SerializeField] private int maxClipVolume = 16;

    private int currentClipVolume;
    private int currentBarrelIndex;
    private float currentShotDelay;
    private bool clipStart;
    private BarrelController[] barrels;
    private bool[] barrelsLock;

    // Start is called before the first frame update
    void Start()
    {
        currentClipVolume = maxClipVolume;
        currentBarrelIndex = 0;
        currentShotDelay = 0.0f;
        clipStart = true;

        barrels = this.gameObject.GetComponentsInChildren<BarrelController>();
        if (barrels != null)
        {
            foreach (var barrel in barrels)
            {
                if (barrel != null)
                {
                    barrel.ProjectilePrefab = projectilePrefab;
                    barrel.ProjectileCount = maxClipVolume / barrels.Length;
                }
            }
            barrelsLock = new bool[barrels.Length];
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (barrels != null)
        //{
        //    if (currentClipVolume > 0)
        //    {
        //        foreach (var barrel in barrels)
        //            barrel.CreateProjectile();
        //    }
        //}

        //FireTheClip();

        BarrelShot();
    }

    private void BarrelShot()
    {
        if (currentShotDelay >= shotDelay)
        {
            barrels[currentBarrelIndex++].CreateProjectile();
            currentShotDelay = 0.0f;

            if (currentBarrelIndex >= barrels.Length)
                currentBarrelIndex = 0;
        }
        currentShotDelay += Time.deltaTime;
    }

    private void FireTheClip()
    { 
        if (barrels != null)
        {
            if (clipStart)
            {
                if (currentShotDelay >= shotDelay * currentBarrelIndex)
                    barrels[currentBarrelIndex++].CreateProjectile();
                currentShotDelay += Time.deltaTime;

                if (currentBarrelIndex >= barrels.Length)
                    clipStart = false;
            }
            else
            {
                foreach (var barrel in barrels)
                    barrel.CreateProjectile();
                clipStart = true;
            }
        }
    }

    enum ShootingStage
    {
        Shooting,
        Recharge
    }
}
