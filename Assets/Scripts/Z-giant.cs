using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GiantAI : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 2.5f;
    public float attackRange = 3f;
    public float attackDamage = 50f;
    public float attackCooldown = 2f;

    [Header("References")]
    public Animator animator;
    public GameObject hitEffect;

    private NavMeshAgent agent;
    private Transform targetDefend;
    private float lastAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        FindNearestDefend();
    }

    void Update()
    {
        if (targetDefend == null)
        {
            FindNearestDefend();
            animator.SetBool("IsWalking", false);
            return;
        }

        // �������� � ����
        agent.SetDestination(targetDefend.position);
        animator.SetBool("IsWalking", true);

        // �������� ��������� ��� �����
        float distance = Vector3.Distance(transform.position, targetDefend.position);
        if (distance <= attackRange)
        {
            agent.isStopped = true;
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            agent.isStopped = false;
        }
    }

    void FindNearestDefend()
    {
        GameObject[] defends = GameObject.FindGameObjectsWithTag("defend");
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
        // �������� (���� ����)
        animator.SetTrigger("Attack");

        // ���������� ������ �����
        Instantiate(hitEffect, targetDefend.position, Quaternion.identity);

        // ��������� �����
        targetDefend.GetComponent<DefendHealth>()?.TakeDamage(attackDamage);
    }
}
