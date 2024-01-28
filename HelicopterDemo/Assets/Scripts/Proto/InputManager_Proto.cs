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
    private float angularDistance;
    private Vector3 currentDirection;

    // Start is called before the first frame update
    void Start()
    {
        translationInput = GetComponentInChildren<TranslationInput_Proto>();
        rotationInput = GetComponentInChildren<RotationInput_Proto>();
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
        //movement around X, Y, Z and rotation around X, Z
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Jump");
        float inputZ = Input.GetAxis("Vertical");

        //change speed (high/low)
        bool keyQ = Input.GetKeyDown(KeyCode.Q);


        if (translationInput != null)
        {
            if (keyQ) rotateToDirection = translationInput.ChangeSpeed();
            angularDistance = translationInput.GetAngularDistance(currentDirection);
            targetDirection = translationInput.TargetDirection;            

            if (translationX) translationInput.Translate(Axis_Proto.X, inputX * GetSignAxis(invertTranslationX));
            if (translationY) translationInput.Translate(Axis_Proto.Y, inputY * GetSignAxis(invertTranslationY));
            if (translationZ) translationInput.Translate(Axis_Proto.Z, inputZ * GetSignAxis(invertTranslationZ));
        }

        if (rotationInput != null)
        {
            currentDirection = rotationInput.CurrentDirection;

            if ((translationX || translationZ) && (rotationX || rotationZ)) rotationInput.Rotate(targetDirection);
            if (rotationY && rotateToDirection) rotationInput.DecreaseAngularDistance(Axis_Proto.Y, 1.0f, angularDistance);
        }
    }

    private float GetSignAxis(bool invertAxis) => invertAxis ? -1.0f : 1.0f;

    public enum Axis_Proto
    { X, Y, Z }
}
