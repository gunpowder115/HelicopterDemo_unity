using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class TranslationInput : MonoBehaviour
{
    [SerializeField] float speed = 6.0f;    

    private CharacterController characterContoller;
    private float pitchAngle, rollAngle;
    private EulerAngleTarget pitchTarget, rollTarget;

    // Start is called before the first frame update
    void Start()
    {
        characterContoller = GetComponent<CharacterController>();
        pitchAngle = rollAngle = 0.0f;
        pitchTarget = rollTarget = EulerAngleTarget.Zero;
    }

    // Update is called once per frame
    void Update()
    {
        float deltaX = Input.GetAxis("Horizontal") * speed;
        float deltaY = Input.GetAxis("Jump") * speed;
        float deltaZ = Input.GetAxis("Vertical") * speed;

        rollTarget = GetAngleTarget(deltaX);
        pitchTarget = GetAngleTarget(-deltaZ);

        Vector3 movement = new Vector3(deltaX, deltaY, deltaZ);
        movement = Vector3.ClampMagnitude(movement, speed);
        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);
        characterContoller.Move(movement);

        Rotation rotation = this.transform.gameObject.GetComponentInChildren<Rotation>();
        if (rotation != null)
        {
            //rotation.IncreaseAngle(ref rollAngle, GetDeltaAngleSign(rollTarget, rollAngle));
            //rotation.IncreaseAngle(ref pitchAngle, GetDeltaAngleSign(pitchTarget, pitchAngle));
            //rotation.CropAngle(ref rollAngle);
            //rotation.CropAngle(ref pitchAngle);
            //rotation.Rotate(pitchAngle, rollAngle);

            rotation.Rotate(deltaZ);
        }
    }

    private int GetDeltaAngleSign(EulerAngleTarget angleTarget, float currentAngle)
    {
        if (angleTarget == EulerAngleTarget.Max ||
            angleTarget == EulerAngleTarget.Zero && currentAngle < 0f)
            return 1;
        else if (angleTarget == EulerAngleTarget.Min ||
            angleTarget == EulerAngleTarget.Zero && currentAngle > 0f)
            return -1;
        else
            return 0;
    }

    private EulerAngleTarget GetAngleTarget(float deltaAngle)
    {
        if (deltaAngle > 3f)
            return EulerAngleTarget.Min;
        else if (deltaAngle < -3f)
            return EulerAngleTarget.Max;
        else
            return EulerAngleTarget.Zero;
    }

    enum EulerAnglePos
    {
        Pos,
        Zero,
        Neg
    }

    enum EulerAngleTarget
    {
        Min,
        Zero,
        Max
    }
}
