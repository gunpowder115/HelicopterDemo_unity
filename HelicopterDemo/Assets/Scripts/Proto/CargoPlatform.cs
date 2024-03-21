using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoPlatform : MonoBehaviour
{
    [SerializeField] private GameObject cargoHelicopterPrefab;
    [SerializeField] private GameObject cargoPrefab;
    [SerializeField] private CargoType cargoType = CargoType.helicopter;

    private GameObject cargoHelicopterItem;
    private GameObject cargoItem;
    private CargoHelicopter cargoHelicopter;
    private HelicopterAI helicopter;
    private CargoPlatformState cargoPlatformState;

    // Start is called before the first frame update
    void Start()
    {
        cargoPlatformState = CargoPlatformState.cargoIsLost;
    }

    // Update is called once per frame
    void Update()
    {
        switch(cargoPlatformState)
        {
            case CargoPlatformState.cargoIsOk:
                if (cargoItem.gameObject == null)
                    cargoPlatformState = CargoPlatformState.cargoIsLost;
                break;
            case CargoPlatformState.cargoIsLost:
                cargoHelicopterItem = Instantiate(cargoHelicopterPrefab);
                cargoHelicopter = cargoHelicopterItem.GetComponent<CargoHelicopter>();
                cargoHelicopter.Init(this.gameObject.transform.position);
                cargoPlatformState = CargoPlatformState.cargoIsExpected;
                break;
            case CargoPlatformState.cargoIsExpected:
                if (cargoHelicopter.CargoIsDelivered)
                    cargoPlatformState = CargoPlatformState.cargoDelivered;
                break;
            case CargoPlatformState.cargoDelivered:
                cargoItem = Instantiate(cargoPrefab, this.gameObject.transform.position, cargoHelicopterItem.transform.rotation);
                helicopter = cargoItem.GetComponent<HelicopterAI>();
                helicopter.StartFlight();
                cargoPlatformState = CargoPlatformState.cargoIsOk;
                //StartCoroutine(DestroyHelicopter());
                break;
        }
    }

    private void CallForCargo(CargoType cargoType)
    {

    }

    private IEnumerator DestroyHelicopter()
    {
        yield return new WaitForSeconds(5);
        Destroy(cargoItem.gameObject);
    }

    public enum CargoPlatformState
    {
        cargoIsOk,
        cargoIsLost,
        cargoIsExpected,
        cargoDelivered
    }

    public enum CargoType
    {
        helicopter,
        tank,
        nuclearBomb
    }
}
