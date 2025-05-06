using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class SuicideSkeleton3D : MonoBehaviour
{
    public float moveSpeed = 8f;       // �������� ��������
    public float explosionRadius = 3f; // ������ ������
    public float explosionForce = 500f; // ���� ������������
    public float damage = 50f;         // ����
    public GameObject explosionEffect; // ������ ������ (Particle System)

    private Transform targetFence;
    private bool hasExploded = false;

    void Start()
    {
        // ���� ��������� �����
        GameObject fence = GameObject.FindGameObjectWithTag("Fence");
        if (fence != null)
            targetFence = fence.transform;
        else
            Debug.LogError("��� ������� � ����� 'Fence' �� �����!");
    }

    void Update()
    {
        if (targetFence == null || hasExploded) return;

        // �������� � ������
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetFence.position,
            moveSpeed * Time.deltaTime
        );

        // �������� ��������� ��� ������
        float distance = Vector3.Distance(transform.position, targetFence.position);
        if (distance < 1.5f) // ��������� ������
        {
            Explode();
        }
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // ������ ������
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // ������� ���� � ����������� �������
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hits)
        {
            // ���� ������
            if (hit.CompareTag("Fence"))
            {
                hit.GetComponent<FenceHealth>()?.TakeDamage(damage);
            }

            // ������������ �������� � Rigidbody
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        Destroy(gameObject); // ���������� �������
    }

    // ������������ ������� ������ � ���������
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
