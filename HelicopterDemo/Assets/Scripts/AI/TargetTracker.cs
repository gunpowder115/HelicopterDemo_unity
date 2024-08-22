using UnityEngine;

public class TargetTracker : MonoBehaviour
{
    [SerializeField] Axes axes = Axes.X;
    [SerializeField] float speed = 1.0f;
    [SerializeField] float minLookDistance = 30.0f;
    [SerializeField] float maxLookDistance = 50.0f;

    private Quaternion targetRotation;

    private void Start()
    {
        targetRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, speed * Time.deltaTime);
        switch (axes)
        {
            case Axes.X:
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0, 0);
                break;
            case Axes.Y:
                transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
                break;
            case Axes.Z:
                transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z);
                break;
        }
    }

    public void SetRotation(GameObject target, Vector3 defaultDir)
    {
        if (target)
        {
            Vector3 direction = target.transform.position - transform.position;
            targetRotation = Quaternion.LookRotation(direction);
        }
        else
            targetRotation = Quaternion.LookRotation(defaultDir);
    }

    public enum Axes
    {
        X, Y, Z,
        X_Y_Z
    }
}
