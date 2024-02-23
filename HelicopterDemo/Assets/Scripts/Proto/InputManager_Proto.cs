using UnityEngine;

public class InputManager_Proto : MonoBehaviour
{
    [SerializeField] float changeSpeedInput = 0.5f;

    private TranslationInput_Proto translationInput;
    private RotationInput_Proto rotationInput;
    private CameraRotation cameraRotation;
    private bool rotateToDirection;
    private Vector3 targetDirection;
    private float angularDistance;
    private Vector3 currentDirection;

    // Start is called before the first frame update
    void Start()
    {
        translationInput = GetComponentInChildren<TranslationInput_Proto>();
        rotationInput = GetComponentInChildren<RotationInput_Proto>();
        cameraRotation = GetComponentInChildren<CameraRotation>();
        rotateToDirection = false;
        targetDirection = transform.forward;
        currentDirection = transform.forward;
        angularDistance = 0.0f;

        //hide cursor in center of screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Jump");
        float inputZ = Input.GetAxis("Vertical");
        float inputXZ = Mathf.Clamp01(new Vector3(inputX, 0f, inputZ).magnitude);

        //change speed (high/low)
        bool keyQ = Input.GetKeyDown(KeyCode.Q);

        //movement around X, Y, Z
        if (translationInput != null)
        {
            if (keyQ) rotateToDirection = translationInput.ChangeSpeed();
            angularDistance = translationInput.GetAngularDistance(currentDirection);
            targetDirection = translationInput.TargetDirection;            

            translationInput.Translate(Axis_Proto.X, inputX);
            translationInput.Translate(Axis_Proto.Y, inputY);
            translationInput.Translate(Axis_Proto.Z, inputZ);
        }

        //rotation around X, Y, Z
        if (rotationInput != null)
        {
            currentDirection = rotationInput.CurrentDirection;

            rotationInput.Rotate(targetDirection, angularDistance, inputXZ, rotateToDirection);
        }

        //camera rotation
        cameraRotation.RotateCameraHorizontally(rotateToDirection ? currentDirection.x : targetDirection.x);
        //cameraRotation.RotateCameraVertically();
    }

    public enum Axis_Proto : int
    { X, Y, Z }
}
