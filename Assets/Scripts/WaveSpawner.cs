using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyWave
    {
        public GameObject enemyPrefab;
        public int count;
    }

    public List<EnemyWave> waves;
    public Transform[] spawnPoints; 
    public float spawnDelay = 0.5f;

    private bool isSpawning = false;

    public void StartNextWave()
    {
        if (!isSpawning)
        {
            StartCoroutine(SpawnWaveCoroutine());
        }
    }

    private System.Collections.IEnumerator SpawnWaveCoroutine()
    {
        isSpawning = true;

        foreach (var wave in waves)
        {
            for (int i = 0; i < wave.count; i++)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Instantiate(wave.enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                yield return new WaitForSeconds(spawnDelay);
            }
        }

        isSpawning = false;
    }
}