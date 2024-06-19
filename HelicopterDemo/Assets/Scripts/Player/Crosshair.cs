using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private float aimSpeed = 5f;
    [SerializeField] private float rayRadius = 1f;
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private GameObject aimItem;
    [SerializeField] private GameObject targetAimItem;

    public static Crosshair singleton { get; private set; }
    public Vector2 ToTargetSelection => toTargetSelection;

    private Vector2 toTargetSelection;
    private Camera mainCamera;

    void Awake()
    {
        singleton = this;
    }

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        aimItem.SetActive(false);
        targetAimItem.SetActive(false);
    }

    public void Show()
    {
        if (aimItem)
        {
            aimItem.SetActive(true);
            aimItem.transform.position = new Vector3(mainCamera.pixelWidth / 2f, mainCamera.pixelHeight / 2f, 0f);
        }
    }

    public void Hide()
    {
        aimItem.SetActive(false);
        targetAimItem.SetActive(false);
    }

    public void Translate(Vector2 direction)
    {
        if (aimItem)
        {
            float aimX, aimY;
            aimX = Limit(aimItem.transform.position.x + direction.x * aimSpeed, 0f, mainCamera.pixelWidth);
            aimY = Limit(aimItem.transform.position.y + direction.y * aimSpeed, 0f, mainCamera.pixelHeight);
            aimItem.transform.position = new Vector3(aimX, aimY, 0f);

            float cameraCoefX = 2f * aimX / mainCamera.pixelWidth - 1f;
            float cameraCoefY = 2f * aimY / mainCamera.pixelHeight - 1f;
            toTargetSelection = new Vector2(cameraCoefX, -cameraCoefY);

            SetTargetAim();
        }
    }

    private void SetTargetAim()
    {
        Ray ray = mainCamera.ScreenPointToRay(aimItem.transform.position);
        var raycastHits = Physics.SphereCastAll(ray, rayRadius, maxDistance);
        bool hitEnemy = false;
        foreach (var hit in raycastHits)
        {
            var hitObject = hit.transform.gameObject;
            if (hitObject.GetComponent<SimpleNpc>())
            {
                var screenPos = mainCamera.WorldToScreenPoint(hitObject.transform.position);
                Ray rayCenter = mainCamera.ScreenPointToRay(screenPos);
                Physics.Raycast(rayCenter, out RaycastHit hitCenter);
                var hitCenterObject = hitCenter.transform.gameObject;
                if (hitObject == hitCenterObject)
                {
                    targetAimItem.SetActive(true);
                    targetAimItem.transform.position = new Vector3(screenPos.x, screenPos.y, targetAimItem.transform.position.z);
                    hitEnemy = true;
                    break;
                }
            }
        }
        if (!hitEnemy) targetAimItem.SetActive(false);
    }

    private float Limit(float value, float min, float max)
    {
        if (value <= min) return min;
        else if (value >= max) return max;
        else return value;
    }
}
