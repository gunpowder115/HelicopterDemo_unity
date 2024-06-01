using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 10.0f;
    [SerializeField] float lifetime = 10.0f;
    [SerializeField] float damage = 5.0f;

    float currLifetime;

    void Start()
    {
        currLifetime = 0.0f;
    }

    void Update()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);
        currLifetime += Time.deltaTime;

        if (currLifetime >= lifetime)
            Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponent<Health>();
        if (health != null)
            health.Hurt(damage);
        Destroy(gameObject);
    }
}
