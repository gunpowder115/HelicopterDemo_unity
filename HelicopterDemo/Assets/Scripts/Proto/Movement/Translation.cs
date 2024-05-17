using UnityEngine;

public class Translation : MonoBehaviour
{
    [SerializeField] float maxHeight = 50.0f;
    [SerializeField] float minHeight = 10.0f;

    public Vector3 TargetDirection => new Vector3(deltas[(int)Player.Axis_Proto.X], 0, deltas[(int)Player.Axis_Proto.Z]).normalized;
    public float CurrSpeed { get; set; }
    public bool IsHeightBorder => this.gameObject.transform.position.y >= maxHeight || this.gameObject.transform.position.y <= minHeight;
    public bool RotToDir { get; private set; }

    private CharacterController characterContoller;
    private float[] deltas;

    // Start is called before the first frame update
    void Start()
    {
        deltas = new float[3] { 0f, 0f, 0f };
        characterContoller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = new Vector3(deltas[(int)Player.Axis_Proto.X],
                                        0f,
                                        deltas[(int)Player.Axis_Proto.Z]);
        movement = Vector3.ClampMagnitude(movement, CurrSpeed);
        movement = new Vector3(movement.x, deltas[(int)Player.Axis_Proto.Y], movement.z);
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

    public void TranslateGlobal(Vector3 input, float currSpeed)
    {
        CurrSpeed = currSpeed;
        input *= CurrSpeed;
        deltas[0] = input.x;
        deltas[1] = input.y;
        deltas[2] = input.z;
    }

    public void TranslateRelToTarget(Vector3 input, float angle, float currSpeed)
    {
        CurrSpeed = currSpeed;
        input *= CurrSpeed;
        Vector3 temp = new Vector3(input.x, input.y, input.z);
        Quaternion rot = Quaternion.Euler(0f, angle, 0f);
        temp = rot * temp;
        deltas[0] = temp.x;
        deltas[1] = temp.y;
        deltas[2] = temp.z;
    }

    public bool SwitchRotation()
    {
        RotToDir = !RotToDir;
        return RotToDir;
    }
}
