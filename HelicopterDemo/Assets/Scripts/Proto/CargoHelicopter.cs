using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoHelicopter : MonoBehaviour
{
    [SerializeField] private GameObject cargoPrefab;
    [SerializeField] private CargoPlatform.CargoType cargoType = CargoPlatform.CargoType.helicopter;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float minDistance = 80f;
    [SerializeField] private float maxDistance = 120f;
    [SerializeField] private float targetHeight = 15f;
    [SerializeField] private float minHeightRange = 2f;
    [SerializeField] private float maxHeightRange = 3f;

    public bool CargoIsDelivered { get; private set; }

    private GameObject cargo;
    private Vector3 targetDirection, targetPosition;
    private Vector3 translation;
    private Vector3 beginPosition;
    private float minHeight, maxHeight;
    private bool isDelivering, isLeaving;
    private float beginDistance;
    private bool slow;

    private const float DELTA = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = this.transform.position - targetPosition;
        float currDistance = delta.magnitude;

        if (currDistance < beginDistance * 0.2f && !slow)
        {
            speed /= 2.0f;
            slow = true;
        }
        else if (currDistance > beginDistance * 0.2f && slow)
        {
            speed *= 2.0f;
            slow = false;
        }

        if (currDistance < DELTA)
        {
            if (isDelivering)
            {
                isDelivering = false;               
                StartCoroutine(CargoDrop());
            }
            if (isLeaving)
            {
                isLeaving = false;
                Destroy(this.gameObject);
            }
        }

        if (isDelivering)
            Translate(translation, speed);
        else if (isLeaving)
            Translate(translation, speed, false);
    }

    public void Init(Vector3 cargoPlatformPosition)
    {
        minHeight = targetHeight * minHeightRange;
        maxHeight = targetHeight * maxHeightRange;

        float distance = Random.Range(minDistance, maxDistance);
        float height = Random.Range(minHeight, maxHeight);
        float angle = Random.Range(0.0f, 360.0f);

        this.gameObject.transform.Rotate(new Vector3(0, angle, 0));
        this.gameObject.transform.position = new Vector3();
        this.gameObject.transform.Translate(0, height, distance);
        this.gameObject.transform.position += cargoPlatformPosition;
        targetDirection = new Vector3(cargoPlatformPosition.x, height + cargoPlatformPosition.y, cargoPlatformPosition.z);
        this.gameObject.transform.LookAt(targetDirection);

        targetPosition = new Vector3(cargoPlatformPosition.x, targetHeight + cargoPlatformPosition.y, cargoPlatformPosition.z);
        translation = targetPosition - this.gameObject.transform.position;        
        beginPosition = this.gameObject.transform.position;
        beginDistance = translation.magnitude;
        translation = translation.normalized;

        InitCargo();

        isDelivering = true;
        slow = false;
        CargoIsDelivered = false;
    }

    private void InitCargo()
    {
        cargo = Instantiate(cargoPrefab);
        cargo.transform.rotation = this.transform.rotation;
        cargo.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - targetHeight, this.transform.position.z);
    }

    private void Translate(Vector3 translation, float speed, bool withCargo = true, bool back = false)
    {
        translation *= (back ? -1f : 1f);
        this.gameObject.transform.Translate(translation * speed * Time.deltaTime, Space.World);
        if (withCargo)
            cargo.transform.Translate(translation * speed * Time.deltaTime, Space.World);
    }

    private IEnumerator CargoDrop()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(cargo.gameObject);
        CargoIsDelivered = true;
        translation = new Vector3(translation.x, -translation.y, translation.z);
        targetPosition = new Vector3(-beginPosition.x, beginPosition.y, -beginPosition.z);
        isLeaving = true;
    }
}
