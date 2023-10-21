using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
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
    
    private TranslationInput translationInput;
    private RotationInput rotationInput;

    // Start is called before the first frame update
    void Start()
    {
        translationInput = GetComponent<TranslationInput>();
        rotationInput = GetComponent<RotationInput>();
    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Jump");
        float inputZ = Input.GetAxis("Vertical");

        if (translationInput != null)
        {
            if (translationX) translationInput.Translate(Axis.X, inputX * GetSignAxis(invertTranslationX));
            if (translationY) translationInput.Translate(Axis.Y, inputY * GetSignAxis(invertTranslationY));
            if (translationZ) translationInput.Translate(Axis.Z, inputZ * GetSignAxis(invertTranslationZ));
        }

        if (rotationInput != null)
        {
            if (rotationX) rotationInput.RotateWithLimits(Axis.X, inputZ * GetSignAxis(invertRotationX));
            if (rotationY) rotationInput.RotateNoLimits(Axis.Y, inputY * GetSignAxis(invertRotationY));
            if (rotationZ) rotationInput.RotateWithLimits(Axis.Z, inputX * GetSignAxis(invertRotationZ));
        }
    }

    private float GetSignAxis(bool invertAxis) => invertAxis ? -1.0f : 1.0f;

    public enum Axis
    { X, Y, Z }
}
