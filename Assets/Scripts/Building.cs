using UnityEngine;

public class Building : MonoBehaviour
{
    public enum BuildingType { Defensive, Economic, TownHall, Fence }

    public BuildingType type;
    public float health = 100f;
    public bool isBlockingPath = false;

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"{type} ������� {damage} �����. �������� ��������: {health}");

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public bool IsDestroyed()
    {
        return health <= 0;
    }
}