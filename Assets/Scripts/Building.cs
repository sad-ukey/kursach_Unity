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
        Debug.Log($"{type} получил {damage} урона. Осталось здоровья: {health}");

        if (health <= 0)
        {
            var state = GetComponent<BuildingState>();
            if (state != null)
            {
                state.OnDestroyed();
            }

            Destroy(gameObject);
        }
    }

    public bool IsDestroyed()
    {
        return health <= 0;
    }
}