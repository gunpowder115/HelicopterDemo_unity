using UnityEngine;

public class TranslationInput : MonoBehaviour
{
    [SerializeField] float lowSpeed = 6.0f;
    [SerializeField] float highSpeed = 10.0f;

    public Vector3 TargetDirection
    {
        get => new Vector3(deltas[(int)InputManager.Axis_Proto.X], 0, deltas[(int)InputManager.Axis_Proto.Z]).normalized;
    }

    public bool CurrSpeedIsHigh { get; private set; }

    private CharacterController characterContoller;
    private float[] deltas;
    private float currSpeed;
    //private bool currSpeedIsHigh;

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
                                        deltas[(int)InputManager.Axis_Proto.Y],
                                        deltas[(int)InputManager.Axis_Proto.Z]);
        movement = Vector3.ClampMagnitude(movement, currSpeed);
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
    }

    public void Translate(InputManager.Axis_Proto axis, float input)
    {
        deltas[(int)axis] = input * currSpeed;
    }

    public float GetAngularDistance(Vector3 currentDirection)
    {
        float angularDistance = Vector3.SignedAngle(currentDirection, TargetDirection, Vector3.up);
        return angularDistance;
    }

    public bool ChangeSpeed()
    {
        currSpeed = CurrSpeedIsHigh ? lowSpeed : highSpeed;
        CurrSpeedIsHigh = currSpeed == highSpeed;
        return CurrSpeedIsHigh;
    }
}
