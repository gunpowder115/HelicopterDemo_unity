using UnityEngine;

public class CargoItem : MonoBehaviour
{
    public Building Building { get; private set; }
    public BaseCenter BaseCenter => Building.BaseCenter;

    public void SetBuilding(Building building) => Building = building;
}
