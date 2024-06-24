using UnityEngine;

public class GuidedMissile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 10f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private float minDistToEmpty = 1f;
    [SerializeField] private GameObject emptyMissleTargetPrefab;

    private bool isEmptyTarget;
    private GameObject emptyMissileTargetItem;

    public GameObject SelectedTarget { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        emptyMissileTargetItem = Instantiate(emptyMissleTargetPrefab);
        emptyMissileTargetItem.transform.position = transform.position;
        emptyMissileTargetItem.transform.rotation = transform.rotation;

        float distToTgt = (SelectedTarget.transform.position - transform.position).magnitude;
        emptyMissileTargetItem.transform.Translate(0f, 0f, distToTgt, Space.Self);

        EmptyMissileTarget emptyMissileTarget = emptyMissileTargetItem.GetComponent<EmptyMissileTarget>();
        if (emptyMissileTarget)
            emptyMissileTarget.SelectedTarget = SelectedTarget;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 toEmptyTarget = emptyMissileTargetItem.transform.position - transform.position;
        if (toEmptyTarget.magnitude <= minDistToEmpty && !isEmptyTarget)
            isEmptyTarget = true;

        if (!isEmptyTarget)
            transform.rotation = Quaternion.LookRotation((emptyMissileTargetItem.transform.position - transform.position).normalized);

        transform.Translate(0f, 0f, speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponent<Health>();
        if (health != null)
            health.Hurt(damage);
        Destroy(gameObject);
        Destroy(emptyMissileTargetItem.gameObject);
    }
}