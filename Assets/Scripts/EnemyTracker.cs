using UnityEngine;

public class EnemyTracker : MonoBehaviour
{
    private WaveSpawner spawner;

    public void Init(WaveSpawner spawner)
    {
        this.spawner = spawner;
    }

    void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.UnregisterEnemy(gameObject);
        }
    }
}