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

    // Start is called before the first frame update
    void Start()
    {
        translationInput = GetComponent<TranslationInput_Proto>();
        rotationInput = GetComponent<RotationInput_Proto>();

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

        //rotation around Y
        float headingInput = 0.0f;
        bool keyQ = Input.GetKey(KeyCode.Q);
        bool keyE = Input.GetKey(KeyCode.E);
        if (keyQ && !keyE) headingInput = -1.0f;
        else if (!keyQ && keyE) headingInput = 1.0f;

        if (translationInput != null)
        {
            if (translationX) translationInput.Translate(Axis_Proto.X, inputX * GetSignAxis(invertTranslationX));
            if (translationY) translationInput.Translate(Axis_Proto.Y, inputY * GetSignAxis(invertTranslationY));
            if (translationZ) translationInput.Translate(Axis_Proto.Z, inputZ * GetSignAxis(invertTranslationZ));
        }

        if (rotationInput != null)
        {
            if (rotationY) rotationInput.RotateNoLimits(Axis_Proto.Y, headingInput * GetSignAxis(invertRotationY));
        }
    }

    private float GetSignAxis(bool invertAxis) => invertAxis ? -1.0f : 1.0f;

    public enum Axis_Proto
    { X, Y, Z }
}
