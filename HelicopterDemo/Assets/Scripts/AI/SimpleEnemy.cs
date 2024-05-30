using System.Collections;
using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    float health;
    NpcController npcController;

    public void Hurt(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            StartCoroutine(Die());
        }
    }

    void Start()
    {
        health = 100f;
        npcController = NpcController.singleton;
    }

    IEnumerator Die()
    {
        transform.Rotate(-75, 0, 0);

        yield return new WaitForSeconds(1.5f);

        npcController.Remove(gameObject);
        Destroy(gameObject);
    }
}
