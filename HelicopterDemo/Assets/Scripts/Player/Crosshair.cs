using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private float aimSpeed = 5f;
    [SerializeField] private float rayRadius = 1f;
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private float targetHoldTime = 1f;
    [SerializeField] private GameObject aimItem;
    [SerializeField] private GameObject targetAimItem;

    public Vector2 ToTargetSelection => toTargetSelection;
    public Vector3 HitPoint { get; private set; }
    public GameObject SelectedTarget { get; private set; }
    public static Crosshair singleton { get; private set; }

    private float currHoldTime;
    private Vector2 toTargetSelection, targetScreenPos;
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
        aimItem.SetActive(true);
        aimItem.transform.position = new Vector3(mainCamera.pixelWidth / 2f, mainCamera.pixelHeight / 2f, 0f);
    }

    public void Hide()
    {
        aimItem.SetActive(false);
        targetAimItem.SetActive(false);
        SelectedTarget = null;
        currHoldTime = 0f;
    }

    public void Translate(Vector2 direction)
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

    private void SetTargetAim()
    {
        Ray ray = mainCamera.ScreenPointToRay(aimItem.transform.position);
        var raycastHits = Physics.SphereCastAll(ray, rayRadius, maxDistance);
        bool hitEnemy = false;
        for (int i = 0; i < raycastHits.Length; i++)
        {
            if (i == 0) HitPoint = raycastHits[i].point;
            var hitObject = raycastHits[i].transform.gameObject;
            if (hitObject.GetComponent<Npc>())
            {
                targetScreenPos = mainCamera.WorldToScreenPoint(hitObject.transform.position);
                Ray rayCenter = mainCamera.ScreenPointToRay(targetScreenPos);
                Physics.Raycast(rayCenter, out RaycastHit hitCenter);
                var hitCenterObject = hitCenter.transform.gameObject;
                if (hitObject == hitCenterObject)
                {
                    currHoldTime = targetHoldTime;
                    SelectedTarget = hitObject;
                    HitPoint = hitObject.transform.position;
                    hitEnemy = true;
                    break;
                }
            }
        }

        if (!hitEnemy)
        {
            if ((currHoldTime -= Time.deltaTime) > 0f && SelectedTarget)
            {
                HitPoint = SelectedTarget.transform.position;
                targetScreenPos = mainCamera.WorldToScreenPoint(SelectedTarget.transform.position);
                targetAimItem.transform.position = new Vector3(targetScreenPos.x, targetScreenPos.y, targetAimItem.transform.position.z);
                targetAimItem.SetActive(true);
            }
            else
            {
                SelectedTarget = null;
                targetAimItem.SetActive(false);
            }
        }
        else
        {
            targetAimItem.transform.position = new Vector3(targetScreenPos.x, targetScreenPos.y, targetAimItem.transform.position.z);
            targetAimItem.SetActive(true);
        }
    }

    private float Limit(float value, float min, float max)
    {
        if (value <= min) return min;
        else if (value >= max) return max;
        else return value;
    }
}
