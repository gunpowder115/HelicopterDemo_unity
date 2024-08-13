using UnityEngine;

public class Building : MonoBehaviour
{
    public bool IsUnderAttack { get; } //todo

    public BaseCenter BaseCenter => Platform.BaseCenter;
    public Platform Platform { get; private set; }

    public void SetPlatform(Platform platform) => Platform = platform;
}
