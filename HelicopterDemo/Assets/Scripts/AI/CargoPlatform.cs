using UnityEngine;
using static CargoHelicopter;

[RequireComponent(typeof(Building))]

public class CargoPlatform : MonoBehaviour
{
    [SerializeField] private GameObject cargoHelicopterPrefab;
    [SerializeField] private GameObject cargoPrefab;
    [SerializeField] private CargoType cargoType = CargoType.Air;

    private Building building;
    private GameObject cargoHelicopterObject;
    private GameObject cargoObject;
    private CargoHelicopter cargoHelicopter;
    private HelicopterAI helicopter;
    private NpcController npcController;
    private CargoState cargoState;

    private void Awake()
    {
        building = GetComponent<Building>();
        npcController = NpcController.singleton;
        cargoState = CargoState.Lost;
    }

    // Update is called once per frame
    void Update()
    {
        SimpleCargoCall();
    }

    private void SimpleCargoCall()
    {
        switch (cargoState)
        {
            case CargoState.Works:
                if (cargoObject.gameObject == null)
                    cargoState = CargoState.Lost;
                break;
            case CargoState.Lost:
                cargoObject = Instantiate(cargoPrefab, gameObject.transform.position, gameObject.transform.rotation);
                cargoState = CargoState.Works;

                if (!cargoObject.GetComponent<NpcSquad>())
                    npcController.Add(cargoObject);
                CargoItem cargoItemComp = cargoObject.GetComponent<CargoItem>();
                if (cargoItemComp)
                    cargoItemComp.SetBuilding(building);

                cargoHelicopterObject = Instantiate(cargoHelicopterPrefab);
                cargoHelicopter = cargoHelicopterObject.GetComponent<CargoHelicopter>();
                cargoHelicopter.Init(gameObject.transform.position, transform.position.y + cargoItemComp.DropHeight + cargoItemComp.ParachuteHeight, CargoFlightType.Horizontal);
                break;
        }
    }

    private void CargoCall()
    {
        switch (cargoState)
        {
            case CargoState.Works:
                if (cargoObject.gameObject == null)
                    cargoState = CargoState.Lost;
                break;
            case CargoState.Lost:
                cargoHelicopterObject = Instantiate(cargoHelicopterPrefab);
                cargoHelicopter = cargoHelicopterObject.GetComponent<CargoHelicopter>();
                cargoHelicopter.Init(this.gameObject.transform.position);
                cargoState = CargoState.Expecting;
                break;
            case CargoState.Expecting:
                if (cargoHelicopter.CargoIsDelivered)
                    cargoState = CargoState.Delivered;
                break;
            case CargoState.Delivered:
                cargoObject = Instantiate(cargoPrefab, this.gameObject.transform.position, cargoHelicopterObject.transform.rotation);
                helicopter = cargoObject.GetComponent<HelicopterAI>();
                if (helicopter)
                    helicopter.StartFlight();
                cargoState = CargoState.Works;
                //StartCoroutine(DestroyHelicopter());
                break;
        }
    }

    public enum CargoState
    {
        Works,
        Lost,
        Expecting,
        Delivered
    }

    public enum CargoType
    {
        Air,
        Ground,
        Drop
    }
}
