using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Translation))]
[RequireComponent(typeof(Rotation))]

public class CargoHelicopter : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float distance = 80f;
    [SerializeField] private float dropDistCoef = 0.2f;
    [SerializeField] private float lowSpeedCoef = 0.6f;
    [SerializeField] private float leaveDistCoef = 1.1f;
    [SerializeField] private float dropDistDelta = 1f;

    public bool CargoIsDelivered { get; private set; }
    public bool NearDropPoint => currDist < dropDistDelta;

    private Translation translation;
    private Rotation rotation;
    private Vector3 dropPoint;
    private float dropDist;
    private float currDist;

    private void Awake()
    {
        translation = GetComponent<Translation>();
        rotation = GetComponent<Rotation>();
        dropDist = dropDistCoef * distance;
    }

    // Update is called once per frame
    void Update()
    {
        currDist = Vector3.Magnitude(dropPoint - transform.position);
        float currentSpeed = currDist > dropDist ? speed : speed * lowSpeedCoef;
        translation.SetGlobalTranslation(transform.forward * currentSpeed);
        rotation.RotateToDirection(transform.forward, currentSpeed / speed, true);

        if (currDist > distance * leaveDistCoef)
            Destroy(gameObject);
    }

    public void Init(Vector3 cargoPlatformPos, float height)
    {
        dropPoint = new Vector3(cargoPlatformPos.x, cargoPlatformPos.y + height, cargoPlatformPos.z);
        currDist = distance;
        transform.Translate(0, height, -distance);
    }
}
