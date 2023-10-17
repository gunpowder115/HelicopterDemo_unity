using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class TranslationInput : MonoBehaviour
{
    [SerializeField] float speed = 6.0f;
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

    private CharacterController characterContoller;

    // Start is called before the first frame update
    void Start()
    {
        characterContoller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Jump");
        float inputZ = Input.GetAxis("Vertical");

        float deltaX = inputX * speed * GetSignAxis(invertTranslationX);
        float deltaY = inputY * speed * GetSignAxis(invertTranslationY);
        float deltaZ = inputZ * speed * GetSignAxis(invertTranslationZ);

        Vector3 movement = new Vector3(translationX ? deltaX : 0.0f,
                                        translationY ? deltaY : 0.0f,
                                        translationZ ? deltaZ : 0.0f);
        movement = Vector3.ClampMagnitude(movement, speed);
        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);
        characterContoller.Move(movement);

        Rotation rotation = this.transform.gameObject.GetComponent<Rotation>();
        //if (rotation == null) rotation = this.transform.gameObject.GetComponentInChildren<Rotation>();
        if (rotation != null)
        {
            if (rotationX) rotation.RotateWithLimits(Rotation.Axis.X, inputZ * GetSignAxis(invertRotationX));
            if (rotationY) rotation.RotateNoLimits(Rotation.Axis.Y, inputY * GetSignAxis(invertRotationY));
            if (rotationZ) rotation.RotateWithLimits(Rotation.Axis.Z, inputX * GetSignAxis(invertRotationZ));
        }
    }

    private float GetSignAxis(bool invertAxis) => invertAxis ? -1.0f : 1.0f;
}
