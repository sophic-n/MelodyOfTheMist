using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject[] lowEnemyPrefabs;
    public GameObject[] mediumEnemyPrefabs;
    public GameObject[] highEnemyPrefabs;
    public GameObject[] veryHighEnemyPrefabs;


    public Transform playerTarget;

    public float timeBetweenWaves = 3f;
    private int currentWave = 0;
    private int totalWaves = 5;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private GameStateManager gameStateManager;

    void Start()
    {
        gameStateManager = FindObjectOfType<GameStateManager>();
        StartCoroutine(SpawnNextWave());
    }

    IEnumerator SpawnNextWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);

        currentWave++;

        if (currentWave > totalWaves)
        {
            Debug.Log("[WaveSpawner] All waves completed!");
            yield break;
        }

        Debug.Log($"[WaveSpawner] Spawning Wave {currentWave}");

        int enemiesThisWave = 3 + currentWave * 2;

        List<Enemy.Difficulty> waveDifficulties = new List<Enemy.Difficulty>();
        for (int i = 0; i < enemiesThisWave; i++)
        {
            waveDifficulties.Add(GetEnemyDifficultyForWave(currentWave));
        }

        StartCoroutine(SpawnEnemiesWithDelay(waveDifficulties));
    }

    IEnumerator SpawnEnemiesWithDelay(List<Enemy.Difficulty> waveDifficulties)
    {
        foreach (var difficulty in waveDifficulties)
        {
            SpawnEnemy(difficulty);
            yield return new WaitForSeconds(1.5f); //  Adjust spawn delay 
        }

        StartCoroutine(CheckForWaveClear());
    }


    void SpawnEnemy(Enemy.Difficulty difficulty)
    {
        GameObject prefab = GetPrefabForDifficulty(difficulty);
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject enemyGO = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        Enemy enemy = enemyGO.GetComponent<Enemy>();
        enemy.playerTarget = playerTarget;

        activeEnemies.Add(enemyGO);
        gameStateManager?.RegisterEnemy();

        // 🔊 Play enemy spawn sound
        AudioManager.Instance.PlayRandomSFX(AudioManager.Instance.enemySpawns, "enemySpawn");
    }

    Enemy.Difficulty GetEnemyDifficultyForWave(int wave)
    {
        float r = Random.value;

        return wave switch
        {
            1 => (r < 0.9f) ? Enemy.Difficulty.Low : Enemy.Difficulty.Medium,
            2 => (r < 0.6f) ? Enemy.Difficulty.Low : (r < 0.9f ? Enemy.Difficulty.Medium : Enemy.Difficulty.High),
            3 => (r < 0.4f) ? Enemy.Difficulty.Medium : (r < 0.85f ? Enemy.Difficulty.High : Enemy.Difficulty.VeryHigh),
            4 => (r < 0.2f) ? Enemy.Difficulty.Medium : (r < 0.9f ? Enemy.Difficulty.High : Enemy.Difficulty.VeryHigh),
            5 => (r < 0.8f) ? Enemy.Difficulty.High : Enemy.Difficulty.VeryHigh,
            _ => Enemy.Difficulty.Low
        };
    }

    GameObject GetPrefabForDifficulty(Enemy.Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Enemy.Difficulty.Low:
                return lowEnemyPrefabs[Random.Range(0, lowEnemyPrefabs.Length)];
            case Enemy.Difficulty.Medium:
                return mediumEnemyPrefabs[Random.Range(0, mediumEnemyPrefabs.Length)];
            case Enemy.Difficulty.High:
                return highEnemyPrefabs[Random.Range(0, highEnemyPrefabs.Length)];
            case Enemy.Difficulty.VeryHigh:
                return veryHighEnemyPrefabs[Random.Range(0, veryHighEnemyPrefabs.Length)];
            default:
                return lowEnemyPrefabs[0];
        }
    }

    IEnumerator CheckForWaveClear()
{
    var inputManager = FindObjectOfType<PlayerInputManager>();

    while (activeEnemies.Count > 0)
    {
        // Remove null enemies and unregister them
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i] == null)
            {
                if (inputManager != null)
                {
                    inputManager.UnregisterEnemy(null); // null-safe call
                }

                activeEnemies.RemoveAt(i);
            }
        }

        yield return new WaitForSeconds(1f);
    }

    Debug.Log($"[WaveSpawner] Wave {currentWave} cleared!");
    StartCoroutine(SpawnNextWave());
}

    
    public void ForceNextWave()
{
    Debug.Log("[WaveSpawner] Force skipping to next wave.");

    // Destroy all active enemies immediately
    foreach (var enemy in activeEnemies)
    {
        if (enemy != null)
            Destroy(enemy);
    }

    activeEnemies.Clear();

    StopAllCoroutines(); // Stop any pending wave checks/spawns

    StartCoroutine(SpawnNextWave());
}
}
