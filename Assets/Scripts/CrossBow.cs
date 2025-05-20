using UnityEngine;

public class CrossBow : MonoBehaviour
{
    public float attackRange = 10f;
    public float damage = 15f;
    public float fireRate = 1f;

    private float fireCooldown = 0f;

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f)
        {
            GameObject target = FindAnyEnemyInRange();
            if (target != null)
            {
                Attack(target);
                fireCooldown = 1f / fireRate;
            }
        }
    }

    GameObject FindAnyEnemyInRange()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= attackRange)
            {
                return enemy;
            }
        }

        return null;
    }

    void Attack(GameObject enemy)
    {
        damageable damageable = enemy.GetComponent<damageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            Debug.Log("Арбалет нанес" + damage + " урона врагу: " + enemy.name);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
