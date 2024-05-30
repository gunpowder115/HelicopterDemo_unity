using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 10.0f;
    [SerializeField] float lifetime = 10.0f;
    [SerializeField] float damage = 5.0f;

    float currLifetime;

    // Start is called before the first frame update
    void Start()
    {
        currLifetime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);
        currLifetime += Time.deltaTime;

        if (currLifetime >= lifetime)
            Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        SimpleEnemy enemy = other.GetComponent<SimpleEnemy>();
        if (player != null)
            player.Hurt(damage);
        else if (enemy != null)
            enemy.Hurt(damage);
        Destroy(gameObject);
    }
}
