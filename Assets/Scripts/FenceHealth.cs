using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// FenceHealth.cs
using UnityEngine;

public class FenceHealth : MonoBehaviour
{
    public float health = 100f;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject); // ћожно добавить разрушение с анимацией
        }
    }
}

