using UnityEngine;

public class SimpleRotation : MonoBehaviour
{
    [SerializeField] float delta = 10.0f;
    [SerializeField] Axes axis = Axes.X;

    Vector3 deltaRotation;

    // Start is called before the first frame update
    void Start()
    {
        deltaRotation = new Vector3(axis == Axes.X ? delta : 0,
                                    axis == Axes.Y ? delta : 0,
                                    axis == Axes.Z ? delta : 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
            transform.Rotate(deltaRotation);
        else
        {
            float x = axis == Axes.X ? 0f : transform.eulerAngles.x;
            float y = axis == Axes.Y ? 0f : transform.eulerAngles.y;
            float z = axis == Axes.Z ? 0f : transform.eulerAngles.z;
            transform.eulerAngles = new Vector3(x, y, z);
        }
    }

    public enum Axes
    {
        X, Y, Z
    }
}
