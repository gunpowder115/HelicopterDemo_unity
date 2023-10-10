using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] float maxAbsOfXandZ = 45.0f;
    [SerializeField] float minDeltaAngle = 0.1f;
    [SerializeField] float angleKoef = 0.1f;
    [SerializeField] float limitAngleKoef = 0.8f;
    [SerializeField] float angleAliasing = 15f;

    private float[] angles;
    private float[] prevDeltas;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        if (rigidBody)
            rigidBody.freezeRotation = true;

        angles = new float[3] { 0f, 0f, 0f };
        prevDeltas = new float[3] { 0f, 0f, 0f };
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotateVector = new Vector3(angles[(int)Axis.X],
                                            0,
                                            angles[(int)Axis.Z]);
        transform.localEulerAngles = rotateVector;
    }

    public void Rotate(Axis axis, float delta)
    {
        int index = (int)axis;

        float deltaAngle = delta * angleKoef;
        float angle = angles[index];
        float prevDelta = prevDeltas[index];

        float signDelta = Mathf.Sign(delta);
        float signPrevDelta = Mathf.Sign(prevDelta);
        float absDelta = Mathf.Abs(delta);
        float absPrevDelta = Mathf.Abs(prevDelta);

        if (signDelta == signPrevDelta && absDelta - absPrevDelta < 0 ||
            signDelta != signPrevDelta)
            deltaAngle = -deltaAngle;

        //кнопка не нажата, уход вертолёта из крайнего положения
        if (maxAbsOfXandZ - Mathf.Abs(angle) < angleAliasing && absDelta < 5)
            angle -= Mathf.Sign(angle) * minDeltaAngle * (maxAbsOfXandZ - Mathf.Abs(angle)) * limitAngleKoef;
        //кнопка нажата, приход вертолёта в крайнее положение
        else if (maxAbsOfXandZ - Mathf.Abs(angle) < angleAliasing && absDelta > 7 && Mathf.Sign(angle) == signDelta)
            angle += Mathf.Sign(angle) * minDeltaAngle * (maxAbsOfXandZ - Mathf.Abs(angle)) * limitAngleKoef;
        //кнопка не нажата, возврат вертолёта к нулевому углу
        else if (absDelta < 7 && Mathf.Abs(angle) > minDeltaAngle * 2)
        {
            angle += deltaAngle;
            angle -= Mathf.Sign(angle) * minDeltaAngle * Mathf.Abs(angle) * angleKoef * 3.5f;
        }
        //остальные случаи
        else
            angle += deltaAngle;

        angle = Mathf.Clamp(angle, -maxAbsOfXandZ, maxAbsOfXandZ);
        angles[index] = angle;

        prevDeltas[index] = delta;
    }

    public enum Axis
    {
        X, Y, Z
    }
}
