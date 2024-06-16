using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float baseHealth = 100f;

    private float health;
    private NpcController npcController;
    private SimpleNpc npc;

    public bool IsAlive { get; private set; }

    public void Hurt(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            IsAlive = false;
            if (npc) DestroyNpc();
        }
    }

    public void SetAlive(bool isAlive)
    {
        IsAlive = isAlive;
        health = isAlive ? baseHealth : 0f;
    }

    private void Start()
    {
        IsAlive = true;
        health = baseHealth;
        npcController = NpcController.singleton;
        npc = GetComponent<SimpleNpc>();
    }

    private void DestroyNpc()
    {
        npcController.Remove(gameObject);
        Destroy(gameObject);
    }
}
