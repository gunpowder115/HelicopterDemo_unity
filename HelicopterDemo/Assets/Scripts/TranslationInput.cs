using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class TranslationInput : MonoBehaviour
{
    [SerializeField] float speed = 6.0f;

    private CharacterController characterContoller;

    // Start is called before the first frame update
    void Start()
    {
        characterContoller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float deltaX = Input.GetAxis("Horizontal") * speed;
        float deltaY = Input.GetAxis("Jump") * speed;
        float deltaZ = Input.GetAxis("Vertical") * speed;

        Vector3 movement = new Vector3(deltaX, deltaY, deltaZ);
        movement = Vector3.ClampMagnitude(movement, speed);
        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);
        characterContoller.Move(movement);

        Rotation rotation = this.transform.gameObject.GetComponentInChildren<Rotation>();
        if (rotation != null)
        {
            rotation.Rotate(Rotation.Axis.X, deltaZ);
            rotation.Rotate(Rotation.Axis.Z, -deltaX);
        }
    }
}
