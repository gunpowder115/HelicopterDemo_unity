using UnityEngine;

public class BarrelController : MonoBehaviour
{
    [SerializeField] private float projectileStartCoef = 2.0f;

    private GameObject[] projectileItems;
    private int lastProjectileIndex;

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
        lastProjectileIndex = 0;
    }

    public void CreateProjectile()
    {
        if (ProjectilePrefab != null && projectileItems[lastProjectileIndex] == null)
        {
            projectileItems[lastProjectileIndex] = Instantiate(ProjectilePrefab);
            projectileItems[lastProjectileIndex].transform.position = 
                this.transform.TransformPoint(Vector3.forward * projectileStartCoef);
            projectileItems[lastProjectileIndex].transform.rotation = this.transform.rotation;

            lastProjectileIndex++;

            if (lastProjectileIndex >= ProjectileCount)
                lastProjectileIndex = 0;
        }
    }
}
