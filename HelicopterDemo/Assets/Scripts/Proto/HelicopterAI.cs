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
    private Vector3 prevPosition;
    private float distance;
    private bool flight;

    private const float DELTA = 1f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(this.gameObject.transform.position);
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
                    float currDistance = (this.transform.position - prevPosition).magnitude;
                    bool inBorder = this.transform.position.x < absBorderX &&
                                    this.transform.position.x > -absBorderX &&
                                    this.transform.position.z < absBorderZ &&
                                    this.transform.position.z > -absBorderZ;
                    //inBorder checking not working correctly yet
                    if (currDistance > distance/* || !inBorder*/)
                        SelectTranslation();
                    this.gameObject.transform.Translate(translation * speed * Time.deltaTime);
                    break;
            }
        }
    }

    public void StartFlight()
    {
        flightPhase = FlightPhase.Takeoff;
        prevPosition = this.gameObject.transform.position;
        flight = true;
    }

    private void SelectTranslation()
    {
        float angle = Random.Range(-110f, 110f);
        this.gameObject.transform.Rotate(new Vector3(0, angle, 0));

        prevPosition = this.gameObject.transform.position;

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
