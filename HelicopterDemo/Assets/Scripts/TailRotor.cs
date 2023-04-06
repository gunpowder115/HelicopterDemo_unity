using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailRotor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    void ProcessInput()
    {
        RotateTailRotor();
    }

    void RotateTailRotor()
    {
        transform.Rotate(new Vector3(10, 0, 0));
    }
}
