using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject CreateBuild(GameObject buildPrefab)
    {
        GameObject build = Instantiate(buildPrefab);

        build.transform.position = this.transform.position;
        build.transform.rotation = this.transform.rotation;

        return build;
    }
}
