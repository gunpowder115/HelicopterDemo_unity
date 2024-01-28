using UnityEngine;

public class HelicopterAI : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float verticalSpeed = 0.5f;
    [SerializeField] private float minHeight = 15f;
    [SerializeField] private float maxHeight = 50f;
    [SerializeField] private float minDistance = 15f;
    [SerializeField] private float maxDistance = 50f;
    [SerializeField] private float absBorderX = 200f;
    [SerializeField] private float absBorderZ = 200f;

    private FlightPhase flightPhase;
    private Vector3 translation;
    private float distance;
    private bool flight;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (flight)
        {
            switch (flightPhase)
            {
                case FlightPhase.Takeoff:
                    if (this.gameObject.transform.position.y < minHeight)
                        this.gameObject.transform.Translate(Vector3.up * verticalSpeed * Time.deltaTime);
                    else
                    {
                        SelectTranslation();
                        flightPhase = FlightPhase.Flight;
                    }
                    break;
                case FlightPhase.Flight:
                    this.gameObject.transform.Translate(translation * speed * Time.deltaTime);

                    if (this.gameObject.transform.position.y >= maxHeight ||
                        this.gameObject.transform.position.y <= minHeight)
                        SelectHeight();

                    Ray ray = new Ray(this.gameObject.transform.position, this.gameObject.transform.forward);
                    RaycastHit hit;
                    if (Physics.SphereCast(ray, 5.0f, out hit))
                    {
                        GameObject hitObject = hit.transform.gameObject;
                        if (hitObject.CompareTag("Obstacle") && hit.distance < 15.0f)
                            SelectTranslation();
                    }
                    break;
            }
        }
    }

    public void StartFlight(Transform platformTransform)
    {
        this.gameObject.transform.position = platformTransform.position;
        this.gameObject.transform.rotation = platformTransform.rotation;
        flightPhase = FlightPhase.Takeoff;
        flight = true;
    }

    private void SelectTranslation()
    {
        float angle = Random.Range(-110, 110);
        this.gameObject.transform.Rotate(new Vector3(0, angle, 0));

        distance = Random.Range(minDistance, maxDistance);
        float deltaHeight = Random.Range(minHeight, maxHeight) - this.gameObject.transform.position.y;
        float vertSpeed = deltaHeight / distance * speed;
        Vector3 targetDirection = speed * Vector3.forward + vertSpeed * Vector3.up;
        translation = targetDirection.normalized;
    }

    private void SelectHeight()
    {
        distance = Random.Range(minDistance, maxDistance);
        float deltaHeight = Random.Range(minHeight, maxHeight) - this.gameObject.transform.position.y;
        float vertSpeed = deltaHeight / distance * speed;
        Vector3 targetDirection = speed * Vector3.forward + vertSpeed * Vector3.up;
        translation = targetDirection.normalized;
    }

    public enum FlightPhase
    {
        Takeoff,
        Flight
    }
}
