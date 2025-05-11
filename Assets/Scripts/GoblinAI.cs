using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GoblinAI : MonoBehaviour
{
    // ��������������
    public float moveSpeed = 5f;
    public float health = 50f;
    public float attackDamage = 20f;

    private NavMeshAgent agent;
    private Transform currentTarget;
    private readonly Building.BuildingType[] priorityOrder =
    {
        Building.BuildingType.Economic,  // ������� ���������
        Building.BuildingType.TownHall, // ��������� ����
        Building.BuildingType.Defensive
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

        if (Vector3.Distance(transform.position, currentTarget.position) <= 1.5f)
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
            Debug.Log("������ ������ " + building.type);
        }
    }
}