using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotation : MonoBehaviour
{
    [SerializeField] float delta = 10.0f;
    [SerializeField] Axes axis = Axes.X;

    private Vector3 deltaRotation;

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
        transform.Rotate(deltaRotation);
    }

    public enum Axes
    {
        X, Y, Z
    }
}
