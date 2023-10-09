using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] float maxAbsOfXandZ = 45.0f;
    [SerializeField] float minDeltaAngle = 0.1f;
    [SerializeField] float maxDeltaAngle = 1.5f;
    [SerializeField] float angleKoef = 0.1f;
    [SerializeField] float limitAngleKoef = 0.8f;
    [SerializeField] float angleAliasing = 15f;

    private float k, k1, k2;
    private float b1, b2;
    private float angleX, angleZ;
    private float deltaAngleX, deltaAngleZ;
    private float halfAbsOfXandZ;
    private float prevDelta;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        if (rigidBody)
            rigidBody.freezeRotation = true;

        halfAbsOfXandZ = maxAbsOfXandZ / 2f;
        angleX = angleZ = 0f;
        prevDelta = 0f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //public void CropAngle(ref float angle)
    //{
    //    angle = Mathf.Clamp(angle, -maxAbsOfXandZ, maxAbsOfXandZ);
    //}

    //public void IncreaseAngle(ref float angle, int sign)
    //{
    //    angle += deltaAngle * sign;
    //    if (Mathf.Abs(angle) < deltaAngle)
    //        angle = 0f;
    //}

    public void Rotate(float delta)
    {
        //Debug.Log(delta);
        deltaAngleX = delta * angleKoef;

        float signDelta = Mathf.Sign(delta);
        float signPrevDelta = Mathf.Sign(prevDelta);
        float absDelta = Mathf.Abs(delta);
        float absPrevDelta = Mathf.Abs(prevDelta);
        if (signDelta == signPrevDelta && absDelta - absPrevDelta < 0 ||
            signDelta != signPrevDelta)
        {
            deltaAngleX = -deltaAngleX;
        }

        if (maxAbsOfXandZ - Mathf.Abs(angleX) < angleAliasing && Mathf.Abs(delta) < 5)
        {
            Debug.Log("FROM MAX");
            angleX -= Mathf.Sign(angleX) * minDeltaAngle * (maxAbsOfXandZ - Mathf.Abs(angleX)) * limitAngleKoef;
        }
        else if (maxAbsOfXandZ - Mathf.Abs(angleX) < angleAliasing && Mathf.Abs(delta) > 7)
        {
            Debug.Log("TO MAX");
            angleX += Mathf.Sign(angleX) * minDeltaAngle * (maxAbsOfXandZ - Mathf.Abs(angleX)) * limitAngleKoef;
        }
        else if (Mathf.Abs(delta) < 7 && Mathf.Abs(angleX) > minDeltaAngle * 2)
        {
            Debug.Log("TO ZERO");
            angleX += deltaAngleX;
            angleX -= Mathf.Sign(angleX) * minDeltaAngle * Mathf.Abs(angleX) * angleKoef * 3.5f;
        }
        else
        {
            Debug.Log("ANOTHER");
            angleX += deltaAngleX;
        }
        angleX = Mathf.Clamp(angleX, -maxAbsOfXandZ, maxAbsOfXandZ);

        Vector3 rotateVector = new Vector3(angleX, 0, 0);
        transform.localEulerAngles = rotateVector;

        prevDelta = delta;
    }

    //public void Rotate(/*float x, float y, float z*/float delta)
    //{
    //    float sign = Mathf.Sign(delta);
    //    k = 2f * sign * (maxDeltaAngle - minDeltaAngle) / maxAbsOfXandZ;
    //    k1 = k;
    //    k2 = -k;
    //    b1 = sign * minDeltaAngle;
    //    b2 = sign * minDeltaAngle - k2 * maxAbsOfXandZ;

    //    angleX = Mathf.Clamp(angleX, -maxAbsOfXandZ, maxAbsOfXandZ);

    //    float xAbs = Mathf.Abs(angleX);

    //    float kCurr, bCurr;
    //    if (xAbs >= 0 && xAbs <= halfAbsOfXandZ)
    //    {
    //        kCurr = k1;
    //        bCurr = b1;
    //    }
    //    else
    //    {
    //        kCurr = k2;
    //        bCurr = b2;
    //    }
    //    deltaAngleX = kCurr * angleX + bCurr;
    //    angleX += deltaAngleX;
    //    //angleX += press > 0f ? deltaAngleX : -deltaAngleX;

    //    Vector3 rotateVector = new Vector3(angleX, 0, 0);
    //    transform.localEulerAngles = rotateVector;
    //}

    //public void Rotate(float x, float z)
    //{
    //    Rotate(x, 0, z);
    //}

    //public void Rotate(float y)
    //{
    //    Rotate(0, y, 0);
    //}
}
