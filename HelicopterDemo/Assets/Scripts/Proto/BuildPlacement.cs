using UnityEngine;

public class BuildPlacement : MonoBehaviour
{
    private bool placement;

    // Start is called before the first frame update
    void Start()
    {
        placement = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!placement)
        {
            this.gameObject.transform.position = this.gameObject.transform.parent.position;
            this.gameObject.transform.rotation = this.gameObject.transform.parent.rotation;
            this.gameObject.transform.localScale = new Vector3(
                1 / this.gameObject.transform.parent.localScale.x,
                1 / this.gameObject.transform.parent.localScale.y,
                1 / this.gameObject.transform.parent.localScale.z);

            placement = true;
        }
    }
}
