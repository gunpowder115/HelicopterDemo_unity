using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    int unguidedMissileIndex, guidedMissileIndex;
    List<BarrelLauncher> barrels;
    List<MissileLauncher> unguidedMissiles, guidedMissiles;

    public void BarrelFire(GameObject target)
    {
        if (barrels.Count > 0)
        {
            foreach(var barrel in barrels)
                if (barrel) barrel.Fire(target);
        }
    }
    public void StopBarrelFire()
    {
        if (barrels.Count > 0)
        {
            foreach (var barrel in barrels)
                if (barrel) barrel.StopFire();
        }
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
        barrels = new List<BarrelLauncher>();
        barrels.AddRange(GetComponentsInChildren<BarrelLauncher>());

        List<MissileLauncher> missiles = new List<MissileLauncher>(GetComponentsInChildren<MissileLauncher>());
        unguidedMissiles = new List<MissileLauncher>();
        guidedMissiles = new List<MissileLauncher>();
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
