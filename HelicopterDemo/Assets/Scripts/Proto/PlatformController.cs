using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public static PlatformController singleton {get; private set;}
    public List<GameObject> Platforms => platforms;

    readonly string platformTag = "Platform";

    List<GameObject> platforms;

    public SortedDictionary<float, GameObject> FindDistToPlatforms(in GameObject origin)
    {
        SortedDictionary<float, GameObject> result = new SortedDictionary<float, GameObject>();

        foreach (var platform in platforms)
        {
            float distTo = Vector3.Magnitude(platform.transform.position - origin.transform.position);
            result.Add(distTo, platform);
        }
        return result;
    }

    void Awake()
    {
        singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        platforms = new List<GameObject>();
        platforms.AddRange(GameObject.FindGameObjectsWithTag(platformTag));
    }
}
