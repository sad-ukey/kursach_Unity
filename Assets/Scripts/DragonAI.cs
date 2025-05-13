using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DragonAI : MonoBehaviour, damageable
{
    public float moveSpeed = 6f;
    public float health = 100f;
    public float attackDamage = 30f;

    private NavMeshAgent agent;
    private Transform currentTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        if (!agent.isOnNavMesh)
        {
            Debug.LogError("Дракон не находится на NavMesh!");
            enabled = false;
            return;
        }

        FindTarget();
    }

    void Update()
    {
        if (currentTarget == null)
        {
            FindTarget();
            return;
        }

        if (agent == null || !agent.isOnNavMesh)
        {
            Debug.LogWarning("NavMeshAgent не готов или дракон не стоит на NavMesh");
            return;
        }

        agent.SetDestination(currentTarget.position);

        if (Vector3.Distance(transform.position, currentTarget.position) <= 5f)
        {
            Attack();
        }
    }

    void FindTarget()
    {
        Building[] buildings = GameObject.FindObjectsOfType<Building>();
        float closestDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (var building in buildings)
        {
            if (building.type == Building.BuildingType.Fence) continue;

            float dist = Vector3.Distance(transform.position, building.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = building.transform;
            }
        }

        currentTarget = closest;

        if (currentTarget == null)
        {
            Debug.LogWarning("Дракон не нашёл подходящей цели.");
        }
    }

    void Attack()
    {
        if (currentTarget == null) return;

        Building building = currentTarget.GetComponent<Building>();
        if (building != null)
        {
            building.TakeDamage(attackDamage);
            Debug.Log("Дракон атакует " + building.type);
        }
        else
        {
            Debug.Log("Цель уничтожена или невалидна. Поиск новой цели.");
            FindTarget();
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log("Дракон получил " + amount + " урона. Осталось: " + health);

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Дракон погиб.");
        Destroy(gameObject);
    }
}