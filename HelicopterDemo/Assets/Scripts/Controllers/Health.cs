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
            if (player) RespawnPlayer();
            else if (npc) DestroyNpc();
        }
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

    void RespawnPlayer()
    {
        IsAlive = true;
        health = baseHealth;
        transform.position = new Vector3(0, 10, 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
