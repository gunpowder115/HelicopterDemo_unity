using UnityEngine;

public class BarrelController : MonoBehaviour
{
    //[SerializeField] private float shotDelay = 0.5f;

    private GameObject[] projectileItems;
    private int lastProjectileIndex;
    private float currentShotDelay;

    private int _projectileCount;
    public int ProjectileCount
    {
        get => _projectileCount;
        set
        {
            _projectileCount = value;
            projectileItems = new GameObject[_projectileCount];
        }
    }

    public GameObject ProjectilePrefab { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        currentShotDelay = 0.0f;
        lastProjectileIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateProjectile()
    {
        currentShotDelay += Time.deltaTime;
        if (ProjectilePrefab != null && projectileItems[lastProjectileIndex] == null/* && currentShotDelay >= shotDelay*/)
        {
            projectileItems[lastProjectileIndex] = Instantiate(ProjectilePrefab);
            projectileItems[lastProjectileIndex].transform.position = this.transform.TransformPoint(Vector3.forward * 2.0f);
            projectileItems[lastProjectileIndex].transform.rotation = this.transform.rotation;

            lastProjectileIndex++;
            currentShotDelay = 0.0f;

            if (lastProjectileIndex >= ProjectileCount)
                lastProjectileIndex = 0;
        }
    }
}
