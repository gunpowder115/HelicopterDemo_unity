using UnityEngine;

[RequireComponent(typeof(Shooter))]
[RequireComponent(typeof(Health))]

public class SimpleNpc : MonoBehaviour
{
    Shooter shooter;

    void Start()
    {
        shooter = GetComponent<Shooter>();
    }

    void Update()
    {
        shooter.BarrelFire(null);
    }
}
