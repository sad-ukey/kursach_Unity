using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GiantAI : MonoBehaviour, damageable
{
    public float moveSpeed = 3f;
    public float attackRange = 2f;
    public float attackDamage = 40f;
    public float health = 50f;

    private NavMeshAgent agent;
    private Transform currentTarget;
    private Animator animator;
    private readonly Building.BuildingType[] priorityOrder =
    {
        Building.BuildingType.Defensive, 
        Building.BuildingType.TownHall,  
        Building.BuildingType.Economic
    };

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
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
            animator.SetTrigger("Attack");
            building.TakeDamage(attackDamage);
            Debug.Log("������ ������� " + building.type);
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
        Debug.Log("������ � ����.");
        Destroy(gameObject);
    }   
}