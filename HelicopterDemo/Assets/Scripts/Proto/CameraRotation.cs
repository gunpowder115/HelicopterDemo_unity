using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] float maxHorizontalAngle = 30f;
    [SerializeField] float maxVerticalAngle_cameraUp = 5f;
    [SerializeField] float maxVerticalAngle_cameraDown = 40f;
    [SerializeField] float horizontalSpeed = 1f;
    [SerializeField] float horizontalSpeedManual = 1f;
    [SerializeField] float verticalSpeed = 1f;

    private float horizontalRot, verticalRot;
    private float inputHor, inputVert;
    private float targetCameraVertRot;

    private readonly float defaultVerticalAngle = 15f;

    private void Start()
    {
        horizontalRot = verticalRot = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        inputHor = Input.GetAxis("CameraHorizontal");
        inputVert = Input.GetAxis("CameraVertical");

        //transform.localEulerAngles = new Vector3(verticalRot + defaultVerticalAngle, horizontalRot, 0f);
    }
    
    public void RotateCameraHorizontally(float playerDirX)
    {
        float currHorSpeed = horizontalSpeed;
        if (inputHor != 0f)
        {
            playerDirX += inputHor;
            currHorSpeed = horizontalSpeedManual;
        }

        float targetCameraHorRot = playerDirX * maxHorizontalAngle;
        
        Vector3 eulerAnglesCurrent = transform.rotation.eulerAngles;
        Vector3 eulerAnglesTarget = new Vector3(eulerAnglesCurrent.x, targetCameraHorRot, eulerAnglesCurrent.z);

        Quaternion rotationTarget = Quaternion.Euler(eulerAnglesTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationTarget, currHorSpeed * Time.deltaTime);
    }

    public void RotateCameraVertically()
    {
        targetCameraVertRot = inputVert * (inputVert > 0 ? maxVerticalAngle_cameraDown - defaultVerticalAngle : 
            maxVerticalAngle_cameraUp - defaultVerticalAngle);
        float rotSign = Mathf.Sign(targetCameraVertRot - verticalRot);

        if (Mathf.Abs(verticalRot - targetCameraVertRot) > 2f * verticalSpeed)
        {
            verticalRot += rotSign * verticalSpeed;
            verticalRot = Mathf.Clamp(verticalRot, maxVerticalAngle_cameraUp - defaultVerticalAngle, 
                                                    maxVerticalAngle_cameraDown - defaultVerticalAngle);
        }
    }
}
