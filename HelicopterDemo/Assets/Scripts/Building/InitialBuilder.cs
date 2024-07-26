using UnityEngine;

public class InitialBuilder : MonoBehaviour
{
    public const int platformCount = 8;

    [Header("Prefabs array from N (north, X = 0, Z = 1) to clockwise")]
    [SerializeField] private GameObject[] buildPrefabs = new GameObject[platformCount];
    [SerializeField] private float distToPlatform = 40f;

    public Platform[] GetPlatforms(in BaseCenter baseCenter)
    {
        var platforms = new Platform[platformCount];

        var platformComponents = GetComponentsInChildren<Platform>();
        if (platformComponents.Length != platformCount)
            Debug.LogError(this.ToString() + ": platformComponents.Length = " + platforms.Length);
        for (int i = 0; i < platformComponents.Length; i++)
        {
            platforms[i] = platformComponents[i];
            Vector3 toPlatform = (platforms[i].transform.position - transform.position).normalized * distToPlatform;
            Vector3 platfromTranslation = (transform.position + toPlatform) - platforms[i].transform.position;
            platfromTranslation.y = 0f;
            platforms[i].transform.Translate(platfromTranslation, Space.World);

            platforms[i].SetBaseCenter(baseCenter);
        }

        return platforms;
    }

    public GameObject[] InitBuildings(in Platform[] platforms)
    {
        var buildings = new GameObject[platformCount];

        for (int i = 0; i < platformCount; i++)
        {
            if (buildPrefabs[i])
            {
                buildings[i] = Instantiate(buildPrefabs[i], platforms[i].transform);
                buildings[i].transform.position = platforms[i].transform.position;
                buildings[i].transform.rotation = platforms[i].transform.rotation;
                buildings[i].transform.localScale = new Vector3(
                    1 / platforms[i].transform.localScale.x,
                    1 / platforms[i].transform.localScale.y,
                    1 / platforms[i].transform.localScale.z);

                Building building = buildings[i].GetComponent<Building>();
                if (building)
                    building.SetPlatform(platforms[i]);
            }
        }

        return buildings;
    }
}
