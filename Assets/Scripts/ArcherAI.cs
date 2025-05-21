using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(NavMeshAgent))]
public class ArcherAI : MonoBehaviour, damageable
{
    [Header("���������")]
    public float moveSpeed = 3.5f;
    public float attackRange = 8f;      // ������� ��������� �����
    public float attackDamage = 25f;    // ������� ����
    public float attackCooldown = 2f;
    public float health = 60f;          // ����� ��������

    [Header("������")]
    public GameObject attackEffect;
    public AudioClip attackSound;

    private NavMeshAgent agent;
    private Transform currentTarget;
    private float lastAttackTime;
    private AudioSource audioSource;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.stoppingDistance = attackRange * 0.9f; // ������������ ������ � ����

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        FindTarget();
    }

    void Update()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
        if (health <= 0) return;

        if (currentTarget == null)
        {
            FindTarget();
            return;
        }

        // �������� � ����
        agent.SetDestination(currentTarget.position);

        // �������� ��������� ��� �����
        float distance = Vector3.Distance(transform.position, currentTarget.position);
        if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    void FindTarget()
    {
        // �������� ��� ���������, ����� ����
        var buildings = GameObject.FindObjectsOfType<Building>()
            .Where(b => b.type != Building.BuildingType.Fence && b.health > 0)
            .OrderBy(b => Vector3.Distance(transform.position, b.transform.position))
            .ToArray();

        // ���� ���� ������� ��������� - ������� ���������
        if (buildings.Length > 0)
        {
            currentTarget = buildings[0].transform;
            return;
        }

        // ���� ������� �������� ��� - ���� �����, ����������� ����
        var blockingWalls = GameObject.FindObjectsOfType<Building>()
            .Where(b => b.type == Building.BuildingType.Fence &&
                       b.health > 0 &&
                       b.isBlockingPath)
            .OrderBy(b => Vector3.Distance(transform.position, b.transform.position))
            .ToArray();

        if (blockingWalls.Length > 0)
        {
            currentTarget = blockingWalls[0].transform;
        }
    }

    void Attack()
    {
        // ������ ����� (��� ��� ������)
        if (attackEffect != null)
        {
            Instantiate(attackEffect, transform.position, Quaternion.identity);
        }

        // ���� �����
        if (attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        // ��������� �����
        if (currentTarget != null)
        {
            animator.SetTrigger("Attack");
            Building building = currentTarget.GetComponent<Building>();
            if (building != null)
            {
                building.TakeDamage(attackDamage);
                Debug.Log($"������ ������� {building.type} (����: {attackDamage})");
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        animator.SetTrigger("Death");
        Destroy(gameObject);
        AchievementManager.Instance.IncrementProgress("������� �� ��������", 1);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}