using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieAI : MonoBehaviour, damageable
{
    [Header("Характеристики Зомби")]
    public float moveSpeed = 3.5f;
    public float health = 100f;
    public float attackDamage = 15f;

    public float attackCooldown = 1.5f; 
    private float nextAttackTime = 0f;  

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

        float distance = Vector3.Distance(transform.position, currentTarget.position);
        if (distance <= 2f && Time.time >= nextAttackTime)
        {
            Attack();
        }
    }

    void FindTarget()
    {
        Building[] allBuildings = FindObjectsOfType<Building>();
        float closestDistance = Mathf.Infinity;
        Building bestTarget = null;

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
            Debug.Log("Зомби атакует " + building.type);

            nextAttackTime = Time.time + attackCooldown;

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
        AchievementManager.Instance.IncrementProgress("Охотник за головами", 1);
        Debug.Log("Зомби уничтожен");
        Destroy(gameObject);
    }
}