using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildController_proto : MonoBehaviour
{
    public const int platformCount = 8;

    [Header("Prefabs array from N (north, X = 0, Z = 1) to clockwise")]
    [SerializeField] private GameObject[] buildPrefabs = new GameObject[platformCount];
    [SerializeField] private float distToPlatform = 40f;

    private GameObject[] platforms;
    private GameObject[] builds;

    // Start is called before the first frame update
    void Start()
    {
        builds = new GameObject[platformCount];

        platforms = new GameObject[platformCount];
        var platformComponents = GetComponentsInChildren<Platform>();
        if (platformComponents.Length != platformCount)
            Debug.LogError(this.ToString() + ": platformComponents.Length = " + platforms.Length);
        for (int i = 0; i < platformComponents.Length; i++)
        {
            platforms[i] = platformComponents[i].gameObject;
            Vector3 toPlatform = (platforms[i].transform.position - transform.position).normalized * distToPlatform;
            Vector3 platfromTranslation = (transform.position + toPlatform) - platforms[i].transform.position;
            platfromTranslation.y = 0f;
            platforms[i].transform.Translate(platfromTranslation, Space.World);
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
}
