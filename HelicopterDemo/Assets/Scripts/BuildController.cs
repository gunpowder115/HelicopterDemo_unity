using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildController : MonoBehaviour
{
    [SerializeField] int minBuildCount = 2;
    [SerializeField] int maxBuildCount = 5;
    [SerializeField] int platformCount = 8;
    [SerializeField] float minDistance = 50;
    [SerializeField] float maxDistance = 80;
    [Header("Build Prefabs")]
    [SerializeField] GameObject platformPrefab;
    [SerializeField] GameObject buildPrefab1;
    [SerializeField] GameObject buildPrefab2;

    private GameObject[] builds;
    private GameObject[] platforms;
    private List<GameObject> prefabs;
    private List<int> unusedPlatforms;

    // Start is called before the first frame update
    void Start()
    {
        int buildCount = Random.Range(minBuildCount, maxBuildCount + 1);
        builds = new GameObject[buildCount];
        platforms = new GameObject[platformCount];
        unusedPlatforms = new List<int>();
        for (int i = 0; i < platformCount; i++)
            unusedPlatforms.Add(i);

        float deltaAngle = 360 / platformCount;
        int prefabCount = AddPrefabs();

        for (int i = 0; i < platformCount; i++)
        {
            platforms[i] = Instantiate(platformPrefab);

            platforms[i].transform.Rotate(0, deltaAngle * i + this.transform.position.y, 0);

            float platformDistance = Random.Range(minDistance, maxDistance);
            platforms[i].transform.position = new Vector3();
            platforms[i].transform.Translate(platformDistance, 0, 0);
            platforms[i].transform.position += this.transform.position;
        }

        for (int i = 0; i < buildCount; i++)
        {
            builds[i] = Instantiate(prefabs[Random.Range(0, prefabCount)]);

            int unusedPlatformIndex = Random.Range(0, unusedPlatforms.Count);
            int platformIndex = unusedPlatforms[unusedPlatformIndex];            
            builds[i].transform.position = platforms[platformIndex].transform.position;
            builds[i].transform.rotation = platforms[platformIndex].transform.rotation;
            unusedPlatforms.Remove(platformIndex);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private int AddPrefabs()
    {
        prefabs = new List<GameObject>();
        prefabs.Add(buildPrefab1);
        prefabs.Add(buildPrefab2);
        return prefabs.Count;
    }
}
