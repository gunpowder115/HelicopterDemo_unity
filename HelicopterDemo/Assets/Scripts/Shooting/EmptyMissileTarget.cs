using UnityEngine;

public class EmptyMissileTarget : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float standTime = 1f;
    [SerializeField] private float minDistToTgt = 1f;

    private float currStandTime;

    public GameObject SelectedTarget { get; set; }

    private void Start()
    {
        currStandTime = standTime;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 toSelTarget = SelectedTarget.transform.position - transform.position;

        if (currStandTime <= 0f)
        {
            if (SelectedTarget)
                transform.rotation = Quaternion.LookRotation(toSelTarget.normalized);

            if (toSelTarget.magnitude > minDistToTgt)
                transform.Translate(0f, 0f, speed * Time.deltaTime);
        }
        else
            currStandTime -= Time.deltaTime;
    }
}
