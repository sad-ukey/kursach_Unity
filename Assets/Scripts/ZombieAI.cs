using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieAI : MonoBehaviour, IDamageable
{
    [Header("�������������� �����")]
    public float moveSpeed = 3.5f;
    public float health = 100f;
    public float attackDamage = 15f;

    private NavMeshAgent agent;
    private Transform currentTarget;

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
        Building[] allBuildings = FindObjectsOfType<Building>();
        float closestDistance = Mathf.Infinity;
        Building bestTarget = null;

        // 1. ���� ��������� ��������� ���� (����� �������)
        foreach (var building in allBuildings)
        {
            if (building.type == Building.BuildingType.Fence) continue;
            if (!IsPathReachable(building.transform.position)) continue;

            float distance = Vector3.Distance(transform.position, building.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                bestTarget = building;
            }
        }

        // 2. ���� ������ ��� � ���� �����, ������� ��������� ����
        if (bestTarget == null)
        {
            foreach (var building in allBuildings)
            {
                if (building.type != Building.BuildingType.Fence || !building.isBlockingPath) continue;
                if (!IsPathReachable(building.transform.position)) continue;

                float distance = Vector3.Distance(transform.position, building.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestTarget = building;
                }
            }
        }

        if (bestTarget != null)
        {
            currentTarget = bestTarget.transform;
        }
    }

    bool IsPathReachable(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(targetPosition, path))
        {
            return path.status == NavMeshPathStatus.PathComplete;
        }
        return false;
    }

    void Attack()
    {
        if (currentTarget == null) return;

        Building building = currentTarget.GetComponent<Building>();
        if (building != null)
        {
            building.TakeDamage(attackDamage);
            Debug.Log("����� ������� " + building.type);

            if (building.IsDestroyed())
            {
                currentTarget = null;
            }
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("����� ���������");
        Destroy(gameObject);
    }
}