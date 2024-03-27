using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildController_proto : MonoBehaviour
{
    public const int platformCount = 8;

    [Header("Prefabs array from N (north, X = 0, Z = 1) to clockwise")]
    [SerializeField] private GameObject[] buildPrefabs = new GameObject[platformCount];
    [SerializeField] private GameObject platformPrefab = null;
    [SerializeField] private float distToPlatform = 40f;

    private GameObject[] platforms;
    private GameObject[] builds;

    // Start is called before the first frame update
    void Start()
    {
        platforms = new GameObject[platformCount];
        builds = new GameObject[platformCount];

        float deltaAngle = 360 / platformCount;
        for (int i = 0; i < platformCount; i++)
        {
            platforms[i] = Instantiate(platformPrefab, this.gameObject.transform);
            platforms[i].transform.Rotate(0, deltaAngle * i, 0);
            platforms[i].transform.position = new Vector3();
            platforms[i].transform.Translate(0, 0, distToPlatform);
            platforms[i].transform.position += this.transform.position;
        }

        for (int i = 0; i < platformCount; i++)
        {
            if (buildPrefabs[i])
            {
                builds[i] = Instantiate(buildPrefabs[i], platforms[i].transform);
                builds[i].transform.position = platforms[i].transform.position;
                builds[i].transform.rotation = platforms[i].transform.rotation;
                builds[i].transform.localScale = new Vector3(
                    1 / platforms[i].transform.localScale.x, 
                    1 / platforms[i].transform.localScale.y,
                    1 / platforms[i].transform.localScale.z);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
