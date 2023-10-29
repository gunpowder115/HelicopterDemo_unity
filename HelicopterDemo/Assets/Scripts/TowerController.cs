using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    [SerializeField] int towerCount = 8;
    [SerializeField] float minDistance = 100.0f;
    [SerializeField] float maxDistance = 300.0f;
    [SerializeField] GameObject towerPrefab;

    private GameObject[] towers;

    // Start is called before the first frame update
    void Start()
    {
        CreateTowers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateTowers()
    {
        towers = new GameObject[towerCount];
        float deltaAngle = 360 / towerCount;

        for (int i = 0; i < towerCount; i++)
        {
            towers[i] = Instantiate(towerPrefab);

            float towerDistance = Random.Range(minDistance, maxDistance);
            towers[i].transform.position = new Vector3(0, 0, 0);
            towers[i].transform.Rotate(0, deltaAngle * i, 0);
            towers[i].transform.Translate(towerDistance, 0, 0);
        }
    }
}
