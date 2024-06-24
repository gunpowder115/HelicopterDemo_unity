using UnityEngine;

public class EmptyMissileTarget : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float standTime = 1f;

    private float currStandTime;

    public GameObject SelectedTarget { get; set; }

    private void Start()
    {
        currStandTime = standTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (SelectedTarget && (currStandTime -= Time.deltaTime) <= 0f)
        {
            Vector3 toSelTarget = SelectedTarget.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(toSelTarget.normalized);
        }
        transform.Translate(0f, 0f, speed * Time.deltaTime);
    }
}
