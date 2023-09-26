using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour
{
    const int BUILD_NUM = 4;

    [SerializeField]
    BuildingType selectedBuilding = BuildingType.PlayerAirMissile;
    BuildingType oldSelectedBuilding;

    GameObject[] buildingPrefabs;
    GameObject currentBuilding;      
    Vector3 position;
    Quaternion rotation;

    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
        rotation = transform.rotation;

        buildingPrefabs = new GameObject[BUILD_NUM];

        buildingPrefabs[(int)BuildingType.PlayerAirMissile] = Resources.Load<GameObject>("PlayerDefenceSystem");
        buildingPrefabs[(int)BuildingType.EnemyAirMissile] = Resources.Load<GameObject>("EnemyDefenceSystem");
        buildingPrefabs[(int)BuildingType.PlayerAirGun] = Resources.Load<GameObject>("PlayerAirGun");
        buildingPrefabs[(int)BuildingType.EnemyAirGun] = Resources.Load<GameObject>("EnemyAirGun");

        currentBuilding = Instantiate(buildingPrefabs[(int)selectedBuilding], position, rotation);
        currentBuilding.transform.position = transform.position;
        currentBuilding.transform.rotation = transform.rotation;

        oldSelectedBuilding = selectedBuilding;
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedBuilding != oldSelectedBuilding)
        {
            if (currentBuilding != null)
            {
                Destroy(currentBuilding);
            }
            position = transform.position;
            rotation = transform.rotation;

            currentBuilding = Instantiate(buildingPrefabs[(int)selectedBuilding], position, rotation);
            currentBuilding.transform.position = transform.position;
            currentBuilding.transform.rotation = transform.rotation;

            oldSelectedBuilding = selectedBuilding;
        }
    }
}

public enum BuildingType
{
    PlayerAirMissile,
    EnemyAirMissile,
    PlayerAirGun,
    EnemyAirGun
}
