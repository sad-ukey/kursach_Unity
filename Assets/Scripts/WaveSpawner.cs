using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyInfo
    {
        public GameObject enemyPrefab;
        public int count;
    }

    [System.Serializable]
    public class EnemyWave
    {
        public List<EnemyInfo> enemies = new List<EnemyInfo>();
    }

    public List<EnemyWave> waves = new List<EnemyWave>();
    public Transform[] spawnPoints;
    public float spawnDelay = 0.5f;

    private int currentWaveIndex = 0;
    private bool isSpawning = false;

    private List<GameObject> aliveEnemies = new List<GameObject>();

    public void StartNextWave()
    {
        if (isSpawning || currentWaveIndex >= waves.Count)
            return;

        if (aliveEnemies.Count > 0)
        {
            Debug.Log("Нельзя начать следующую волну — враги ещё живы.");
            return;
        }


        StartCoroutine(SpawnWaveCoroutine(waves[currentWaveIndex]));
        currentWaveIndex++;
    }

    private IEnumerator SpawnWaveCoroutine(EnemyWave wave)
    {
        isSpawning = true;

        foreach (var enemyInfo in wave.enemies)
        {
            for (int i = 0; i < enemyInfo.count; i++)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                GameObject enemy = Instantiate(enemyInfo.enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                aliveEnemies.Add(enemy);

                if (enemy.GetComponent<EnemyTracker>() == null)
                {
                    enemy.AddComponent<EnemyTracker>().Init(this);
                }

                yield return new WaitForSeconds(spawnDelay);
            }
        }

        isSpawning = false;
    }

    public void UnregisterEnemy(GameObject enemy)
    {
        aliveEnemies.Remove(enemy);

        if (currentWaveIndex == 1 && aliveEnemies.Count==0)
        {
            AchievementManager.Instance.IncrementProgress("Твердая оборона", 1);
        }

        if (currentWaveIndex == 3 && aliveEnemies.Count == 0)
        {
            AchievementManager.Instance.IncrementProgress("Нас 25 тысяч!", 1);
        }

        if (currentWaveIndex == 5 && aliveEnemies.Count == 0)
        {
            AchievementManager.Instance.IncrementProgress("Z!", 1);
        }
    }
}