using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float baseHealth = 100f;

    float health;
    NpcController npcController;
    Player player;
    SimpleNpc npc;

    public bool IsAlive { get; private set; }

    public void Hurt(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            IsAlive = false;
            if (npc) DestroyNpc();
        }
        if (player) Debug.Log(health);
    }

    public void SetAlive(bool isAlive)
    {
        IsAlive = isAlive;
        health = isAlive ? baseHealth : 0f;
    }

    void Start()
    {
        IsAlive = true;
        health = baseHealth;
        npcController = NpcController.singleton;
        player = GetComponent<Player>();
        npc = GetComponent<SimpleNpc>();
    }

    void DestroyNpc()
    {
        npcController.Remove(gameObject);
        Destroy(gameObject);
    }
}
