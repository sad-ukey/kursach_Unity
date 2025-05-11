using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GiantAI : MonoBehaviour
{
    // Настройки
    public float moveSpeed = 3f;
    public float attackRange = 2f;
    public float attackDamage = 40f;

    private NavMeshAgent agent;
    private Transform currentTarget;
    private readonly Building.BuildingType[] priorityOrder =
    {
        Building.BuildingType.Defensive, // Главный приоритет
        Building.BuildingType.TownHall,  // Вторичные цели
        Building.BuildingType.Economic
    };

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        FindTarget();
    }

    void Update()
    {
        if (currentTarget == null)
        {
            FindTarget();
            return;
        }

        agent.SetDestination(currentTarget.position);

        if (Vector3.Distance(transform.position, currentTarget.position) <= attackRange)
        {
            Attack();
        }
    }

    void FindTarget()
    {
        currentTarget = PriorityTargetSystem.GetPriorityTarget(transform, priorityOrder);
    }

    void Attack()
    {
        var building = currentTarget.GetComponent<Building>();
        if (building != null)
        {
            building.TakeDamage(attackDamage);
            Debug.Log("Гигант атакует " + building.type);
        }
    }
}