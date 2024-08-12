using UnityEngine;

[RequireComponent(typeof(InitialBuilder))]

public class BaseCenter : MonoBehaviour
{
    private InitialBuilder initialBuilder;

    public bool HasPrimaryProtection { get; } //todo
    public bool HasSecondaryProtection { get; } //todo
    public bool IsUnderAttack { get; } //todo
    public CargoItem Protection { get; } //todo
    public Platform[] Platforms { get; private set; }
    public GameObject[] Buildings { get; private set; }
    public CargoItem[] CargoItems { get; private set; }

    private void Awake()
    {
        initialBuilder = GetComponent<InitialBuilder>();
        Platforms = initialBuilder.GetPlatforms(this);
        Buildings = initialBuilder.InitBuildings(Platforms);
    }
}
