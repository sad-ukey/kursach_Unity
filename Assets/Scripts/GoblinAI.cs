using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GoblinAI : MonoBehaviour, damageable
{
    public float moveSpeed = 5f;
    public float health = 50f;
    public float attackDamage = 20f;


    private NavMeshAgent agent;
    private Transform currentTarget;
    private Animator animator;

    private readonly Building.BuildingType[] priorityOrder =
    {
        Building.BuildingType.Economic,   
        Building.BuildingType.TownHall,  
        Building.BuildingType.Defensive
    };

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        if (!agent.isOnNavMesh)
        {
            Debug.LogError("������ �� ��������� �� NavMesh! ����������� ��� �� ������������� ����.");
            enabled = false; 
            return;
        }

        FindTarget();
    }

    void Update()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
        if (currentTarget == null)
        {
            FindTarget();
            return;
        }

        if (agent == null || !agent.isOnNavMesh)
        {
            Debug.LogWarning("NavMeshAgent �� ����� ��� ������ �� ����� �� NavMesh");
            return;
        }

        agent.SetDestination(currentTarget.position);

        if (Vector3.Distance(transform.position, currentTarget.position) <= 2f)
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
        if (currentTarget == null) return;

        Building building = currentTarget.GetComponent<Building>();
        if (building != null && !building.IsDestroyed())
        {
            animator.SetTrigger("Attack");
            building.TakeDamage(attackDamage);
            Debug.Log("������ ������� " + building.type);
        }
        else
        {
            Debug.Log("���� ���������� ��� ���������. ����� ����� ����.");
            FindTarget(); 
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log("������ ������� " + amount + " �����. ��������: " + health);

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        animator.SetTrigger("Death");
        AchievementManager.Instance.IncrementProgress("Охотник за головами", 1);
        Debug.Log("������ �����.");
        Destroy(gameObject);
    }
}