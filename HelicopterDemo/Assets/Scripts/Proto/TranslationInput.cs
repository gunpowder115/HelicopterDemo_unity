using UnityEngine;

public class TranslationInput : MonoBehaviour
{
    [SerializeField] float lowSpeed = 6.0f;
    [SerializeField] float highSpeed = 10.0f;
    [SerializeField] float maxHeight = 50.0f;
    [SerializeField] float minHeight = 10.0f;

    public Vector3 TargetDirection => new Vector3(deltas[(int)InputManager.Axis_Proto.X], 0, deltas[(int)InputManager.Axis_Proto.Z]).normalized;
    public bool IsHeightBorder => this.gameObject.transform.position.y >= maxHeight || this.gameObject.transform.position.y <= minHeight;

    public bool CurrSpeedIsHigh { get; private set; }

    private CharacterController characterContoller;
    private float[] deltas;
    private float currSpeed;

    // Start is called before the first frame update
    void Start()
    {
        CurrSpeedIsHigh = false;
        currSpeed = lowSpeed;
        deltas = new float[3] { 0f, 0f, 0f };
        characterContoller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = new Vector3(deltas[(int)InputManager.Axis_Proto.X],
                                        0f,
                                        deltas[(int)InputManager.Axis_Proto.Z]);
        movement = Vector3.ClampMagnitude(movement, currSpeed);
        movement = new Vector3(movement.x, deltas[(int)InputManager.Axis_Proto.Y], movement.z);
        movement *= Time.deltaTime;

        if (characterContoller != null)
        {
            movement = transform.TransformDirection(movement);
            characterContoller.Move(movement);
        }
        else
        {
            movement = transform.InverseTransformDirection(movement);
            transform.Translate(movement);
        }

        if (this.gameObject.transform.position.y >= maxHeight)
            this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, maxHeight, this.gameObject.transform.position.z);
        else if (this.gameObject.transform.position.y <= minHeight)
            this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, minHeight, this.gameObject.transform.position.z);
    }

    public void TranslateGlobal(Vector3 input)
    {
        deltas[0] = input.x * currSpeed;
        deltas[1] = input.y * currSpeed;
        deltas[2] = input.z * currSpeed;
    }

    public void TranslateRelToTarget(Vector3 input, float angle)
    {
        Vector3 temp = new Vector3(input.x * lowSpeed, input.y * lowSpeed, input.z * lowSpeed);
        Quaternion rot = Quaternion.Euler(0f, angle, 0f);
        temp = rot * temp;
        deltas[0] = temp.x;
        deltas[1] = temp.y;
        deltas[2] = temp.z;
    }

    public bool ChangeSpeed()
    {
        currSpeed = CurrSpeedIsHigh ? lowSpeed : highSpeed;
        CurrSpeedIsHigh = currSpeed == highSpeed;
        return CurrSpeedIsHigh;
    }
}
