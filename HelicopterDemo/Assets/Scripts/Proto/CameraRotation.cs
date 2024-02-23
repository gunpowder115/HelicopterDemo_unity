using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] float maxHorizontalAngle = 30f;
    [SerializeField] float maxVerticalAngle_cameraUp = 40f;
    [SerializeField] float maxVerticalAngle_cameraDown = 5f;
    [SerializeField] float rotSpeed = 1f;
    [SerializeField] float rotSpeedManual = 1f;

    private float inputHor, inputVert;

    private readonly float defaultVerticalAngle = 15f;

    // Update is called once per frame
    void Update()
    {
        inputHor = Input.GetAxis("CameraHorizontal");
        inputVert = Input.GetAxis("CameraVertical");
    }

    public void RotateHorizontally(float playerDirX)
    {
        playerDirX += inputHor;

        float targetCameraHorRot = playerDirX * maxHorizontalAngle;

        Vector3 eulerAnglesCurrent = transform.rotation.eulerAngles;
        float currRotSpeed = inputHor != 0f ? rotSpeedManual : rotSpeed;
        Vector3 eulerAnglesTarget = new Vector3(eulerAnglesCurrent.x, targetCameraHorRot, eulerAnglesCurrent.z);

        Quaternion rotationTarget = Quaternion.Euler(eulerAnglesTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationTarget, currRotSpeed * Time.deltaTime);
    }

    public void RotateVertically(float playerDirZ)
    {
        playerDirZ += inputVert;

        float targetCameraVertRot;
        if (playerDirZ > 0f)
            targetCameraVertRot = playerDirZ * maxVerticalAngle_cameraUp;
        else if (playerDirZ < 0f)
            targetCameraVertRot = playerDirZ * maxVerticalAngle_cameraDown;
        else
            targetCameraVertRot = defaultVerticalAngle;

        Vector3 eulerAnglesCurrent = transform.rotation.eulerAngles;
        float currRotSpeed = rotSpeedManual;
        Vector3 eulerAnglesTarget = new Vector3(targetCameraVertRot, eulerAnglesCurrent.y, eulerAnglesCurrent.z);

        Quaternion rotationTarget = Quaternion.Euler(eulerAnglesTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationTarget, currRotSpeed * Time.deltaTime);
    }
}
