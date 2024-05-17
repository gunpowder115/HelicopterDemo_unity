using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    int unguidedMissileIndex, guidedMissileIndex;
    BarrelShooter barrel;
    List<MissileShooter> unguidedMissiles, guidedMissiles;

    public void BarrelFire(GameObject target)
    {
        if (barrel) barrel.Fire(target);
    }
    public void StopBarrelFire()
    {
        if (barrel) barrel.StopFire();
    }
    public void UnguidedMissileLaunch(GameObject target)
    {
        if (unguidedMissiles.Count > 0 && unguidedMissiles[unguidedMissileIndex].IsEnable)
        {
            unguidedMissiles[unguidedMissileIndex++].Launch(target);
            if (unguidedMissileIndex >= unguidedMissiles.Count) unguidedMissileIndex = 0;
        }
    }
    public void GuidedMissileLaunch(GameObject target)
    {
        if (guidedMissiles.Count > 0 && guidedMissiles[guidedMissileIndex].IsEnable)
        {
            guidedMissiles[guidedMissileIndex++].Launch(null);
            if (guidedMissileIndex >= guidedMissiles.Count) guidedMissileIndex = 0;
        }
    }

    void Start()
    {
        barrel = GetComponentInChildren<BarrelShooter>();

        List<MissileShooter> missiles = new List<MissileShooter>(GetComponentsInChildren<MissileShooter>());
        unguidedMissiles = new List<MissileShooter>();
        guidedMissiles = new List<MissileShooter>();
        if (missiles != null)
        {
            foreach (var missile in missiles)
            {
                if (missile.IsGuided)
                    guidedMissiles.Add(missile);
                else
                    unguidedMissiles.Add(missile);
            }
        }
    }
}
