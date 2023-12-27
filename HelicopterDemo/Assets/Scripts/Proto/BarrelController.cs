using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelController : MonoBehaviour
{
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateProjectile()
    {
        if (ProjectilePrefab != null)
        {
            projectileItems[lastProjectileIndex++] = Instantiate(ProjectilePrefab);
            if (lastProjectileIndex >= ProjectileCount)
                lastProjectileIndex = 0;
        }
    }
}
