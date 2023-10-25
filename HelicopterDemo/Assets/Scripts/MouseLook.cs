using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] RotationAxes axes = RotationAxes.MouseXAndY;
    [SerializeField] float sensitivityHor = 6.0f;
    [SerializeField] float sensitivityVert = 6.0f;
    [SerializeField] float minVertAngle = -30.0f;
    [SerializeField] float maxVertAngle = 30.0f;

    private float verticalRot = 0.0f;
    private float horizontalRot = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(axes)
        {
            case RotationAxes.MouseX:
                transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityVert, 0);
                break;
            case RotationAxes.MouseY:
                verticalRot -= Input.GetAxis("Mouse Y") * sensitivityVert;
                verticalRot = Mathf.Clamp(verticalRot, minVertAngle, maxVertAngle);

                horizontalRot = transform.localEulerAngles.y;

                transform.localEulerAngles = new Vector3(verticalRot, horizontalRot, 0);
                break;
            case RotationAxes.MouseXAndY:
                verticalRot -= Input.GetAxis("Mouse Y") * sensitivityVert;
                verticalRot = Mathf.Clamp(verticalRot, minVertAngle, maxVertAngle);

                float horDelta = Input.GetAxis("Mouse X") * sensitivityHor;
                horizontalRot = transform.localEulerAngles.y + horDelta;

                transform.localEulerAngles = new Vector3(verticalRot, horizontalRot, 0);
                break;
        }
    }

    public enum RotationAxes
    {
        MouseXAndY,
        MouseX,
        MouseY
    }
}
