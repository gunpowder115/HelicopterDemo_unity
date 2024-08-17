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
        if (!FriendlyFire(other.gameObject.tag) && health)
            health.Hurt(damage, other.GetComponent<Npc>());

        Destroy(gameObject);
    }

    private bool FriendlyFire(string anotherTag) //todo remove tags
    {
        string thisTag = gameObject.tag;
        bool isPlayer = thisTag == "Player" || anotherTag == "Player";
        bool isFriendly = thisTag.Contains("Friendly") || anotherTag.Contains("Friendly");
        bool isEnemy = thisTag.Contains("Enemy") || anotherTag.Contains("Enemy");

        return !((isFriendly && isEnemy) || (isPlayer && isEnemy));
    }
}
