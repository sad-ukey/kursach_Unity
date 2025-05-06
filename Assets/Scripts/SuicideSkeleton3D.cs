using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class SuicideSkeleton3D : MonoBehaviour
{
    public float moveSpeed = 8f;       // Скорость движения
    public float explosionRadius = 3f; // Радиус взрыва
    public float explosionForce = 500f; // Сила отбрасывания
    public float damage = 50f;         // Урон
    public GameObject explosionEffect; // Префаб взрыва (Particle System)

    private Transform targetFence;
    private bool hasExploded = false;

    void Start()
    {
        // Ищем ближайший забор
        GameObject fence = GameObject.FindGameObjectWithTag("Fence");
        if (fence != null)
            targetFence = fence.transform;
        else
            Debug.LogError("Нет объекта с тегом 'Fence' на сцене!");
    }

    void Update()
    {
        if (targetFence == null || hasExploded) return;

        // Движение к забору
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetFence.position,
            moveSpeed * Time.deltaTime
        );

        // Проверка дистанции для взрыва
        float distance = Vector3.Distance(transform.position, targetFence.position);
        if (distance < 1.5f) // Дистанция взрыва
        {
            Explode();
        }
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Эффект взрыва
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Наносим урон и отбрасываем объекты
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hits)
        {
            // Урон забору
            if (hit.CompareTag("Fence"))
            {
                hit.GetComponent<FenceHealth>()?.TakeDamage(damage);
            }

            // Отбрасывание объектов с Rigidbody
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        Destroy(gameObject); // Уничтожаем скелета
    }

    // Визуализация радиуса взрыва в редакторе
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
