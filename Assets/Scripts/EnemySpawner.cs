using System;
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private EnemyHealth enemyPrefab;
    [SerializeField] private int prewarmCount = 10;

    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 1.5f;

    private ObjectPool<EnemyHealth> pool;

    public int ActiveEnemies { get; private set; }
    public int EnemiesLeftToSpawn { get; private set; }

    public event Action OnWaveCleared;
    public event Action<int, int> OnWaveProgressChanged;

    private void Start()
    {
        pool = new ObjectPool<EnemyHealth>(enemyPrefab, transform, prewarmCount);
    }

    public void StartWave(int enemyCount)
    {
        StopAllCoroutines();

        ActiveEnemies = 0;
        EnemiesLeftToSpawn = enemyCount;

        OnWaveProgressChanged?.Invoke(ActiveEnemies, EnemiesLeftToSpawn);

        StartCoroutine(SpawnWaveCoroutine());
    }

    private IEnumerator SpawnWaveCoroutine()
    {
        while (EnemiesLeftToSpawn > 0)
        {
            SpawnEnemy();
            EnemiesLeftToSpawn--;

            OnWaveProgressChanged?.Invoke(ActiveEnemies, EnemiesLeftToSpawn);

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (spawnPoints.Length == 0) return;

        Transform point = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        EnemyHealth enemy = pool.Get(point.position, point.rotation);

        ActiveEnemies++;
        enemy.OnDied += HandleEnemyDied;

        OnWaveProgressChanged?.Invoke(ActiveEnemies, EnemiesLeftToSpawn);
    }

    private void HandleEnemyDied(EnemyHealth enemy)
    {
        enemy.OnDied -= HandleEnemyDied;
        ActiveEnemies--;

        pool.Return(enemy);

        OnWaveProgressChanged?.Invoke(ActiveEnemies, EnemiesLeftToSpawn);

        if (ActiveEnemies <= 0 && EnemiesLeftToSpawn <= 0)
        {
            OnWaveCleared?.Invoke();
        }
    }
}