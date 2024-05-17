using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTracker : MonoBehaviour
{
    [SerializeField] Axes axes = Axes.X;
    [SerializeField] float speed = 1.0f;
    [SerializeField] float minLookDistance = 30.0f;
    [SerializeField] float maxLookDistance = 50.0f;

    private GameObject target;
    private bool currentTrackingState;
    private Quaternion defaultDirection;

    // Start is called before the first frame update
    void Start()
    {
        currentTrackingState = false;
        defaultDirection = transform.rotation;
        target = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Vector3 direction = target.transform.position - this.transform.position;
            Vector3 directionHor = direction;
            directionHor.y = transform.position.y;

            if (!currentTrackingState && directionHor.magnitude < minLookDistance)
                currentTrackingState = true;
            else if (currentTrackingState && directionHor.magnitude > maxLookDistance)
                currentTrackingState = false;

            Quaternion rotation = currentTrackingState ? Quaternion.LookRotation(direction) : defaultDirection;
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, speed * Time.deltaTime);
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
    }

    public enum Axes
    {
        X, Y, Z,
        X_Y_Z
    }
}
