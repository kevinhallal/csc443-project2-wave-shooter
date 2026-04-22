using System.Collections;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;

    [Header("Wave Settings")]
    [SerializeField] private int startingWave = 1;
    [SerializeField] private int baseEnemyCount = 5;
    [SerializeField] private int extraEnemiesPerWave = 2;
    [SerializeField] private float intermissionDuration = 5f;
    [SerializeField] private int maxWaves = 5;

    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI enemiesRemainingText;
    [SerializeField] private TextMeshProUGUI intermissionText;

    private int currentWave;
    private bool waveInProgress = false;

    private void Start()
    {
        currentWave = startingWave - 1;

        if (enemySpawner != null)
        {
            enemySpawner.OnWaveCleared += HandleWaveCleared;
            enemySpawner.OnWaveProgressChanged += UpdateEnemiesRemainingUI;
        }

        StartCoroutine(BeginNextWaveAfterDelay(1f));
    }

    private IEnumerator BeginNextWaveAfterDelay(float delay)
    {
        waveInProgress = false;

        if (intermissionText != null)
            intermissionText.gameObject.SetActive(true);

        float timer = delay;

        while (timer > 0)
        {
            if (intermissionText != null)
                intermissionText.text = "Intermission: " + Mathf.CeilToInt(timer);

            timer -= Time.deltaTime;
            yield return null;
        }

        if (intermissionText != null)
            intermissionText.gameObject.SetActive(false);

        StartNextWave();
    }

    private void StartNextWave()
    {
        currentWave++;

        if (currentWave > maxWaves)
        {
            if (intermissionText != null)
            {
                intermissionText.gameObject.SetActive(true);
                intermissionText.text = "All Waves Cleared!";
            }

            waveInProgress = false;
            return;
        }

        waveInProgress = true;

        int enemyCount = baseEnemyCount + ((currentWave - 1) * extraEnemiesPerWave);

        UpdateWaveUI();
        enemySpawner.StartWave(enemyCount);
    }

    private void HandleWaveCleared()
    {
        waveInProgress = false;
        StartCoroutine(BeginNextWaveAfterDelay(intermissionDuration));
    }

    private void UpdateWaveUI()
    {
        if (waveText != null)
        {
            waveText.text = "Wave: " + currentWave;
        }
    }

    private void UpdateEnemiesRemainingUI(int activeEnemies, int enemiesLeftToSpawn)
    {
        if (enemiesRemainingText != null)
        {
            int totalRemaining = activeEnemies + enemiesLeftToSpawn;
            enemiesRemainingText.text = "Enemies Remaining: " + totalRemaining;
        }
    }

    private void OnDestroy()
    {
        if (enemySpawner != null)
        {
            enemySpawner.OnWaveCleared -= HandleWaveCleared;
            enemySpawner.OnWaveProgressChanged -= UpdateEnemiesRemainingUI;
        }
    }
}