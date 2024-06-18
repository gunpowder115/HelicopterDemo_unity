using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField] float aimSpeed = 5f;
    [SerializeField] float rayRadius = 1f;
    [SerializeField] float maxDistance = 100f;
    [SerializeField] GameObject aimItem;
    [SerializeField] GameObject targetAimItem;

    public static Crosshair singleton { get; private set; }
    public Vector2 ToTargetSelection => toTargetSelection;

    Vector2 toTargetSelection;
    new Camera camera;

    void Awake()
    {
        singleton = this;
    }

    void Start()
    {
        camera = GetComponent<Camera>();
        aimItem.SetActive(false);
        targetAimItem.SetActive(false);
    }

    public void Show()
    {
        if (aimItem)
        {
            aimItem.SetActive(true);
            aimItem.transform.position = new Vector3(camera.pixelWidth / 2f, camera.pixelHeight / 2f, 0f);
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
            aimX = Limit(aimItem.transform.position.x + direction.x * aimSpeed, 0f, camera.pixelWidth);
            aimY = Limit(aimItem.transform.position.y + direction.y * aimSpeed, 0f, camera.pixelHeight);
            aimItem.transform.position = new Vector3(aimX, aimY, 0f);

            float cameraCoefX = 2f * aimX / camera.pixelWidth - 1f;
            float cameraCoefY = 2f * aimY / camera.pixelHeight - 1f;
            toTargetSelection = new Vector2(cameraCoefX, -cameraCoefY);

            Ray ray = camera.ScreenPointToRay(aimItem.transform.position);
            var raycastHits = Physics.SphereCastAll(ray, rayRadius, maxDistance);
            bool hitEnemy = false;
            foreach(var hit in raycastHits)
            {
                var hitObject = hit.transform.gameObject;
                if (hitObject.GetComponent<SimpleNpc>())
                {
                    targetAimItem.SetActive(true);
                    var targetScreenPos = camera.WorldToScreenPoint(hitObject.transform.position);
                    targetAimItem.transform.position = new Vector3(targetScreenPos.x, targetScreenPos.y, targetAimItem.transform.position.z);
                    hitEnemy = true;
                    break;
                }
            }
            if (!hitEnemy) targetAimItem.SetActive(false);
        }
    }

    float Limit(float value, float min, float max)
    {
        if (value <= min) return min;
        else if (value >= max) return max;
        else return value;
    }
}
