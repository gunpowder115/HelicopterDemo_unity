using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainRotor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RotateMainRotor();
    }

    void RotateMainRotor()
    {
        transform.Rotate(Vector3.up);
    }
}
