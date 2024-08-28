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
            guidedMissiles[guidedMissileIndex++].Launch(target);
            if (guidedMissileIndex >= guidedMissiles.Count) guidedMissileIndex = 0;
        }
    }

    void Start()
    {
        barrels = new List<BarrelLauncher>();
        barrels.AddRange(GetComponentsInChildren<BarrelLauncher>());
        foreach (var bar in barrels)
            bar.gameObject.tag = gameObject.tag;

        List<MissileLauncher> missiles = new List<MissileLauncher>(GetComponentsInChildren<MissileLauncher>());
        unguidedMissiles = new List<MissileLauncher>();
        guidedMissiles = new List<MissileLauncher>();
        if (missiles != null)
        {
            foreach (var missile in missiles)
            {
                missile.gameObject.tag = gameObject.tag;
                if (missile.IsGuided)
                    guidedMissiles.Add(missile);
                else
                    unguidedMissiles.Add(missile);
            }
        }
    }
}
