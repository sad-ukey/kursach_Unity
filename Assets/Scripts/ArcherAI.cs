using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(NavMeshAgent))]
public class ArcherAI : MonoBehaviour, damageable
{
    [Header("Настройки")]
    public float moveSpeed = 3.5f;
    public float attackRange = 8f;      // Большая дистанция атаки
    public float attackDamage = 25f;    // Средний урон
    public float attackCooldown = 2f;
    public float health = 60f;          // Малое здоровье

    [Header("Ссылки")]
    public GameObject attackEffect;
    public AudioClip attackSound;

    private NavMeshAgent agent;
    private Transform currentTarget;
    private float lastAttackTime;
    private AudioSource audioSource;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.stoppingDistance = attackRange * 0.9f; // Остановиться близко к цели

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        FindTarget();
    }

    void Update()
    {
        if (health <= 0) return;

        if (currentTarget == null)
        {
            FindTarget();
            return;
        }

        // Движение к цели
        agent.SetDestination(currentTarget.position);

        // Проверка дистанции для атаки
        float distance = Vector3.Distance(transform.position, currentTarget.position);
        if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    void FindTarget()
    {
        // Получаем все постройки, кроме стен
        var buildings = GameObject.FindObjectsOfType<Building>()
            .Where(b => b.type != Building.BuildingType.Fence && b.health > 0)
            .OrderBy(b => Vector3.Distance(transform.position, b.transform.position))
            .ToArray();

        // Если есть обычные постройки - атакуем ближайшую
        if (buildings.Length > 0)
        {
            currentTarget = buildings[0].transform;
            return;
        }

        // Если обычных построек нет - ищем стены, блокирующие путь
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
        // Эффект атаки (луч без физики)
        if (attackEffect != null)
        {
            Instantiate(attackEffect, transform.position, Quaternion.identity);
        }

        // Звук атаки
        if (attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        // Нанесение урона
        if (currentTarget != null)
        {
            Building building = currentTarget.GetComponent<Building>();
            if (building != null)
            {
                building.TakeDamage(attackDamage);
                Debug.Log($"Лучник атакует {building.type} (Урон: {attackDamage})");
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
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}