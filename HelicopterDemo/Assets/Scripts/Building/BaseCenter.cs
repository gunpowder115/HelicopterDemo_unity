using UnityEngine;

[RequireComponent(typeof(InitialBuilder))]

public class BaseCenter : MonoBehaviour
{
    private InitialBuilder initialBuilder;

    public bool HasPrimaryProtection => true; //todo
    public bool HasSecondaryProtection => true; //todo
    public bool IsUnderAttack
    {
        get
        {
            foreach (var build in Buildings)
            {
                var buildItem = build.GetComponent<Building>();
                if (buildItem && buildItem.IsUnderAttack)
                    return true;
            }
            return false;
        }
    }
    public CargoItem Protection => null; //todo
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
