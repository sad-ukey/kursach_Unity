using UnityEngine;
using UnityEngine.AI;
using System.Linq; // Добавляем это пространство имен для LINQ

[RequireComponent(typeof(NavMeshAgent))]
public class BombSkeletonAI : MonoBehaviour, damageable
{
    [Header("Settings")]
    public float moveSpeed = 2f;
    public float explosionDamage = 80f;
    public float explosionRadius = 3f;
    public float health = 50f;
    public GameObject explosionEffect;

    private NavMeshAgent agent;
    private Transform currentTarget;
    private bool isTargetingWall = false;

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

        if (isTargetingWall)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                currentTarget.position,
                moveSpeed * Time.deltaTime
            );
        }
        else
        {
            agent.SetDestination(currentTarget.position);
        }

        if (Vector3.Distance(transform.position, currentTarget.position) <= 1f)
        {
            Explode();
        }
    }

    void FindTarget()
    {
        // Используем ToArray() для преобразования в массив перед Where
        var walls = GameObject.FindObjectsOfType<Building>()
            .Where(b => b.type == Building.BuildingType.Fence && b.health > 0)
            .OrderBy(b => Vector3.Distance(transform.position, b.transform.position))
            .ToArray(); // Явное преобразование в массив

        if (walls.Length > 0)
        {
            currentTarget = walls[0].transform;
            isTargetingWall = true;
            agent.enabled = false;
            return;
        }

        isTargetingWall = false;
        agent.enabled = true;
        currentTarget = PriorityTargetSystem.GetPriorityTarget(transform, new[]
        {
            Building.BuildingType.Defensive,
            Building.BuildingType.Economic,
            Building.BuildingType.TownHall
        });
    }

    void Explode()
    {
        Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            var building = hit.GetComponent<Building>();
            if (building != null)
            {
                building.TakeDamage(explosionDamage);
            }
        }

        Destroy(gameObject);
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log("Подрывник получил " + amount + " урона. Осталось: " + health);

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        AchievementManager.Instance.IncrementProgress("Охотник за головами", 1);
        Debug.Log("Подрывник погиб.");
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}