using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    const float MAX_ANGLE = 45.0f;
	const float VEL_STATIC = 1.0f;
	const float DELTA_ANGLE = 1.0f;

    [SerializeField] float speed;
    [SerializeField] float yawRotation;
    [SerializeField] float pitchRollRotation;
    
	bool toLeft, toRight, forward, backward;
    bool yawLeft, yawRight;
    int rollKoef, pitchKoef;
    float pitchRollRotationLocal;

    Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        rollKoef = 0;
        pitchKoef = 0;
        pitchRollRotationLocal = 0;
    }

    // Update is called once per frame
    void Update()
    {
        rigidBody.freezeRotation = true;
        SimpleLateralMoving();
        SimpleLongitudinalMoving();
        SimpleYawRotation();
        rigidBody.freezeRotation = false;
    }

    void SimpleLateralMoving()
    {
        float localRoll = TransformEulerAngle(transform.localEulerAngles.z);
        CheckTwoSideInput(Input.GetKey(KeyCode.LeftArrow), Input.GetKey(KeyCode.RightArrow), 
            out toLeft, out toRight);
        if (toLeft)
        {            
            rigidBody.AddRelativeForce(Vector3.left * speed * Time.deltaTime);
            if (localRoll < MAX_ANGLE)
            {
                if (localRoll < MAX_ANGLE / 2)
                    pitchRollRotationLocal = localRoll * 2;
                else
                    pitchRollRotationLocal = (MAX_ANGLE - localRoll) * 2;
                transform.Rotate(Vector3.forward * pitchRollRotationLocal * Time.deltaTime, DELTA_ANGLE);
                //rigidBody.AddRelativeTorque(Vector3.forward * pitchRollRotation * Time.deltaTime);
            }
            rollKoef = -1;
        }
        else if (toRight)
        {
            rigidBody.AddRelativeForce(Vector3.right * speed * Time.deltaTime);
            if (localRoll > -MAX_ANGLE)
            {
                if (localRoll > -MAX_ANGLE / 2)
                    pitchRollRotationLocal += DELTA_ANGLE * 2;
                else
                    pitchRollRotationLocal -= DELTA_ANGLE * 2;
                transform.Rotate(-Vector3.forward * pitchRollRotationLocal * Time.deltaTime, DELTA_ANGLE);
                //rigidBody.AddRelativeTorque(-Vector3.forward * pitchRollRotation * Time.deltaTime);
            }
            rollKoef = 1;
        }
        else
        {
            float localRollAbs = localRoll > 0.0f ? localRoll : -localRoll;
            if (localRollAbs > DELTA_ANGLE && rollKoef != 0)
            {
                if (localRollAbs > MAX_ANGLE / 2)
                    pitchRollRotationLocal += DELTA_ANGLE * 2;
                else
                    pitchRollRotationLocal -= DELTA_ANGLE * 2;
                transform.Rotate(Vector3.forward * rollKoef * pitchRollRotationLocal * Time.deltaTime, DELTA_ANGLE);
                //rigidBody.AddRelativeTorque(Vector3.forward * rollKoef * pitchRollRotation * Time.deltaTime);
            }
            else
            {
                rollKoef = 0;
                pitchRollRotationLocal = 0;
            }
        }
    }

    void SimpleLongitudinalMoving()
    {
        float localPitch = TransformEulerAngle(transform.localEulerAngles.x);
        CheckTwoSideInput(Input.GetKey(KeyCode.UpArrow), Input.GetKey(KeyCode.DownArrow), 
            out forward, out backward);
        if (forward)
        {
            rigidBody.AddRelativeForce(Vector3.forward * speed * Time.deltaTime);
            if (localPitch < MAX_ANGLE)
                transform.Rotate(-Vector3.left * pitchRollRotation * Time.deltaTime, DELTA_ANGLE);
            pitchKoef = 1;
        }
        else if (backward)
        {
            rigidBody.AddRelativeForce(-Vector3.forward * speed * Time.deltaTime);
            if (localPitch > -MAX_ANGLE)
                transform.Rotate(Vector3.left * pitchRollRotation * Time.deltaTime, DELTA_ANGLE);
            pitchKoef = -1;
        }
        else
        {
            float localPitchAbs = localPitch > 0.0f ? localPitch : -localPitch;
            if (localPitchAbs > DELTA_ANGLE && pitchKoef != 0)
                transform.Rotate(Vector3.left * pitchKoef * pitchRollRotation * Time.deltaTime, DELTA_ANGLE);
            else
                pitchKoef = 0;
        }
    }

    void SimpleYawRotation()
    {
        int sign = 0;
        CheckTwoSideInput(Input.GetKey(KeyCode.Q), Input.GetKey(KeyCode.E),
            out yawLeft, out yawRight);
        if (yawLeft) sign = -1;
        else if (yawRight) sign = 1;
        transform.Rotate(Vector3.up * sign * yawRotation * Time.deltaTime, Space.World);
    }

    float TransformEulerAngle(float localEulerAngle)
    {
        if (localEulerAngle > 180 && localEulerAngle < 360)
            return localEulerAngle - 360;
        else
            return localEulerAngle;
    }

    void CheckTwoSideInput(bool keyPos, bool keyNeg, out bool resultPos, out bool resultNeg)
    {
        resultPos = keyPos && !keyNeg;
        resultNeg = keyNeg && !keyPos;
    }

    void Limit(ref float value, float min, float max)
    {
        if (value >= max)
            value = max;
        else if (value <= min)
            value = min;
    }
}
