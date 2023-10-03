using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] float maxAbsOfXandZ = 45.0f;
    [SerializeField] float deltaAngle = 1.0f;

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

    public void CropAngle(ref float angle)
    {
        angle = Mathf.Clamp(angle, -maxAbsOfXandZ, maxAbsOfXandZ);
    }

    public void IncreaseAngle(ref float angle, int sign)
    {
        angle += deltaAngle * sign;
        if (Mathf.Abs(angle) < deltaAngle)
            angle = 0f;
    }

    public void Rotate(float x, float y, float z)
    {
        x = Mathf.Clamp(x, -maxAbsOfXandZ, maxAbsOfXandZ);
        z = Mathf.Clamp(z, -maxAbsOfXandZ, maxAbsOfXandZ);
        Vector3 rotateVector = new Vector3(x, y, z);
        transform.localEulerAngles = rotateVector;
        Debug.Log(rotateVector.z);
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
