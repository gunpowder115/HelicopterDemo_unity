using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoBehaviour
{
    [SerializeField] float rotSpeed = 100.0f;
    [SerializeField] float takeSpeed = 800.0f;
    [SerializeField] float stabilizeDeltaAngle = 1.0f;
    [SerializeField] float stabilizationKoef = 0.2f;
    Rigidbody rigidBody;
    Vector3 vectorPitchRotation, vectorRollRotation, vectorYawRotation;
    float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        SimpleTakeoff();
        SimpleRotation();
    }

    void SimpleTakeoff()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Space))
            rigidBody.AddRelativeForce(Vector3.up * takeSpeed * Time.deltaTime);
    }

    void SimpleRotation()
    {
        rotationSpeed = rotSpeed * Time.deltaTime;

        rigidBody.freezeRotation = true;
        SimplePitchRotation();
        SimpleRollRotation();
        SimpleYawRotation();
        rigidBody.freezeRotation = false;
    }

    void SimplePitchRotation()
    {
        print("X angle = " + transform.localEulerAngles.x);

        bool pitchUp = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        bool pitchDown = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);

        if (pitchUp || pitchDown)
            Rotation(LocalAxes.Pitch, pitchDown, pitchUp);
        else
            Stabilization(LocalAxes.Pitch);
    }

    void SimpleRollRotation()
    {
        print("Z angle = " + transform.localEulerAngles.z);

        bool rollLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        bool rollRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

        if (rollLeft || rollRight)
            Rotation(LocalAxes.Roll, rollLeft, rollRight);
        else
            Stabilization(LocalAxes.Roll);
    }

    void SimpleYawRotation()
    {
        print("Y angle = " + transform.localEulerAngles.y);

        bool yawLeft = Input.GetKey(KeyCode.Q);
        bool yawRight = Input.GetKey(KeyCode.E);

        if (yawLeft || yawRight)
            Rotation(LocalAxes.Yaw, yawLeft, yawRight);
    }

    void Rotation(LocalAxes localAxis, bool counterClockwise, bool clockwise)
    {
        Vector3 vectorRotation = new Vector3(0, 0, 0);
        switch(localAxis)
        {
            case LocalAxes.Pitch:
                if (counterClockwise && !clockwise)
                    vectorPitchRotation = Vector3.right;
                else if (clockwise && !counterClockwise)
                    vectorPitchRotation = -Vector3.right;
                vectorRotation = vectorPitchRotation;
                break;
            case LocalAxes.Roll:
                if (counterClockwise && !clockwise)
                    vectorRollRotation = Vector3.forward;
                else if (clockwise && !counterClockwise)
                    vectorRollRotation = -Vector3.forward;
                vectorRotation = vectorRollRotation;
                break;
            case LocalAxes.Yaw:
                if (counterClockwise && !clockwise)
                    vectorYawRotation = -Vector3.up;
                else if (clockwise && !counterClockwise)
                    vectorYawRotation = Vector3.up;
                vectorRotation = vectorYawRotation;
                break;
            default: break;
        }
        transform.Rotate(vectorRotation * rotationSpeed);
    }

    void Stabilization(LocalAxes localAxis)
    {
        float localEulerAngle = 0;
        Vector3 stabVectorRotation = new Vector3(0, 0, 0);
        Vector3 newLocalEuler = new Vector3(0, 0, 0);
        switch(localAxis)
        {
            case LocalAxes.Pitch:
                localEulerAngle = transform.localEulerAngles.x;
                stabVectorRotation = -vectorPitchRotation;
                newLocalEuler = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
                break;
            case LocalAxes.Roll:
                localEulerAngle = transform.localEulerAngles.z;
                stabVectorRotation = -vectorRollRotation;
                newLocalEuler = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
                break;
            default: break;
        }

        if (localEulerAngle > stabilizeDeltaAngle && localEulerAngle < 360 - stabilizeDeltaAngle)
            transform.Rotate(stabVectorRotation * rotationSpeed * stabilizationKoef);
        else
            transform.localEulerAngles = newLocalEuler;
    }

    enum LocalAxes
    {
        Pitch,
        Roll,
        Yaw
    }
}