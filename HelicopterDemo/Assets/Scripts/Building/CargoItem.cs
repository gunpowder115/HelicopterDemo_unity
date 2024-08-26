using UnityEngine;

public class CargoItem : MonoBehaviour
{
    [SerializeField] private float deliverySpeed = 5f;
    [SerializeField] private float dropHeight = 25f;
    [SerializeField] private float parachuteHeight = 10f;
    [SerializeField] private GameObject parachutePrefab;

    public float DeliverySpeed => deliverySpeed;
    public float DropHeight => dropHeight;
    public float ParachuteHeight => parachuteHeight;
    public GameObject ParachutePrefab => parachutePrefab;
    public Building Building { get; private set; }
    public BaseCenter BaseCenter => Building.BaseCenter;

    public void SetBuilding(Building building) => Building = building;
}
