using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] float sensitivityXandZ = 6.0f;
    [SerializeField] float sensitivityY = 6.0f;
    [SerializeField] float maxAbsOfXandZ = 45.0f;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        if (rigidBody)
            rigidBody.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Rotate(float x, float y, float z)
    {
        x = Mathf.Clamp(x, -maxAbsOfXandZ, maxAbsOfXandZ);
        z = Mathf.Clamp(x, -maxAbsOfXandZ, maxAbsOfXandZ);
        Vector3 rotateVector = new Vector3(x, y, z) * Time.deltaTime;
        transform.localEulerAngles = rotateVector;
    }

    public void Rotate(float x, float z)
    {
        Rotate(x, 0, z);
    }

    public void Rotate(float y)
    {
        Rotate(0, y, 0);
    }
}
