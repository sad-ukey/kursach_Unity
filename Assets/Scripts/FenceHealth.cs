using UnityEngine;

public class FenceHealth : Building
{
    [Header("Настройки забора")]
    public float baseHealth = 100f;
    public GameObject destroyEffect;

    [Header("Блокировка пути")]
    public bool alwaysBlocking = false;
    public float checkRadius = 5f;

    private void Start()
    {
        type = BuildingType.Fence;
        health = baseHealth;
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage); 

        if (health <= 0 && destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }
    }

    private void Update()
    {
        if (!alwaysBlocking)
        {
            isBlockingPath = CheckIfBlockingPath();
        }
    }

    private bool CheckIfBlockingPath()
    {
        Collider[] buildingsBehind = Physics.OverlapSphere(
            transform.position + transform.forward * checkRadius,
            checkRadius
        );

        foreach (var building in buildingsBehind)
        {
            Building b = building.GetComponent<Building>();
            if (b != null && b.type != BuildingType.Fence && b.health > 0)
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (!alwaysBlocking)
        {
            Gizmos.color = isBlockingPath ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(
                transform.position + transform.forward * checkRadius,
                checkRadius
            );
        }
    }
}