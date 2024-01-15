using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class TranslationInput_Proto : MonoBehaviour
{
    [SerializeField] float speed = 6.0f;

    private CharacterController characterContoller;
    private float[] deltas;

    // Start is called before the first frame update
    void Start()
    {
        deltas = new float[3] { 0f, 0f, 0f };
        characterContoller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = new Vector3(deltas[(int)InputManager_Proto.Axis_Proto.X],
                                        deltas[(int)InputManager_Proto.Axis_Proto.Y],
                                        deltas[(int)InputManager_Proto.Axis_Proto.Z]);
        movement = Vector3.ClampMagnitude(movement, speed);
        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);
        if (characterContoller != null)
            characterContoller.Move(movement);
    }

    public void Translate(InputManager_Proto.Axis_Proto axis, float input)
    {
        deltas[(int)axis] = input * speed;
    }
}
