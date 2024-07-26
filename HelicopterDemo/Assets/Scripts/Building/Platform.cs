using UnityEngine;

public class Platform : MonoBehaviour
{
    public BaseCenter BaseCenter { get; private set; }

    public void SetBaseCenter(BaseCenter baseCenter) => BaseCenter = baseCenter;
}
