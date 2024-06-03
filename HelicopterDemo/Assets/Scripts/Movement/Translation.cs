using UnityEngine;

public class Translation : MonoBehaviour
{
    [SerializeField] float maxHeight = 50.0f;
    [SerializeField] float minHeight = 10.0f;

    public Vector3 TargetDirectionNorm => new Vector3(speed.x, 0f, speed.z).normalized;
    public bool IsHeightBorder => this.gameObject.transform.position.y >= maxHeight || this.gameObject.transform.position.y <= minHeight;
    public bool RotToDir { get; private set; }

    Rigidbody rigidbody;
    Vector3 speed;
    float speedAbs, verticalSpeedAbs;

    public void SetGlobalTranslation(Vector3 speed)
    {
        speedAbs = speed.magnitude;
        this.speed = new Vector3(speed.x, this.speed.y, speed.z);
    }

    public void SetRelToTargetTranslation(Vector3 speed, float angle)
    {
        speedAbs = speed.magnitude;
        Vector3 temp = Quaternion.Euler(0f, angle, 0f) * speed;
        this.speed = new Vector3(temp.x, this.speed.y, temp.z);
    }

    public void SetVerticalTranslation(float speed)
    {
        this.speed = new Vector3(this.speed.x, speed, this.speed.z);
    }

    public bool SwitchRotation()
    {
        RotToDir = !RotToDir;
        return RotToDir;
    }

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(speed.x, 0f, speed.z);
        movement = Vector3.ClampMagnitude(movement, speedAbs);
        movement = new Vector3(movement.x, speed.y, movement.z);
        if (rigidbody)
        {
            rigidbody.AddForce(movement, ForceMode.Force);
            Debug.Log(rigidbody.velocity.magnitude);
        }
    }
}