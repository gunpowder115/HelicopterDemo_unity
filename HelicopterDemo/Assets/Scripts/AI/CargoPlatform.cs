using UnityEngine;

[RequireComponent(typeof(Building))]

public class CargoPlatform : MonoBehaviour
{
    [SerializeField] private GameObject cargoHelicopterPrefab;
    [SerializeField] private GameObject cargoPrefab;
    [SerializeField] private CargoType cargoType = CargoType.Air;

    private Building building;
    private GameObject cargoHelicopterItem;
    private GameObject cargoItem;
    private CargoHelicopter cargoHelicopter;
    private HelicopterAI helicopter;
    private NpcController npcController;
    private CargoState cargoState;

    private void Awake()
    {
        building = GetComponent<Building>();
        npcController = NpcController.singleton;

        cargoItem = Instantiate(cargoPrefab, gameObject.transform.position, gameObject.transform.rotation);
        cargoState = CargoState.Works;
        CargoItem cargoItemComp = cargoItem.GetComponent<CargoItem>();
        if (cargoItemComp)
            cargoItemComp.SetBuilding(building);
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
                if (cargoItem.gameObject == null)
                    cargoState = CargoState.Lost;
                break;
            case CargoState.Lost:
                cargoItem = Instantiate(cargoPrefab, gameObject.transform.position, gameObject.transform.rotation);
                cargoState = CargoState.Works;
                npcController.Add(cargoItem);

                CargoItem cargoItemComp = cargoItem.GetComponent<CargoItem>();
                if (cargoItemComp)
                    cargoItemComp.SetBuilding(building);

                break;
        }
    }

    private void CargoCall()
    {
        switch (cargoState)
        {
            case CargoState.Works:
                if (cargoItem.gameObject == null)
                    cargoState = CargoState.Lost;
                break;
            case CargoState.Lost:
                cargoHelicopterItem = Instantiate(cargoHelicopterPrefab);
                cargoHelicopter = cargoHelicopterItem.GetComponent<CargoHelicopter>();
                cargoHelicopter.Init(this.gameObject.transform.position);
                cargoState = CargoState.Expecting;
                break;
            case CargoState.Expecting:
                if (cargoHelicopter.CargoIsDelivered)
                    cargoState = CargoState.Delivered;
                break;
            case CargoState.Delivered:
                cargoItem = Instantiate(cargoPrefab, this.gameObject.transform.position, cargoHelicopterItem.transform.rotation);
                helicopter = cargoItem.GetComponent<HelicopterAI>();
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
