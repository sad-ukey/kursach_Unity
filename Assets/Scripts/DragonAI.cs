using UnityEngine;
using System.Collections;

public class DragonAI : MonoBehaviour
{
    [Header("Характеристики Дракона")]
    public float moveSpeed = 5f;
    public float health = 300f;
    public float attackDamage = 25f;
    public float attackDelay = 2.5f;
    public float attackRange = 2f;
    public float flightHeight = 10f;

    private Transform currentTarget;
    private bool isAttacking = false;

    void Start()
    {
        FindTarget();
        SetFlightHeight();
    }

    void Update()
    {
        if (currentTarget == null)
        {
            FindTarget();
            return;
        }

        SetFlightHeight();

        float distance = Vector3.Distance(transform.position, currentTarget.position);

        if (distance > attackRange)
        {
            MoveToTarget();
        }
        else
        {
            if (!isAttacking)
            {
                StartCoroutine(AttackRoutine());
            }
        }
    }

    void SetFlightHeight()
    {
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, flightHeight, pos.z);
    }

    void MoveToTarget()
    {
        Vector3 direction = (currentTarget.position - transform.position).normalized;
        direction.y = 0; // Исключаем вертикальное движение
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Поворачиваемся к цели
        transform.forward = direction;
    }

    void FindTarget()
    {
        Building[] buildings = FindObjectsOfType<Building>();
        float closestDistance = Mathf.Infinity;
        Building bestTarget = null;

        foreach (var building in buildings)
        {
            if (building.type == Building.BuildingType.Fence) continue; // Игнорируем забор

            float distance = Vector3.Distance(transform.position, building.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                bestTarget = building;
            }
        }

        if (bestTarget != null)
        {
            currentTarget = bestTarget.transform;
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        Building building = currentTarget.GetComponent<Building>();
        if (building != null)
        {
            building.TakeDamage(attackDamage);
            Debug.Log("Дракон атакует " + building.type);

            if (building.IsDestroyed())
            {
                currentTarget = null;
            }
        }

        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
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
        Debug.Log("Дракон погиб");
        Destroy(gameObject);
    }
}