using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    [SerializeField] private GameObject lineObjectPrefab;

    private GameObject lineObject;
    private LineRenderer lineRenderer;

    public bool Enabled { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        lineObject = Instantiate(lineObjectPrefab);
        lineRenderer = lineObject.GetComponent<LineRenderer>();
        Enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.enabled = Enabled;
    }

    public void SetPosition(in Vector3 pos1, in Vector3 pos2)
    {
        lineRenderer.SetPosition(0, pos1);
        lineRenderer.SetPosition(1, pos2);
    }

    public void SetColor(in Color col1, in Color col2)
    {
        lineRenderer.startColor = col1;
        lineRenderer.endColor = col2;
    }

    public void SetColor(in Color col) => SetColor(col, col);

    public void SetWidth(float wid1, float wid2)
    {
        lineRenderer.startWidth = wid1;
        lineRenderer.endWidth = wid2;
    }

    public void SetWidth(float wid) => SetWidth(wid, wid);
}
