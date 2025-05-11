using UnityEngine;

public class Building : MonoBehaviour
{
    public enum BuildingType { Defensive, Economic, TownHall, Fence }

    public BuildingType type;
    public float health = 100f;
    public bool isBlockingPath = false;

    // Делаем метод виртуальным для переопределения
    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}