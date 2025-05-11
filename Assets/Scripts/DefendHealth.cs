using UnityEngine;

public class DefendHealth : MonoBehaviour
{
    [Header("���������")]
    public float health = 200f;
    public GameObject destroyEffect;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            DestroyBuilding();
        }
    }

    void DestroyBuilding()
    {
        // ������ ����������
        if (destroyEffect != null)
            Instantiate(destroyEffect, transform.position, Quaternion.identity);

        // ����������� �������
        Destroy(gameObject);
    }
}