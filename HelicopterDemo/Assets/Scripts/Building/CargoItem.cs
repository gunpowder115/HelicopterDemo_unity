using System;
using UnityEngine;

public class CargoItem : MonoBehaviour
{
    [SerializeField] private float deliverySpeed = 5f;
    [SerializeField] private GameObject parachutePrefab;

    public float DeliverySpeed => deliverySpeed;
    public float DropHeight => CargoPlatform.DropHeight;
    public float ParachuteHeight => CargoPlatform.ParachuteHeight;
    public GameObject ParachutePrefab => parachutePrefab;
    public Building Building { get; private set; }
    public BaseCenter BaseCenter => Building.BaseCenter;
    public CargoPlatform CargoPlatform { get; private set; }
    public Action InitCargoItem { get; set;  }

    public void Init(Building building)
    {
        Building = building;
        CargoPlatform = Building.gameObject.GetComponent<CargoPlatform>();
        InitCargoItem?.Invoke();
    }
}
