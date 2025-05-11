using UnityEngine;

public class DefendHealth : MonoBehaviour
{
    [Header("Настройки")]
    public float health = 200f;
    public GameObject destroyEffect;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            DestroyBuilding();
        }
    }

    void DestroyBuilding()
    {
        // Эффект разрушения
        if (destroyEffect != null)
            Instantiate(destroyEffect, transform.position, Quaternion.identity);

        // Уничтожение объекта
        Destroy(gameObject);
    }
}