using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private float dropDistDelta = 0.5f;
    [SerializeField] private float cableLength = 5f;

    public bool CargoIsDelivered { get; private set; }
    public bool NearDropPoint => currDist < dropDistDelta;
    public float CableLength => cableLength;

    private bool isEscape;
    private float dropDist;
    private float currDist;
    private Vector3 dropPoint, deliveryPoint, escapePoint;
    private Translation translation;
    private Rotation rotation;
    private List<SimpleRotor> rotors;

    private void Awake()
    {
        translation = GetComponent<Translation>();
        rotation = GetComponent<Rotation>();
        dropDist = dropDistCoef * distance;
        rotors = new List<SimpleRotor>();
        rotors.AddRange(GetComponentsInChildren<SimpleRotor>());
        foreach (var rotor in rotors)
            rotor.FastStartRotor();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 toDeliveryPoint = deliveryPoint - transform.position;
        Vector3 toEscapePoint = (escapePoint - transform.position).normalized;
        currDist = toDeliveryPoint.magnitude;
        toDeliveryPoint = toDeliveryPoint.normalized;
        float currentSpeed = currDist > dropDist ? speed : speed * lowSpeedCoef;

        if (isEscape)
            translation.SetGlobalTranslation(toEscapePoint * currentSpeed);
        else
        {
            translation.SetGlobalTranslation(toDeliveryPoint * currentSpeed);
            if (NearDropPoint)
                isEscape = true;
        }
        //rotation.RotateToDirection(toDeliveryPoint, currentSpeed / speed, true);

        if (currDist > distance * leaveDistCoef)
            Destroy(gameObject);
    }

    public void InitForDrop(Vector3 cargoPlatformPos, float height)
    {
        dropPoint = new Vector3(cargoPlatformPos.x, cargoPlatformPos.y + height, cargoPlatformPos.z);
        deliveryPoint = dropPoint;
        currDist = distance;
        transform.Translate(0, height, -distance);
        escapePoint = transform.position + transform.forward * 2.5f * distance;
    }

    public void InitForDelivery(Vector3 cargoPlatformPos, float height)
    {
        dropPoint = new Vector3(cargoPlatformPos.x, cargoPlatformPos.y + height + cableLength, cargoPlatformPos.z);
        deliveryPoint = new Vector3(cargoPlatformPos.x, cargoPlatformPos.y + cableLength, cargoPlatformPos.z);
        currDist = distance;
        transform.Translate(0, height + cableLength, -distance);
        escapePoint = transform.position + transform.forward * 2.5f * distance;
    }
}
