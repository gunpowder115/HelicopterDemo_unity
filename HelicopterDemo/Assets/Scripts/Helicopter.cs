using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoBehaviour
{
    const float deltaRotationAngle = 1.0f;
    const float stabilizationKoef = 0.2f;
    Rigidbody rigidBody;
    float currentPitchRotation;
    float currentRollRotation;
    float currentYawRotation;

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
        {
            rigidBody.AddRelativeForce(Vector3.up);
        }
    }

    void SimplePitch()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            currentPitchRotation = deltaRotationAngle;
            transform.Rotate(new Vector3(currentPitchRotation, 0, 0));
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            currentPitchRotation = -deltaRotationAngle;
            transform.Rotate(new Vector3(currentPitchRotation, 0, 0));
        }
        else
        {
            if (transform.localEulerAngles.x > deltaRotationAngle && transform.localEulerAngles.x < 360 - deltaRotationAngle)
                transform.Rotate(new Vector3(-currentPitchRotation, 0, 0) * stabilizationKoef);
            else
            {
                currentPitchRotation = 0;
                transform.localEulerAngles = new Vector3(currentPitchRotation, transform.localEulerAngles.y, transform.localEulerAngles.z);
            }

        }
    }

    void SimpleRoll()
    {
        print(transform.localEulerAngles.z);
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            currentRollRotation = deltaRotationAngle;
            transform.Rotate(new Vector3(0, 0, currentRollRotation));
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            currentRollRotation = -deltaRotationAngle;
            transform.Rotate(new Vector3(0, 0, currentRollRotation));
        }
        else
        {
            if (transform.localEulerAngles.z > deltaRotationAngle && transform.localEulerAngles.z < 360 - deltaRotationAngle)
                transform.Rotate(new Vector3(0, 0, -currentRollRotation) * stabilizationKoef);
            else
            {
                currentRollRotation = 0;
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, currentRollRotation);
            }
        }
    }

    void SimpleYaw()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            currentYawRotation = -deltaRotationAngle;
            transform.Rotate(new Vector3(0, currentYawRotation, 0));
        }
        if (Input.GetKey(KeyCode.E))
        {
            currentYawRotation = deltaRotationAngle;
            transform.Rotate(new Vector3(0, currentYawRotation, 0));
        }
    }

    void SimpleRotation()
    {
        rigidBody.freezeRotation = true;
        SimplePitch();
        SimpleRoll();
        SimpleYaw();
        rigidBody.freezeRotation = false;
    }
}
