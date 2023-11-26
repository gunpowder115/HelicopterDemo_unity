using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterAI : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float verticalSpeed = 0.5f;
    [SerializeField] private float minHeight = 15f;
    [SerializeField] private float maxHeight = 50f;
    [SerializeField] private float minDistance = 15f;
    [SerializeField] private float maxDistance = 50f;

    private FlightPhase flightPhase;
    private Vector3 translation;
    private Vector3 targetPosition;
    private bool flight;

    private const float DELTA = 1f;

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
                    float currDistance = (this.transform.localPosition - targetPosition).magnitude;
                    if (currDistance < DELTA)
                        SelectTranslation();
                    this.gameObject.transform.Translate(translation * speed * Time.deltaTime, Space.World);
                    break;
            }
        }
    }

    public void StartFlight()
    {
        flightPhase = FlightPhase.Takeoff;
        flight = true;        
    }

    private void SelectTranslation()
    {
        float distanceX = Random.Range(minDistance, maxDistance);
        float distanceZ = Random.Range(minDistance, maxDistance);
        float angle = Random.Range(0f, 360f);
        float height = Random.Range(minHeight, maxHeight);

        Vector3 targetDirection = new Vector3(distanceX, this.gameObject.transform.localPosition.y, distanceZ);
        targetDirection = Vector3.ClampMagnitude(targetDirection, maxDistance);
        this.gameObject.transform.LookAt(targetDirection);
        targetPosition = new Vector3(targetDirection.x, height, targetDirection.z);

        translation = (targetPosition - this.gameObject.transform.localPosition).normalized;
    }

    public enum FlightPhase
    {
        Takeoff,
        Flight
    }
}
