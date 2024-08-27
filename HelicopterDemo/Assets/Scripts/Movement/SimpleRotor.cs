using UnityEngine;

public class SimpleRotor : MonoBehaviour
{
    [SerializeField] private float rotSpeed = 10.0f;
    [SerializeField] private float rotAcceleration = 0.2f;
    [SerializeField] private Axes axis = Axes.X;

    private float currRotorSpeed, tgtRotorSpeed;

    // Update is called once per frame
    void Update()
    {
        currRotorSpeed = Mathf.Lerp(currRotorSpeed, tgtRotorSpeed, rotAcceleration * Time.deltaTime);
        Vector3 rotation = new Vector3(axis == Axes.X ? currRotorSpeed : 0f,
                                        axis == Axes.Y ? currRotorSpeed : 0f,
                                        axis == Axes.Z ? currRotorSpeed : 0f);
        transform.Rotate(rotation);
    }

    public void StartRotor() => tgtRotorSpeed = rotSpeed;
    public void StopRotor() => tgtRotorSpeed = 0f;

    public enum Axes
    {
        X, Y, Z
    }
}
