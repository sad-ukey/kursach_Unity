using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GoblinAI : MonoBehaviour, IDamageable
{
    // Характеристики
    public float moveSpeed = 5f;
    public float health = 50f;
    public float attackDamage = 20f;

    private NavMeshAgent agent;
    private Transform currentTarget;

    private readonly Building.BuildingType[] priorityOrder =
    {
        Building.BuildingType.Economic,   // Главный приоритет
        Building.BuildingType.TownHall,   // Вторичные цели
        Building.BuildingType.Defensive
    };

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        if (!agent.isOnNavMesh)
        {
            Debug.LogError("Гоблин не находится на NavMesh! Переместите его на навигационную зону.");
            enabled = false; // отключаем скрипт, чтобы избежать ошибок
            return;
        }

        FindTarget();
    }

    void Update()
    {
        // Проверка: если цели нет, пытаемся её найти
        if (currentTarget == null)
        {
            FindTarget();
            return;
        }

        // Проверка: агент активен и на NavMesh
        if (agent == null || !agent.isOnNavMesh)
        {
            Debug.LogWarning("NavMeshAgent не готов или гоблин не стоит на NavMesh");
            return;
        }

        // Движение к цели
        agent.SetDestination(currentTarget.position);

        // Проверка дистанции до цели
        if (Vector3.Distance(transform.position, currentTarget.position) <= 1.5f)
        {
            Attack();
        }
    }

    void FindTarget()
    {
        currentTarget = PriorityTargetSystem.GetPriorityTarget(transform, priorityOrder);

        if (currentTarget == null)
        {
            //Debug.LogWarning("Гоблин не нашёл приоритетную цель.");
        }
    }

    void Attack()
    {
        if (currentTarget == null) return;

        Building building = currentTarget.GetComponent<Building>();
        if (building != null && !building.IsDestroyed())
        {
            building.TakeDamage(attackDamage);
            Debug.Log("Гоблин атакует " + building.type);
        }
        else
        {
            Debug.Log("Цель уничтожена или невалидна. Поиск новой цели.");
            FindTarget(); // Ищем новую цель, если старая недоступна
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log("Гоблин получил " + amount + " урона. Осталось: " + health);

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Гоблин погиб.");
        Destroy(gameObject);
    }
}