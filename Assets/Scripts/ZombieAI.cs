using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieAI : MonoBehaviour, damageable
{
    [Header("Zombie Settings")]
    public float moveSpeed = 3.5f;
    public float health = 100f;
    public float attackDamage = 15f;
    public float attackCooldown = 1.5f;
    public float detectionRange = 50f;
    public float pathUpdateInterval = 0.5f;

    private NavMeshAgent agent;
    private Animator animator;
    private Building targetBuilding;
    private Building blockingFence;
    private float nextAttackTime;
    private float lastPathUpdateTime;
    private bool hasDirectPath;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        FindNewTarget();
    }

    void Update()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);

        if (targetBuilding == null)
        {
            FindNewTarget();
            return;
        }

        // Обновление пути с интервалом
        if (Time.time - lastPathUpdateTime > pathUpdateInterval)
        {
            UpdateNavigation();
            lastPathUpdateTime = Time.time;
        }

        // Логика атаки
        if (blockingFence != null &&
            Vector3.Distance(transform.position, blockingFence.transform.position) <= 2.2f &&
            Time.time >= nextAttackTime)
        {
            Attack(blockingFence);
        }
        else if (hasDirectPath &&
                 Vector3.Distance(transform.position, targetBuilding.transform.position) <= 2.2f &&
                 Time.time >= nextAttackTime)
        {
            Attack(targetBuilding);
        }
    }

    void FindNewTarget()
    {
        // Ищем ближайшее здание (не забор)
        Building closestBuilding = null;
        float closestDistance = Mathf.Infinity;

        foreach (Building building in FindObjectsOfType<Building>())
        {
            if (building.type == Building.BuildingType.Fence) continue;

            float distance = Vector3.Distance(transform.position, building.transform.position);
            if (distance < closestDistance && distance <= detectionRange)
            {
                closestDistance = distance;
                closestBuilding = building;
            }
        }

        targetBuilding = closestBuilding;
        blockingFence = null;
        UpdateNavigation();
    }

    void UpdateNavigation()
    {
        if (targetBuilding == null) return;

        // Проверяем прямой путь к зданию
        NavMeshPath path = new NavMeshPath();
        hasDirectPath = agent.CalculatePath(targetBuilding.transform.position, path) &&
                        path.status == NavMeshPathStatus.PathComplete;

        if (hasDirectPath)
        {
            // Прямой путь доступен
            blockingFence = null;
            agent.SetDestination(targetBuilding.transform.position);
        }
        else
        {
            // Ищем ближайший забор к зданию
            FindBlockingFence();

            if (blockingFence != null)
            {
                agent.SetDestination(blockingFence.transform.position);
            }
            else
            {
                // Если забор не найден, все равно пытаемся идти к зданию
                agent.SetDestination(targetBuilding.transform.position);
            }
        }
    }

    void FindBlockingFence()
    {
        float closestDistance = Mathf.Infinity;
        blockingFence = null;

        foreach (Building building in FindObjectsOfType<Building>())
        {
            if (building.type != Building.BuildingType.Fence || !building.isBlockingPath) continue;

            // Ищем заборы в радиусе 10м от здания
            float distanceToBuilding = Vector3.Distance(building.transform.position,
                                                     targetBuilding.transform.position);
            float distanceToZombie = Vector3.Distance(building.transform.position,
                                                    transform.position);

            if (distanceToBuilding < 10f && distanceToZombie < detectionRange)
            {
                if (distanceToBuilding < closestDistance)
                {
                    closestDistance = distanceToBuilding;
                    blockingFence = building;
                }
            }
        }
    }

    void Attack(Building target)
    {
        animator.SetTrigger("Attack");
        target.TakeDamage(attackDamage);
        nextAttackTime = Time.time + attackCooldown;

        if (target.IsDestroyed())
        {
            if (target == blockingFence)
            {
                blockingFence = null;
                UpdateNavigation();
            }
            else
            {
                FindNewTarget();
            }
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0) Die();
    }

    void Die()
    {
        animator.SetTrigger("Death");
        AchievementManager.Instance.IncrementProgress("Охотник за головами", 1);
        Destroy(gameObject, 1f);
    }
}