using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class TranslationInput : MonoBehaviour
{
    [SerializeField] float lowSpeed = 6.0f;
    [SerializeField] float highSpeed = 10.0f;

    public Vector3 TargetDirection 
    { 
        get => new Vector3(deltas[(int)InputManager.Axis_Proto.X], 0, deltas[(int)InputManager.Axis_Proto.Z]).normalized; 
    }

    private CharacterController characterContoller;
    private float[] deltas;
    private float currSpeed;
    private bool currSpeedIsHigh;
    private Vector3 horizontalMovement;

    // Start is called before the first frame update
    void Start()
    {
        currSpeedIsHigh = false;
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
        movement = transform.TransformDirection(movement);
        if (characterContoller != null)
            characterContoller.Move(movement);
    }

    public void Translate(InputManager.Axis_Proto axis, float input)
    {
        deltas[(int)axis] = input * currSpeed;
    }

    public Vector3 GetDirection()
    {
        var temp = new Vector3(deltas[(int)InputManager.Axis_Proto.X], 0, deltas[(int)InputManager.Axis_Proto.Z]);
        if (temp != Vector3.zero) horizontalMovement = temp;
        return horizontalMovement;
    }

    public Vector3 GetDirectionNormalized() => GetDirection().normalized;

    public float GetTargetAngle()
    {
        var temp = new Vector3(deltas[(int)InputManager.Axis_Proto.X], 0, deltas[(int)InputManager.Axis_Proto.Z]).normalized;
        float targetAngle = Vector3.SignedAngle(transform.forward, temp, Vector3.up);
        return targetAngle;
    }

    public float GetAngularDistance(Vector3 currentDirection)
    {
        float angularDistance = Vector3.SignedAngle(currentDirection, TargetDirection, Vector3.up);
        return angularDistance;
    }

    public bool ChangeSpeed()
    {
        currSpeed = currSpeedIsHigh ? lowSpeed : highSpeed;
        currSpeedIsHigh = currSpeed == highSpeed;
        return currSpeedIsHigh;
    }
}
