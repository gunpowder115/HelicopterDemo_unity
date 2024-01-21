using UnityEngine;

public class InputManager_Proto : MonoBehaviour
{
    [Header("Translation Axes")]
    [SerializeField] private bool translationX = true;
    [SerializeField] private bool invertTranslationX = false;
    [SerializeField] private bool translationY = true;
    [SerializeField] private bool invertTranslationY = false;
    [SerializeField] private bool translationZ = true;
    [SerializeField] private bool invertTranslationZ = false;
    [Header("Rotation Axes")]
    [SerializeField] private bool rotationX = false;
    [SerializeField] private bool invertRotationX = false;
    [SerializeField] private bool rotationY = false;
    [SerializeField] private bool invertRotationY = false;
    [SerializeField] private bool rotationZ = false;
    [SerializeField] private bool invertRotationZ = false;

    private TranslationInput_Proto translationInput;
    private RotationInput_Proto rotationInput;
    private bool rotateToDirection;
    private Vector3 targetDirection;
    private float targetAngle;

    // Start is called before the first frame update
    void Start()
    {
        translationInput = GetComponentInChildren<TranslationInput_Proto>();
        rotationInput = GetComponentInChildren<RotationInput_Proto>();
        rotateToDirection = false;
        targetDirection = Vector3.forward;
        targetAngle = 0.0f;

        //hide cursor in center of screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //movement around X, Y, Z and rotatin around X, Z
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Jump");
        float inputZ = Input.GetAxis("Vertical");

        //change speed (high/low)
        bool keyQ = Input.GetKeyDown(KeyCode.Q);

        if (translationInput != null)
        {
            if (translationX) translationInput.Translate(Axis_Proto.X, inputX * GetSignAxis(invertTranslationX));
            if (translationY) translationInput.Translate(Axis_Proto.Y, inputY * GetSignAxis(invertTranslationY));
            if (translationZ) translationInput.Translate(Axis_Proto.Z, inputZ * GetSignAxis(invertTranslationZ));
            //targetAngle = translationInput.GetTargetAngle();
            targetDirection = translationInput.GetDirection();
        }

        if (keyQ)
        {
            rotateToDirection = translationInput.ChangeSpeed();
        }

        if (rotationInput != null)
        {
            //if (rotationY && rotateToDirection) rotationInput.RotateToAngle(Axis_Proto.Y, 1.0f, targetAngle);
            if (rotationY && rotateToDirection) rotationInput.RotateToDirection(Axis_Proto.Y, targetDirection);
        }
    }

    private float GetSignAxis(bool invertAxis) => invertAxis ? -1.0f : 1.0f;

    public enum Axis_Proto
    { X, Y, Z }
}
