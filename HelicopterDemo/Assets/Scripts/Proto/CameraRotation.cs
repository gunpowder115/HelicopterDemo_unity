using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] float maxHorizontalAngle = 30f;
    [SerializeField] float maxVerticalAngle_cameraUp = 5f;
    [SerializeField] float maxVerticalAngle_cameraDown = 40f;
    [SerializeField] float horizontalSpeed = 1f;
    [SerializeField] float verticalSpeed = 1f;

    private float horizontalRot, verticalRot;
    private float inputHor, inputVert;
    private float playerDirX_sign;
    private float targetCameraHorRot;

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

        transform.localEulerAngles = new Vector3(verticalRot + defaultVerticalAngle, horizontalRot, 0f);
    }

    private void RotateCamera(float inputHor, float inputVert)
    {
        horizontalRot += inputHor * horizontalSpeed;
        horizontalRot = Mathf.Clamp(horizontalRot, -maxHorizontalAngle, maxHorizontalAngle);

        verticalRot += inputVert * verticalSpeed;
        verticalRot = Mathf.Clamp(verticalRot, maxVerticalAngle_cameraUp - defaultVerticalAngle, 
                                                maxVerticalAngle_cameraDown - defaultVerticalAngle);

        transform.localEulerAngles = new Vector3(verticalRot + defaultVerticalAngle, horizontalRot, 0f);
    }

    public void RotateCameraHorizontally(float playerDirX)
    {
        targetCameraHorRot = playerDirX * maxHorizontalAngle;
        playerDirX_sign = Mathf.Sign(playerDirX);
        float rotSign = Mathf.Sign(targetCameraHorRot - horizontalRot);

        if (Mathf.Abs(horizontalRot - targetCameraHorRot) > 2f * horizontalSpeed)
        {
            horizontalRot += rotSign * horizontalSpeed;
            horizontalRot = Mathf.Clamp(horizontalRot, -maxHorizontalAngle, maxHorizontalAngle);
        }
    }
}
