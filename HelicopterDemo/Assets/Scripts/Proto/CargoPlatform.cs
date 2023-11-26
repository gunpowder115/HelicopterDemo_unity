using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoPlatform : MonoBehaviour
{
    [SerializeField] private GameObject cargoHelicopterPrefab;
    [SerializeField] private GameObject helicopterPrefab;
    [SerializeField] private CargoType cargoType = CargoType.helicopter;

    private GameObject cargoHelicopterItem;
    private GameObject helicopterItem;
    private CargoHelicopter cargoHelicopter;
    private CargoPlatformState cargoPlatformState;

    // Start is called before the first frame update
    void Start()
    {
        cargoPlatformState = CargoPlatformState.helicopterIsLost;
    }

    // Update is called once per frame
    void Update()
    {
        switch(cargoPlatformState)
        {
            case CargoPlatformState.helicopterIsOk:
                if (helicopterItem.gameObject == null)
                    cargoPlatformState = CargoPlatformState.helicopterIsLost;
                break;
            case CargoPlatformState.helicopterIsLost:
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
                helicopterItem = Instantiate(helicopterPrefab);
                cargoPlatformState = CargoPlatformState.helicopterIsOk;
                StartCoroutine(DestroyHelicopter());
                break;
        }
    }

    private void CallForCargo(CargoType cargoType)
    {

    }

    private IEnumerator DestroyHelicopter()
    {
        yield return new WaitForSeconds(5);
        Destroy(helicopterItem.gameObject);
    }

    public enum CargoPlatformState
    {
        helicopterIsOk,
        helicopterIsLost,
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
