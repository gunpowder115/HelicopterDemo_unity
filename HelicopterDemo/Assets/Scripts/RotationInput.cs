using UnityEngine;

public class RotationInput : MonoBehaviour
{
    [SerializeField] float maxAngleValue = 30.0f; //модуль максимального угла отклонения
    [SerializeField] float angleEpsilon = 0.1f; //модуль окрестности нулевого угла
    [SerializeField] float increaseAngleKoef = 1.0f; //коэф-т зависимости изменения угла от входного воздействия
    [SerializeField] float decreaseAngleKoef = 0.1f; //коэф-т сглаживания околонулевого угла

    private float[] angles;
    private float[] prevInputs;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        if (rigidBody)
            rigidBody.freezeRotation = true;

        angles = new float[3] { transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z };
        prevInputs = new float[3] { 0f, 0f, 0f };
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotateVector = new Vector3(angles[(int)InputManager.Axis.X],
                                            angles[(int)InputManager.Axis.Y],
                                            angles[(int)InputManager.Axis.Z]);
        transform.localEulerAngles = rotateVector;
    }

    public void RotateWithLimits(InputManager.Axis axis, float input)
    {
        int index = (int)axis;

        float deltaAngle = input * increaseAngleKoef;
        float angle = angles[index];
        float prevInput = prevInputs[index];
        float signInput = Mathf.Sign(input);
        float signPrevInput = Mathf.Sign(prevInput);
        float signAngle = Mathf.Sign(angle);
        float absAngle = Mathf.Abs(angle);
        float absInput = Mathf.Abs(input);
        float absPrevInput = Mathf.Abs(prevInput);

        if (signInput == signPrevInput && absInput - absPrevInput < 0 ||
            signInput != signPrevInput)
            deltaAngle = -deltaAngle;

        angle += deltaAngle;

        //если кнопка не нажата, то возврат вертолёта к нулевому углу
        if (absInput < 0.5f && absAngle > angleEpsilon * 2f)
            angle -= signAngle * absAngle * decreaseAngleKoef;

        angle = Mathf.Clamp(angle, -maxAngleValue, maxAngleValue);

        angles[index] = angle;
        prevInputs[index] = input;
    }

    public void RotateNoLimits(InputManager.Axis axis, float input)
    {
        int index = (int)axis;

        float deltaAngle = input * increaseAngleKoef;
        float angle = angles[index];

        angle += deltaAngle;

        angles[index] = angle;
    }
}
