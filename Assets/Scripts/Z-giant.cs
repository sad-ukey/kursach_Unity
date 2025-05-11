using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class GiantAI : MonoBehaviour
{
    [Header("���������")]
    public float moveSpeed = 3.5f;
    public float attackRange = 2f;
    public float attackDamage = 40f;
    public float attackCooldown = 2f;

    [Header("������")]
    public Animator animator;
    public GameObject hitEffect;
    public AudioClip attackSound;

    private NavMeshAgent agent;
    private Transform targetDefend;
    private float lastAttackTime;
    private AudioSource audioSource;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.stoppingDistance = attackRange - 0.5f; // ������������ ���� ������

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        FindNearestDefend();
    }

    void Update()
    {
        if (targetDefend == null)
        {
            FindNearestDefend();
            return;
        }

        // �������� � ����
        agent.SetDestination(targetDefend.position);
        animator.SetBool("IsWalking", agent.velocity.magnitude > 0.1f);

        // �������� ��������� ��� �����
        float distance = Vector3.Distance(transform.position, targetDefend.position);
        if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    void FindNearestDefend()
    {
        GameObject[] defends = GameObject.FindGameObjectsWithTag("Defend");
        if (defends.Length == 0)
        {
            Debug.LogWarning("��� �������� � ����� 'defend'!");
            return;
        }

        float closestDistance = Mathf.Infinity;
        foreach (GameObject defend in defends)
        {
            float distance = Vector3.Distance(transform.position, defend.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                targetDefend = defend.transform;
            }
        }
    }

    void Attack()
    {
        // ��������
        animator.SetTrigger("Attack");

        // ����
        if (attackSound != null)
            audioSource.PlayOneShot(attackSound);

        // ������ �����
        if (hitEffect != null)
            Instantiate(hitEffect, targetDefend.position, Quaternion.identity);

        // ��������� �����
        if (targetDefend != null)
        {
            DefendHealth defendHealth = targetDefend.GetComponent<DefendHealth>();
            if (defendHealth != null)
            {
                defendHealth.TakeDamage(attackDamage);
            }
        }
    }

    // ������������ ������� ����� � ���������
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}