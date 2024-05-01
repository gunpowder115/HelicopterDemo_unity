using UnityEngine;

public class TargetSelectionInput : MonoBehaviour
{
    [SerializeField] float aimSpeed = 5f;

    public Vector2 ToTargetSelection => toTargetSelection;

    private GameObject aimItem;
    private Camera camera;
    private CameraRotation cameraRotation;
    private Vector2 toTargetSelection;

    private void Start()
    {
        camera = GetComponent<Camera>();
        cameraRotation = GetComponent<CameraRotation>();
        aimItem = GameObject.FindGameObjectWithTag("Aim");
        if (aimItem) aimItem.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        camera = GetComponent<Camera>();
    }

    public void ShowAim()
    {
        if (aimItem)
        {
            aimItem.SetActive(true);
            aimItem.transform.position = new Vector3(camera.pixelWidth / 2f, camera.pixelHeight / 2f, 0f);
        }
    }

    public void HideAim()
    {
        if (aimItem) aimItem.SetActive(false);
    }

    public void MoveAim(Vector2 direction)
    {
        if (aimItem && cameraRotation)
        {
            float aimX, aimY;
            aimX = Limit(aimItem.transform.position.x + direction.x * aimSpeed, 0f, camera.pixelWidth);
            aimY = Limit(aimItem.transform.position.y + direction.y * aimSpeed, 0f, camera.pixelHeight);
            aimItem.transform.position = new Vector3(aimX, aimY, 0f);

            float cameraCoefX = 2f * aimX / camera.pixelWidth - 1f;
            float cameraCoefY = 2f * aimY / camera.pixelHeight - 1f;
            toTargetSelection = new Vector2(cameraCoefX, -cameraCoefY);
        }
    }

    private float Limit(float value, float min, float max)
    {
        if (value <= min) return min;
        else if (value >= max) return max;
        else return value;
    }
}
