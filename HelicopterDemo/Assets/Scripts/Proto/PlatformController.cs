using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public List<GameObject> Platforms => platforms;

    readonly string platformTag = "Platform";

    List<GameObject> platforms;

    static PlatformController instance;

    public static PlatformController GetInstance()
    {
        if (instance == null)
            instance = new PlatformController();
        return instance;
    }

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

    private PlatformController() { }

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance == this)
            Destroy(gameObject);

        platforms = new List<GameObject>();
        platforms.AddRange(GameObject.FindGameObjectsWithTag(platformTag));
    }
}
